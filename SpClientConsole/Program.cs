using System.Text.Json;
using SpClientConsole;

var spClient = await SpClient.ConnectAsync("192.168.100.199", 8887);

var responce = await spClient.ReadPacketAsync();

Console.WriteLine(JsonSerializer.Serialize(responce, new JsonSerializerOptions{WriteIndented = true}));