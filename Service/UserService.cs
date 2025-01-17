using System.Text.Json;
using DotnetCourseowork.enums;
using DotnetCourseowork.Models;

namespace DotnetCourseowork.Service
{
    public class UserService :IUserService
    {
        private readonly string _expensesFilePath;
        private List<User> _users;
        private readonly UserContext _userContext;

        private string seedUserName = "raghab";
        private string seedPassword = "raghab";

        public UserService(UserContext userContext)
        {
            string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _expensesFilePath = Path.Combine(userHomeDirectory, "pocketplanner.json");
            _userContext = userContext;
            LoadUsersFromJson();
        }
        
        // Load users and expenses from the JSON file
        private void LoadUsersFromJson()
        {
            if (File.Exists(_expensesFilePath))
            {
                var jsonData = File.ReadAllText(_expensesFilePath);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    _users = JsonSerializer.Deserialize<List<User>>(jsonData) ?? new List<User>();
                }
            }
            else
            {
                List<Tags> tagList = new List<Tags>((Tags[])Enum.GetValues(typeof(Tags)));
                List<string> tags = tagList.Select(t => t.ToString()).ToList();
                User _user = new User();
                _user.Tags = tags;
                _user.Username = seedUserName;
                _user.Password = seedPassword;
                _users = new List<User>();
                _users.Add(_user);
                SaveUsersToJson(); // Create the file if it doesn't exist
            }
        }

        // Save users and expenses back to the JSON file
        public void SaveUsersToJson()
        {
            var jsonData = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_expensesFilePath, jsonData);
        }

        // Authenticate the user and return user details if login is successful
        public User? Login(string username, string password,string currency)
        {
            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                _userContext.CurrentUser = user;
                SetCurrency(currency);
            }

            return user; // Return the user if authentication is successful, otherwise null
        }

        // Get expenses for the currently logged-in user
        public List<Expense> GetUserExpenses()
        {
            var user = _userContext.CurrentUser;
            return user?.Expenses ?? new List<Expense>();
        }

        // Add a new expense for the currently logged-in user and return true/false
        public bool AddExpenseForUser(Expense expense)
        {
            var user = _userContext.CurrentUser;

            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            switch (expense.ExpenseType)
            {
                case "Debit":
                    if (user.TotalBalance < expense.Amount)
                    {
                        return false; 
                    }
                    user.TotalBalance -= expense.Amount; 
                    break;

                case "Credit":
                case "Debt":
                    user.TotalBalance += expense.Amount; 
                    break;

                default:
                    throw new ArgumentException("Invalid expense type");
            }

            expense.Id = user.Expenses.Any() ? user.Expenses.Max(e => e.Id) + 1 : 1;

            user.Expenses.Add(expense); 
            SaveUsersToJson(); 

            return true; 
        }

        // Get expenses by tag for the currently logged-in user
        public List<Expense> GetExpensesByTag(string tag)
        {
            var user = _userContext.CurrentUser;
            return user?.Expenses.Where(e => e.ExpenseTag == tag).ToList() ?? new List<Expense>();
        }

        public List<string> GetAllTags()
        {
            var user = _userContext.CurrentUser;
            List<string> list = user.Tags.ToList();
            return list;
        }

        public User? GetCurrentUser()
        {
            return _userContext.CurrentUser;
        }

      
        

        public bool AddTagToUser(string tag)
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            if (user.Tags.Contains(tag))
            {
                return false; 
            }

            user.Tags.Add(tag);
            SaveUsersToJson(); 
            return true;
        }

        public List<Debt> GetAllDebtsForUser()
        {
            var user = _userContext.CurrentUser;
            return user?.Debts ?? new List<Debt>();
        }

        public bool AddDebtForUser(Debt debt)
        {
            var user = _userContext.CurrentUser;

            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            try
            {
                if (user.TotalBalance < debt.Amount)
                {
                    throw new Exception("Insufficient balance to add debt");
                }

                if (user.Debts == null)
                {
                    user.Debts = new List<Debt>();
                }

                int newDebtId = user.Debts.Count > 0 ? user.Debts.Max(d => d.Id) + 1 : 1;
                debt.Id = newDebtId;

                user.Debts.Add(debt);

                user.TotalBalance += debt.Amount;

                SaveUsersToJson();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error adding debt: {e.Message}");
                return false;
            }
        }


        public bool MarkDebtAsPaid(int debtId)
        {
            var user = _userContext.CurrentUser;

            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            var debt = user.Debts.FirstOrDefault(d => d.Id == debtId);
            if (debt == null)
            {
                throw new Exception("Debt not found");
            }
            

            debt.Paid = true;

            user.TotalBalance -= debt.Amount;

            SaveUsersToJson(); // Save the updated list of debts and user balance to JSON

            return true; // Debt successfully marked as paid
        }
        
        public double GetTotalInflow()
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Expenses.Where(e => e.ExpenseType == "Credit").Sum(e => e.Amount);
        }

        public double GetTotalOutflow()
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Expenses.Where(e => e.ExpenseType == "Debit").Sum(e => e.Amount);
        }

      
        
        public int GetTotalNumberOfInflows()
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Expenses.Count(e => e.ExpenseType == "Credit");
        }

