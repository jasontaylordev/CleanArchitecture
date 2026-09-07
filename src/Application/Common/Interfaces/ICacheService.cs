namespace CleanArchitecture.Application.Common.Interfaces;

public interface ICacheService
{
    Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> createItem, TimeSpan? absoluteExpirationRelativeToNow = null);
    void Remove(string key);
}
