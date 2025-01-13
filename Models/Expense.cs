namespace DotnetCoursework.Model
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime?Date { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public String ExpenseTag { get; set; }
        
        public int UserId { get; set; }
    }
}