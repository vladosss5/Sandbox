namespace SpClientConsole;

/// <summary>
/// Модель пакета для общения с SP.
/// </summary>
public class WakePacket
{
    // /// <summary>
    // /// Конструктор.
    // /// </summary>
    // /// <param name="frameEnd"></param>
    // /// <param name="addressL"></param>
    // /// <param name="command">Команда.</param>
    // /// <param name="addressTrk">Адрес отдельного канала ТРК.</param>
    // /// <param name="dataLenght">Длина массива полезной нагрузки.</param>
    // /// <param name="data">Массив полезной нагрузки.</param>
    // /// <param name="checkSum">Контрольная сумма.</param>
    // public WakePacket(byte frameEnd, byte addressL, SpCommand command, byte addressTrk, byte dataLenght, byte[] data, byte checkSum)
    // {
    //     FrameEnd = 0xC0;
    //     AddressL = 0x01;
    //     
    //     Command = command;
    //     AddressTrk = addressTrk;
    //     DataLenght = dataLenght;
    //     Data = data;
    //     CheckSum = checkSum;
    // }
    
    /// <summary>
    /// Байт начала пакета.
    /// </summary>
    public byte FrameEnd { get; set; } 

    /// <summary>
    /// Байт сетевого адреса устройства, для обращения к Концентратору принимает значение 01.
    /// </summary>
    public byte AddressL { get; set; }
    
    /// <summary>
    /// Команда выполняемая SP.
    /// </summary>
    public byte Command { get; set; }
    
    /// <summary>
    /// Байт адреса отдельного канала ТРК.
    /// Для адресации используется 7 бит (0…127).
    /// Для broadcast сообщения использовать 0xFF.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Возникает при выходе знчения из допустимого диапазона (0…127||255).</exception>
    public byte AddressTrk
    {
        get;
        set
        {
            if (value > 0x7F && value != 0xFF)
                throw new ArgumentOutOfRangeException(nameof(AddressTrk), value, "Адрес должен быть в диапазоне 0..127 (7 бит).");
            
            field = value;
        }
    }
    
    /// <summary>
    /// Кол-во байт полезной нагрузки без учёта стаффинга.
    /// </summary>
    public byte DataLenght { get; set; }

    /// <summary>
    /// Массив байт полезной нагрузки.
    /// </summary>
    public List<byte> Data { get; set; } = [];
    
    /// <summary>
    /// Контрольная сумма пакета.
    /// </summary>
    public byte CheckSum { get; set; }
}