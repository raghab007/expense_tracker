namespace DotnetCourseowork.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime?Date { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string ExpenseTag { get; set; }

        public string ExpenseType { get; set; }
    }
}