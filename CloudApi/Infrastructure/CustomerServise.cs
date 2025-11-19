using Dapper;
using Infrastructure.Database;
using CloudApi.Entity;
public class CustomerService : ICustomerService
{
    private readonly DbContext _context;
    public CustomerService(DbContext context) => _context = context;


    public async Task<Customer> CreateAsync(Customer customer)
    {
        using var conn = _context.CreateConnection();
        var sql = "INSERT INTO customers (name, email, phone, address, profile_picture_path) VALUES (@Name, @Email, @Phone, @Address, @ProfilePicturePath) RETURNING id";
        var id = await conn.QuerySingleAsync<int>(sql, customer);
        customer.Id = id;
        return customer;
    }


    public async Task<Customer?> GetByIdAsync(int id)
    {
        using var conn = _context.CreateConnection();
        var sql = "SELECT * FROM customers WHERE id = @Id";
        return await conn.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
    }


    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        using var conn = _context.CreateConnection();
        var sql = "SELECT * FROM customers ORDER BY id";
        return await conn.QueryAsync<Customer>(sql);
    }


    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        using var conn = _context.CreateConnection();
        var sql = "UPDATE customers SET name=@Name, email=@Email, phone=@Phone, address=@Address, profile_picture_path=@ProfilePicturePath WHERE id=@Id";
        var affected = await conn.ExecuteAsync(sql, customer);
        if (affected == 0) return null;
        return await GetByIdAsync(customer.Id);
    }


    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = _context.CreateConnection();
        var sql = "DELETE FROM customers WHERE id = @Id";
        var affected = await conn.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}