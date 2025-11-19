using CloudApi.Entity;

public interface IAccountService
{
    Task<Account> CreateAsync(Account account);
    Task<Account?> GetByIdAsync(int id);
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account?> UpdateAsync(Account account);
    Task<bool> DeleteAsync(int id);
}