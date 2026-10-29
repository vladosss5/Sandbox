using ConsoleApp;

var spClient = await SpUsbClient.ConnectAsync("192.168.100.199", 8887);

for (int i = 0; i <= 5; i++)
{
    var response = await spClient.ReadPacketAsync();
    Console.WriteLine(BitConverter.ToString(response));

    if (i == 4)
    {
        await spClient.DisposeAsync();
    }
}

// try
// {
//     var responce = await spClient.SendCommandAsync(0x01, 0x90, []);
//     Console.WriteLine(BitConverter.ToString(responce));
// }
// catch (Exception e)
// {
//     Console.WriteLine(e);
//     throw;
// }