using CloudApi.Entity;

public interface ICustomerService
{
    Task<Customer> CreateAsync(Customer customer);
    Task<Customer?> GetByIdAsync(int id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> UpdateAsync(Customer customer);
    Task<bool> DeleteAsync(int id);
}