// Method to get the total number of outflows (debits)
        public int GetTotalNumberOfOutflows()
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Expenses.Count(e => e.ExpenseType == "Debit");
        }

// Method to calculate the total expenses (inflows + outflows)
        public double GetTotalExpenses()
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Expenses.Sum(e => e.Amount); 
        }

        // Method to get recent transactions for the currently logged-in user
        public List<Expense> GetRecentTransactions(int count)
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Expenses
                .OrderByDescending(e => e.Date)
                .Take(count)
                .ToList();
        }
        
        
       

      
        
        // Method to delete an expense by ID
        public bool DeleteExpense(int expenseId)
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            var expense = user.Expenses.FirstOrDefault(e => e.Id == expenseId);
            if (expense == null)
            {
                throw new Exception("Expense not found");
            }

            if (expense.ExpenseType == "Debit")
            {
                user.TotalBalance += expense.Amount; 
            }
            else if (expense.ExpenseType == "Credit")
            {
                user.TotalBalance -= expense.Amount; 
            }

            user.Expenses.Remove(expense); 
            SaveUsersToJson(); 

            return true; 
        }

// Method to delete a debt by ID
        public bool DeleteDebt(int debtId)
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            var debt = user.Debts.FirstOrDefault(d => d.Id == debtId);
            if (debt == null)
            {
                throw new Exception("Debt not found");
            }

            if (debt.Paid)
            {
                throw new Exception("Cannot delete a debt that has already been paid");
            }

            user.TotalBalance += debt.Amount;

            user.Debts.Remove(debt);
            SaveUsersToJson(); 

            return true; 
        }

        
        public double GetTotalDebts()
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Debts.Where(d => !d.Paid).Sum(d => d.Amount);
        }

        public string GetCurrency()
        {
           return _userContext.Currency;
        }

        public void SetCurrency(string currency)
        {
            
            _userContext.Currency = currency;
        }
        
        public bool UpdateExpenseNote(int expenseId, string newNote)
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            var expense = user.Expenses.FirstOrDefault(e => e.Id == expenseId);
            if (expense == null)
            {
                throw new Exception("Expense not found");
            }

            expense.Note = newNote;

            SaveUsersToJson();

            return true; 
        }
        
        
        public Dictionary<string, double> GetExpensesGroupedByTag()
        {
            var user = _userContext.CurrentUser;

            if (user == null)
            {
                throw new Exception("No user is logged in");
            }
            
            var expensesGroupedByTag = user.Expenses
                .GroupBy(e => e.ExpenseTag) 
                .ToDictionary(
                    g => g.Key, 
                    g => g.Sum(e =>(double) e.Amount) 
                );

            return expensesGroupedByTag; 
        }
        



    }
}
