namespace Domain.Interfaces;

public interface ISmsSender
{
    public Task SendSmsAsync(string number, string message);
}