namespace Domain.Dto;

public class CoreResponse(string? message)
{
    public string? Message { get; set; } = message;
}
public class ContentResponse<T> : CoreResponse
{
    public ContentResponse(T content, string? message = null) : base(message)
    {
        Content = content;
        Message = message;
    
    }
    public T Content { get; set; }
}