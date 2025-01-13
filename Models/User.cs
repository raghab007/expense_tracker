using MudBlazor;

namespace DotnetCoursework.Model;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Expense> Expenses { get; set; }
    
    public List<string> Tags { get; set; }
    
    public int TotalBalance { get; set; }
}