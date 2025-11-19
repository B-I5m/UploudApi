using Microsoft.AspNetCore.Mvc;
using Infrastructure.Database;
using CloudApi.Entity;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IWebHostEnvironment _env;

    public CustomersController(ICustomerService customerService, DbContext dbContext, IWebHostEnvironment env)
    {
        _customerService = customerService;
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Customer customer)
    {
        try
        {
            if (customer.Avatar is not null && customer.Avatar.Length > 0)
            {
                var uploads = Path.Combine(_env.ContentRootPath, "Uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(customer.Avatar.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                    await customer.Avatar.CopyToAsync(fs);

                customer.AvatarPath = $"/Uploads/{fileName}";
            }

            var created = await _customerService.CreateAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _customerService.GetAllAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            return customer == null ? NotFound() : Ok(customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] Customer data)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            customer.Name = data.Name;
            customer.Email = data.Email;
            customer.Phone = data.Phone;
            customer.Address = data.Address;

            if (data.Avatar is not null)
            {
                var uploadFolder = Path.Combine(_env.ContentRootPath, "Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var newName = $"{Guid.NewGuid()}{Path.GetExtension(data.Avatar.FileName)}";
                var newPath = Path.Combine(uploadFolder, newName);

                using (var fs = new FileStream(newPath, FileMode.Create))
                    await data.Avatar.CopyToAsync(fs);

                if (!string.IsNullOrWhiteSpace(customer.AvatarPath))
                {
                    var oldPath = Path.Combine(
                        _env.ContentRootPath,
                        customer.AvatarPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                    );

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                customer.AvatarPath = $"/Uploads/{newName}";
            }

            return Ok(await _customerService.UpdateAsync(customer));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(customer.AvatarPath))
            {
                var oldPath = Path.Combine(
                    _env.ContentRootPath,
                    customer.AvatarPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
                );

                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            var deleted = await _customerService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
