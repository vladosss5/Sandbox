using System.Runtime.InteropServices;
using System.Text;

namespace DiscountingTest;

public class DiscountingMethods
{
    private const string DllName = "Discounting.dll";

    /// <summary>
    /// Возвращает результат инициализации.
    /// </summary>
    /// <param name="szInputParam"></param>
    /// <returns>Результат для последующих вычислений. Должен существовать как singleton.</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Winapi)]
    public static extern IntPtr calculate_Initialize(string szInputParam = "");
    
    /// <summary>
    /// Удаление результата инициализации. Всегда должен вызываться при выходе из приложения.
    /// </summary>
    /// <param name="pData">Результат инициализации.</param>
    /// <returns>Ничего.</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Winapi)]
    public static extern IntPtr calculate_Release(IntPtr pData);
    
    /// <summary>
    /// Расчёт.
    /// </summary>
    /// <param name="pData">Результат инициализации.</param>
    /// <param name="szInputXml">(in.xml)</param>
    /// <param name="szInputLimitation">(limit.xml)</param>
    /// <param name="szInputSchema">(dsc)</param>
    /// <param name="szInputParam"></param>
    /// <param name="pOutputXmlBuffer">Выходной буфер. (out.xml должен получиться).</param>
    /// <param name="nOutputXmlBufferSize">Максимальный размер буфера.</param>
    /// <param name="pnReturnBytes">Сколько байт заполнено результатом.</param>
    /// <returns></returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Winapi)]
    public static extern IntPtr calculate_Process(
        IntPtr pData,
        string szInputXml,
        string szInputLimitation,
        string szInputSchema,
        string szInputParam,
        byte[] pOutputXmlBuffer,
        int nOutputXmlBufferSize,
        ref uint pnReturnBytes);
}