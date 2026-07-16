using System.Net.Http.Headers;

const string filePath = "C:\\Users\\VPC\\AppData\\Local\\Xamarin\\Mono for Android\\Archives\\2026-07-16\\Terminal.Android1\\com.snc.terminal-Signed.apk";
const long terminalId = 777;

using var client = new HttpClient();
client.BaseAddress = new Uri("http://127.0.0.1:5297");

var fileBytes = await File.ReadAllBytesAsync(filePath);
var fileName = Path.GetFileName(filePath);

using var formData = new MultipartFormDataContent();

formData.Add(new StringContent(terminalId.ToString()), "TerminalKey");
formData.Add(new StringContent("0.1.2"), "Version");
formData.Add(new StringContent("1"), "PosType");

var fileContent = new ByteArrayContent(fileBytes);
fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.android.package-archive");
formData.Add(fileContent, "file", fileName);

var url = "terminal-updating/upload_apk";
var response = await client.PostAsync(url, formData);

var responseContent = await response.Content.ReadAsStringAsync();
Console.WriteLine($"Status: {response.StatusCode}");
Console.WriteLine($"Response: {responseContent}");