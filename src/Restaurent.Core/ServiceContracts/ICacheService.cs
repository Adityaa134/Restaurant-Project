namespace Restaurent.Core.ServiceContracts
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? baseExpiry = null);
        Task RemoveAsync(string key);
    }
}
