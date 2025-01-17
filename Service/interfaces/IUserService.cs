
using DotnetCourseowork.Models;

namespace DotnetCourseowork.Service
{
    public interface IUserService
    {
        User? Login(string username, string password, string currency);
        User? GetCurrentUser();
        void SaveUsersToJson();
        
        List<Expense> GetUserExpenses();
        bool AddExpenseForUser(Expense expense);
        List<Expense> GetExpensesByTag(string tag);
        bool DeleteExpense(int expenseId);
        bool UpdateExpenseNote(int expenseId, string newNote);
        double GetTotalInflow();
        double GetTotalOutflow();
        double GetTotalExpenses();
        int GetTotalNumberOfInflows();
        int GetTotalNumberOfOutflows();
        List<Expense> GetRecentTransactions(int count);
        Dictionary<string, double> GetExpensesGroupedByTag();

        List<Debt> GetAllDebtsForUser();
        bool AddDebtForUser(Debt debt);
        bool MarkDebtAsPaid(int debtId);
        bool DeleteDebt(int debtId);
        double GetTotalDebts();

        List<string> GetAllTags();
        bool AddTagToUser(string tag);

        string GetCurrency();
        void SetCurrency(string currency);
    }
}
