using System.Collections.Generic;
using System.Text.Json;
using DotnetCourseowork.Models;

namespace DotnetCourseowork.Service
{
    public class UserService
    {
        private readonly string _expensesFilePath;
        private List<User> _users;
        private readonly UserContext _userContext;

        private string seedUserName = "raghab";
        private string seedPassword = "raghab";

        public UserService(UserContext userContext)
        {
            string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _expensesFilePath = Path.Combine(userHomeDirectory, "app.json");
            _userContext = userContext;
            LoadUsersFromJson();
        }

        private void EnsureExpensesFileExists()
        {
            if (!File.Exists(_expensesFilePath))
            {
                File.WriteAllText(_expensesFilePath, "[]");
            }
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
                User _user = new User();
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
        public User? Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                _userContext.CurrentUser = user;
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
                    // Check if the total balance is sufficient to deduct the expense
                    if (user.TotalBalance < expense.Amount)
                    {
                        return false; // Insufficient balance, return false
                    }
                    user.TotalBalance -= expense.Amount; // Deduct the amount
                    break;

                case "Credit":
                case "Debt":
                    user.TotalBalance += expense.Amount; // Add the amount
                    break;

                default:
                    throw new ArgumentException("Invalid expense type");
            }

            // Assign a unique ID to the expense
            expense.Id = user.Expenses.Any() ? user.Expenses.Max(e => e.Id) + 1 : 1;

            user.Expenses.Add(expense); // Add the expense to the user's list
            SaveUsersToJson(); // Persist changes

            return true; // Expense added successfully, return true
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

        // Add a new user (e.g., during registration)
        public void AddUser(User newUser)
        {
            if (_users.Any(u => u.Username == newUser.Username))
            {
                throw new Exception("User already exists");
            }

            newUser.Expenses = new List<Expense>();
            _users.Add(newUser);
            SaveUsersToJson();
        }

        // Get all users (if needed)
        public List<User> GetAllUsers()
        {
            return _users;
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
                return false; // Tag already exists
            }

            user.Tags.Add(tag);
            SaveUsersToJson(); // Persist the updated tags
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
                // Check for sufficient balance before adding the debt
                if (user.TotalBalance < debt.Amount)
                {
                    throw new Exception("Insufficient balance to add debt");
                }
                // Assign a unique ID to the debt using Guid for uniqueness across sessions
                //debt.Id = user.Debts.Any() ? user.Debts.Max(d => d.Id) + 1 : 1;
                // Add the debt to the user's list
                List<Debt> debts = user.Debts;
                Console.WriteLine(user.Debts);
                user.Debts.Add(debt);

                // Update the user's balance based on the debt amount
                user.TotalBalance += debt.Amount;

                // Persist the changes to the data storage
                SaveUsersToJson(); // Ensure this persists data correctly

                return true; // Debt added successfully
            }
            catch (Exception e)
            {
                // Log error for debugging and troubleshooting
                // Ideally, use a logger here instead of Console.WriteLine
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
            

            // Mark the debt as paid
            debt.Paid = true;

            // Deduct the debt amount from the user's balance after clearing the debt
            user.TotalBalance -= debt.Amount;

            // Persist changes to the JSON file
            SaveUsersToJson(); // Save the updated list of debts and user balance to JSON

            return true; // Debt successfully marked as paid
        }
        
        // Method to calculate total inflow (credit)
        public double GetTotalInflow()
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Expenses.Where(e => e.ExpenseType == "Credit").Sum(e => e.Amount);
        }

        // Method to calculate total outflow (debit)
        public double GetTotalOutflow()
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            return user.Expenses.Where(e => e.ExpenseType == "Debit").Sum(e => e.Amount);
        }

        public int GetRemaingBalance()

        {
            return _userContext.CurrentUser.TotalBalance;
        }
        
        
        // Method to get the total number of inflows (credits)
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

            return user.Expenses.Sum(e => e.Amount); // Sum of both credits and debits
        }

        // Method to get recent transactions for the currently logged-in user
        public List<Expense> GetRecentTransactions(int count)
        {
            var user = _userContext.CurrentUser;
            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            // Order expenses by date (assuming `Date` is a DateTime property in the Expense class) and take the latest ones
            return user.Expenses
                .OrderByDescending(e => e.Date)
                .Take(count)
                .ToList();
        }
        
        
        // Method to delete an expense by ID
       

// Method to delete a debt by ID
      
        
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

            // Update the user's balance based on the type of expense being removed
            if (expense.ExpenseType == "Debit")
            {
                user.TotalBalance += expense.Amount; // Refund the deducted amount
            }
            else if (expense.ExpenseType == "Credit")
            {
                user.TotalBalance -= expense.Amount; // Deduct the credited amount
            }

            user.Expenses.Remove(expense); // Remove the expense
            SaveUsersToJson(); // Persist the updated data

            return true; // Return true if the expense was successfully deleted
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

            // Only unpaid debts can be deleted
            if (debt.Paid)
            {
                throw new Exception("Cannot delete a debt that has already been paid");
            }

            // Adjust the user's balance if the debt is being removed
            user.TotalBalance += debt.Amount;

            user.Debts.Remove(debt); // Remove the debt
            SaveUsersToJson(); // Persist the updated data

            return true; // Return true if the debt was successfully deleted
        }


        public bool AddCustomSource(string source)
        {
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
        


    }
}
