namespace ApkSender;

public class UploadApkRequest
{
    public long TerminalId { get; set; }
    
    public string Version { get; set; } = null!;
    
    public PosType PosType { get; set; }
}