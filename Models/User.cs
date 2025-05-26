namespace blogic.Models;

public class User
{
    public int UserId { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string ImageUrl { get; set; } = "/uploads/user.png";
    public decimal Credit { get; set; } = 0;
}