using Dapper;
using Infrastructure.Database;
using CloudApi.Entity;

public class AccountService : IAccountService
{
    private readonly DbContext _context;
    public AccountService(DbContext context) => _context = context;


    public async Task<Account> CreateAsync(Account account)
    {
        using var conn = _context.CreateConnection();
        var customer = await conn.QueryFirstOrDefaultAsync<int?>("SELECT id FROM customers WHERE id = @Id",
            new { Id = account.CustomerId });
        if (customer == null) throw new Exception("Customer not found");
        var sql =
            "INSERT INTO accounts (account_number, account_type, balance, customer_id) VALUES (@AccountNumber, @AccountType, @Balance, @CustomerId) RETURNING id";
        var id = await conn.QuerySingleAsync<int>(sql, account);
        account.Id = id;
        return account;
    }


    public async Task<Account?> GetByIdAsync(int id)
    {
        using var conn = _context.CreateConnection();
        var sql = "SELECT * FROM accounts WHERE id = @Id";
        return await conn.QueryFirstOrDefaultAsync<Account>(sql, new { Id = id });
    }


    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        using var conn = _context.CreateConnection();
        var sql = "SELECT * FROM accounts ORDER BY id";
        return await conn.QueryAsync<Account>(sql);
    }


    public async Task<Account?> UpdateAsync(Account account)
    {
        using var conn = _context.CreateConnection();
        var sql = "UPDATE accounts SET account_type=@AccountType, balance=@Balance WHERE id=@Id";
        var affected = await conn.ExecuteAsync(sql, account);
        if (affected == 0) return null;
        return await GetByIdAsync(account.Id);
    }


    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = _context.CreateConnection();
        var sql = "DELETE FROM accounts WHERE id = @Id";
        var affected = await conn.ExecuteAsync(sql, new { Id = id });
        return affected > 0;
    }
}