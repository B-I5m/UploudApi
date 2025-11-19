using Microsoft.AspNetCore.Mvc;
using Infrastructure.Database;
using CloudApi.Entity;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly DbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public CustomersController(ICustomerService customerService, DbContext dbContext, IWebHostEnvironment env)
        {
            _customerService = customerService;
            _dbContext = dbContext;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Customer customer)
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    var uploads = Path.Combine(_env.ContentRootPath, "Uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var path = Path.Combine(uploads, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    customer.ProfilePicturePath = $"/Uploads/{fileName}";
                }

                var created = await _customerService.CreateAsync(customer);
                return CreatedAtAction("GetById", new { id = created.Id }, created);
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
                var list = await _customerService.GetAllAsync();
                return Ok(list);
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
                if (customer == null) return NotFound();
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Customer customer)
        {
            try
            {
                var existing = await _customerService.GetByIdAsync(id);
                if (existing == null) return NotFound();

                existing.Name = customer.Name;
                existing.Email = customer.Email;
                existing.Phone = customer.Phone;
                existing.Address = customer.Address;

                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    var uploads = Path.Combine(_env.ContentRootPath, "Uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var path = Path.Combine(uploads, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    if (!string.IsNullOrEmpty(existing.ProfilePicturePath))
                    {
                        var oldPath = Path.Combine(_env.ContentRootPath, existing.ProfilePicturePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                    existing.ProfilePicturePath = $"/Uploads/{fileName}";
                }

                var updated = await _customerService.UpdateAsync(existing);
                return Ok(updated);
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
                var existing = await _customerService.GetByIdAsync(id);
                if (existing == null) return NotFound();
                if (!string.IsNullOrEmpty(existing.ProfilePicturePath))
                {
                    var path = Path.Combine(_env.ContentRootPath, existing.ProfilePicturePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                var deleted = await _customerService.DeleteAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
