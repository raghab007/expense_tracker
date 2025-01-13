using DotnetCoursework.Model;
using System.Text.Json;
namespace ExpenseTrackerApp.Services
{
    public class ExpenseService
    {
        private readonly string _expensesFilePath;

        public ExpenseService()
        {
            string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _expensesFilePath = Path.Combine(userHomeDirectory, "expenses.json");

            EnsureExpensesFileExists();
        }
        // Ensure the file exists with an empty array if not present
        private void EnsureExpensesFileExists()
        {
            if (!File.Exists(_expensesFilePath))
            {
                File.WriteAllText(_expensesFilePath, "[]");
            }
        }

        // Get all expenses
        public List<Expense> GetAllExpenses()
        {
            var json = File.ReadAllText(_expensesFilePath);
            return JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
        }

        // Get expenses by userId
        public List<Expense> GetExpensesByUserId(int userId)
        {
            var expenses = GetAllExpenses();
            return expenses.Where(e => e.UserId == userId).ToList();
        }

        // Add a new expense
        public void AddExpense(Expense expense)
        {
            var expenses = GetAllExpenses();
            expense.Id = GenerateExpenseId(expenses);
            expenses.Add(expense);

            SaveExpensesToFile(expenses);
        }

        // Update an existing expense
        public void UpdateExpense(Expense updatedExpense)
        {
            var expenses = GetAllExpenses();
            var expense = expenses.FirstOrDefault(e => e.Id == updatedExpense.Id);

            if (expense == null)
            {
                throw new Exception("Expense not found");
            }

            expense.Date = updatedExpense.Date;
            expense.Amount = updatedExpense.Amount;
            expense.Description = updatedExpense.Description;
            expense.ExpenseTag = updatedExpense.ExpenseTag;

            SaveExpensesToFile(expenses);
        }

        // Delete an expense
        public void DeleteExpense(int expenseId)
        {
            var expenses = GetAllExpenses();
            var expense = expenses.FirstOrDefault(e => e.Id == expenseId);

            if (expense == null)
            {
                throw new Exception("Expense not found");
            }

            expenses.Remove(expense);

            SaveExpensesToFile(expenses);
        }

        // Save expenses to the file
        private void SaveExpensesToFile(List<Expense> expenses)
        {
            var json = JsonSerializer.Serialize(expenses, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_expensesFilePath, json);
        }

        // Generate a new unique expenseId
        private int GenerateExpenseId(List<Expense> expenses)
        {
            return expenses.Any() ? expenses.Max(e => e.Id) + 1 : 1;
        }
    }
}
