using System.Net.Sockets;

namespace SpClientConsole;

public class SpClient
{
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _stream;
    private readonly SemaphoreSlim _lock = new(1, 1);
    
    private readonly byte[] _readBuffer = new byte[4096];
    private int _readBufferOffset;
    private int _readBufferLength;

    private const byte FEND = 0xC0;
    private const byte FESC = 0xDB;
    private const byte TFEND = 0xDC;
    private const byte TFESC = 0xDD;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    private SpClient(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        _stream = tcpClient.GetStream();
    }
    
    /// <summary>
    /// Создать подключение.
    /// </summary>
    /// <param name="host">Адрес SP.</param>
    /// <param name="port">Порт SP.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Экземпляр готового подключения.</returns>
    public static async Task<SpClient> ConnectAsync(string host, int port, CancellationToken ct = default)
    {
        var client = new TcpClient();
        await client.ConnectAsync(host, port, ct);
        return new SpClient(client);
    }
    
    /// <summary>
    /// Прочитать пакет.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Пакет данных протокола Wake.</returns>
    public async Task<WakePacket> ReadPacketAsync(CancellationToken ct = default)
    {
        var buffer = new List<byte>();
        await FillBufferWithPacket(buffer, ct);
        var result = FillWakePacketWhithBuffer(buffer);

        return result;
    }

    private async Task FillBufferWithPacket(List<byte> buffer, CancellationToken ct = default)
    {
        var packetStarted = false;
        
        while (true)
        {
            var b = await ReadByteAsync(ct);
            if (b == FEND)
            {
                if (packetStarted && buffer.Count > 0) 
                    break;
                packetStarted = true;
            }

            if (!packetStarted) 
                continue;
            
            buffer.Add(b);
        }
    }
    
    private async Task<byte> ReadByteAsync(CancellationToken ct)
    {
        if (_readBufferOffset < _readBufferLength) 
            return _readBuffer[_readBufferOffset++];
        
        _readBufferLength = await _stream.ReadAsync(_readBuffer.AsMemory(), ct);
        _readBufferOffset = 0;
        
        return _readBufferLength == 0 
            ? throw new IOException("Соединение закрыто.") 
            : _readBuffer[_readBufferOffset++];
    }

    private static WakePacket FillWakePacketWhithBuffer(List<byte> buffer)
    {
        var resultPacket = new WakePacket();
        var readState = WakeElement.FEND;
        var stuffCount = 0;
        
        for (var i = 0; i < buffer.Count; i++)
        {
            switch (readState)
            {
                case WakeElement.FEND when buffer[i] == FEND:
                    resultPacket.FrameEnd = buffer[i];
                    readState = WakeElement.ADDR_L;
                    break;
                
                case WakeElement.ADDR_L:
                    resultPacket.AddressL = buffer[i];
                    readState = WakeElement.CMD;
                    break;
                
                case WakeElement.CMD:
                    resultPacket.Command = buffer[i];
                    readState = WakeElement.ADDR_TRK;
                    break;
                
                case WakeElement.ADDR_TRK:
                    resultPacket.AddressTrk = buffer[i];
                    readState = WakeElement.N;
                    break;
                
                case WakeElement.N:
                    resultPacket.DataLenght = buffer[i];
                    readState = WakeElement.ElementDataArray;
                    break;
                
                case WakeElement.ElementDataArray:
                    switch (buffer[i])
                    {
                        case FESC when buffer[i+1] == TFEND:
                            resultPacket.Data.Add(FEND);
                            stuffCount++;
                            i++;
                            break;
                        
                        case FESC when buffer[i+1] == TFESC:
                            resultPacket.Data.Add(FESC);
                            stuffCount++;
                            i++;
                            break;
                        
                        default:
                            resultPacket.Data.Add(buffer[i]);
                            break;
                    }

                    if (i == resultPacket.DataLenght + 4 + stuffCount)
                        readState = WakeElement.CRC;
                    
                    break;
                
                case WakeElement.CRC:
                    readState = WakeElement.FEND;
                    break;
            }    
        }

        return resultPacket;
    }
}