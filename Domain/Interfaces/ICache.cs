namespace Domain.Interfaces;

public interface ICacheService
{
    public void SetString(string key, string value, TimeSpan duration);
    public string GetString(string key);
    public void Set<T>(string key, T value, TimeSpan duration);
    public T Get<T>(string key);
    public void Remove(string key);
    public void Clear();
}