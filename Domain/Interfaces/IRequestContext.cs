namespace Domain.Interfaces;

public interface IRequestContext
{
    public string UserId { get; set; }

    public Guid GetId()
    {
        try
        {
            return Guid.Parse(UserId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
}