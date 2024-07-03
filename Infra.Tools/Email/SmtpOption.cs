namespace Infra.Tools.Email;

public class SmtpOption
{
    public  const string SectionName = "Smtp";
    public required string SmtpServer { get; set; }
    public required int Port { get; set; }
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string SenderPassword { get; set; }
}