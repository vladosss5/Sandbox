using System.Net.Sockets;

namespace ConsoleApp;

public class SpUsbClient : IAsyncDisposable
{
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _stream;
    private readonly SemaphoreSlim _lock = new(1, 1);
    
    /// <summary>
    /// Код начала посылки.
    /// </summary>
    private const byte FEND = 0xC0;
    
    /// <summary>
    /// Код передачи ESC-последовательностей.
    /// </summary>
    private const byte FESC = 0xDB;
    private const byte TFEND = 0xDC;
    private const byte TFESC = 0xDD;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SpUsbClient(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        _stream = tcpClient.GetStream();
    }

    public static async Task<SpUsbClient> ConnectAsync(string host, int port, CancellationToken ct = default)
    {
        var client = new TcpClient();
        await client.ConnectAsync(host, port, ct);
        return new SpUsbClient(client);
    }
    
    /// <summary>
    /// Читает пакет из потока и делает обратный стаффинг.
    /// </summary>
    public async Task<byte[]> ReadPacketAsync(CancellationToken ct = default)
    {
        var buffer = new List<byte>();
        var singleByte = new byte[1];
        var packetStarted = false;

        while (true)
        {
            var read = await _stream.ReadAsync(singleByte.AsMemory(0, 1), ct);
            if (read == 0) 
                throw new IOException("Соединение закрыто.");

            var b = singleByte[0];

            if (b == FEND)
            {
                if (packetStarted && buffer.Count > 0) 
                    break;
                
                packetStarted = true;
                continue;
            }

            if (!packetStarted) 
                continue;

            buffer.Add(b);
        }

        var rawData = new List<byte>();
        for (var i = 0; i < buffer.Count; i++)
        {
            if (buffer[i] == FESC && i + 1 < buffer.Count)
            {
                switch (buffer[i + 1])
                {
                    case TFEND:
                        rawData.Add(FEND);
                        break;
                    case TFESC:
                        rawData.Add(FESC);
                        break;
                }

                i++;
            }
            else
            {
                rawData.Add(buffer[i]);
            }
        }

        if (rawData.Count > 3)
        {
            byte expectedCrc = rawData[^1];
            byte calculatedCrc = 0;
            
            calculatedCrc += FEND; 
            
            for (int i = 0; i < rawData.Count - 1; i++) calculatedCrc += rawData[i];

            if (calculatedCrc != expectedCrc)
                throw new InvalidDataException($"Ошибка CRC. Ожидалось {expectedCrc:X2}, получено {calculatedCrc:X2}");
        }

        return rawData.ToArray();
    }
    
    public async Task<byte[]> SendCommandAsync(byte address, byte command, byte[] data, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var packet = BuildPacket(address, command, data);
            await _stream.WriteAsync(packet, ct);
            await _stream.FlushAsync(ct);

            return await ReadPacketAsync(ct);
        }
        finally
        {
            _lock.Release();
        }
    }
    
    /// <summary>
    /// Собирает пакет с учетом CRC и стаффинга.
    /// </summary>
    /// <summary>
    /// Собирает пакет с учетом CRC и стаффинга.
    /// </summary>
    private static byte[] BuildPacket(byte address, byte command, byte[] data)
    {
        var payload = new List<byte> { address, command, (byte)data.Length };
        payload.AddRange(data);

        var crc = payload.Aggregate(FEND, (current, b) => (byte)(current + b));
        payload.Add(crc);

        var finalPacket = new List<byte> { FEND };
    
        foreach (var b in payload)
        {
            switch (b)
            {
                case FEND:
                    finalPacket.Add(FESC); 
                    finalPacket.Add(TFEND);
                    break;
                case FESC:
                    finalPacket.Add(FESC); 
                    finalPacket.Add(TFESC);
                    break;
                default:
                    finalPacket.Add(b);
                    break;
            }
        }

        var finalPacketArray = finalPacket.ToArray();
        Console.WriteLine(BitConverter.ToString(finalPacketArray));
    
        return finalPacketArray;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
        _tcpClient.Dispose();
        _lock.Dispose();
    }
}