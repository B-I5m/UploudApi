public class CustomerCreateDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public IFormFile? Avatar { get; set; }
}