namespace DotnetCoursework.Model
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime?Date { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string ExpenseTag { get; set; }
        
        public int UserId { get; set; }
    }
}