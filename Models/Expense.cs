using System.ComponentModel.DataAnnotations;

namespace DotnetCourseowork.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime?Date { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }
        public string Description { get; set; }
        public string ExpenseTag { get; set; }

        public string ExpenseType { get; set; }
    }
}