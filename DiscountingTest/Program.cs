using DiscountingTest;

class MyClass
{
    public static async Task Main()
    {
        using var discountingCalculator = new DiscountingCalculator();

        var inputXml = await File.ReadAllTextAsync("in.xml");
        var limitationXml = await File.ReadAllTextAsync("limit.xml");
        var inputSchema = await File.ReadAllTextAsync("dsc.xml");
        var result = new byte[10 * 1024 * 1024];

        uint returnBytes = 0;

        var resultString = discountingCalculator.CalculateProcess(
            inputXml, 
            limitationXml, 
            inputSchema, 
            "", 
            result, 
            result.Length, 
            ref returnBytes);

        Console.WriteLine(resultString);
    }
}
    
    
    
    