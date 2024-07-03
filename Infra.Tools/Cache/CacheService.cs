using Domain.Interfaces;

namespace Infra.Tools.Cache;

public class CacheService: ICacheService 
{
    public void SetString(string key, string value, TimeSpan duration)
    {
        
        throw new NotImplementedException();
    }

    public string GetString(string key)
    {
        throw new NotImplementedException();
    }

    public void Set<T>(string key, T value, TimeSpan duration)
    {
        throw new NotImplementedException();
    }

    public T Get<T>(string key)
    {
        throw new NotImplementedException();
    }

    public void Remove(string key)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }
}