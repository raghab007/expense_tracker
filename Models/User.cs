
namespace DotnetCourseowork.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Expense> Expenses { get; set; } = new List<Expense>();

    public List<string> Tags { get; set; } = new List<string>();
    public int TotalBalance { get; set; }
    
    public List<Debt> Debts { get; set; } = new List<Debt>();
}