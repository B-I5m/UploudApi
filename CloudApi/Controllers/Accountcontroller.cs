using CloudApi.Entity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountsController(IAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> FetchAll()
    {
        try
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> FetchOne(int id)
    {
        try
        {
            var acc = await _service.GetByIdAsync(id);
            return acc == null ? NotFound() : Ok(acc);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Account req)
    {
        try
        {
            var created = await _service.CreateAsync(req);
            return CreatedAtAction(nameof(FetchOne), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] Account data)
    {
        try
        {
            var acc = await _service.GetByIdAsync(id);
            if (acc == null) return NotFound();

            acc.Balance = data.Balance;
            acc.AccountType = data.AccountType;
            acc.CustomerId = data.CustomerId;

            var updated = await _service.UpdateAsync(acc);
            return Ok(updated);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
}
