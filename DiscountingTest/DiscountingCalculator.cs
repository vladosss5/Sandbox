using System.Text;

namespace DiscountingTest;

public class DiscountingCalculator : IDisposable
{
    private IntPtr _initData;
    private bool _disposed;
    
    public DiscountingCalculator()
    {
        _initData = DiscountingMethods.calculate_Initialize();

        Console.WriteLine($"Инициализирован калькулятор {_initData}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="szInputXml">(in.xml)</param>
    /// <param name="szInputLimitation">(limit.xml)</param>
    /// <param name="szInputSchema">(dsc)</param>
    /// <param name="szInputParam"></param>
    /// <param name="pOutputXmlBuffer">Выходной буфер. (out.xml должен получиться).</param>
    /// <param name="nOutputXmlBufferSize">Максимальный размер буфера.</param>
    /// <param name="pnReturnBytes">Сколько байт заполнено результатом.</param>
    public string CalculateProcess(
        string szInputXml,
        string szInputLimitation,
        string szInputSchema,
        string szInputParam,
        byte[] pOutputXmlBuffer,
        int nOutputXmlBufferSize,
        ref uint pnReturnBytes)
    {
        try
        {
            var result = DiscountingMethods.calculate_Process(
                _initData, szInputXml, 
                szInputLimitation, 
                szInputSchema, 
                szInputParam,
                pOutputXmlBuffer, 
                nOutputXmlBufferSize, 
                ref pnReturnBytes);
            
            var resultString = Encoding.UTF8.GetString(pOutputXmlBuffer, 0, (int)pnReturnBytes);

            return resultString;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    public void Dispose()
    {
        DiscountingMethods.calculate_Release(_initData);
        Console.WriteLine("Вызван Dispose калькулятора.");
    }
}