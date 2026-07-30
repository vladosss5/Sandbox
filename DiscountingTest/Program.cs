using System.Text;

namespace DiscountingTest;

internal static class MyClass
{
    public static async Task Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.OutputEncoding = Encoding.GetEncoding("windows-1251");
        Console.InputEncoding = Encoding.GetEncoding("windows-1251");
        using var discountingCalculator = new DiscountingCalculator();

        var inputXml = await File.ReadAllTextAsync("in-cardinfo.xml");
        var limitationXml = await File.ReadAllTextAsync("limit.xml");
        var inputSchema = await File.ReadAllTextAsync("dsc.xml");
        var param = await File.ReadAllTextAsync("param.xml");
        var result = new byte[10 * 1024 * 1024];

        uint returnBytes = 0;

        var resultString = discountingCalculator.CalculateProcess(
            inputXml, 
            limitationXml, 
            inputSchema, 
            param, 
            result, 
            result.Length, 
            ref returnBytes);

        Console.WriteLine(resultString);
    }
}