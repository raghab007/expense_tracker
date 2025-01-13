using System.Text.Json;
using DotnetCoursework.Model;

namespace ExpenseTrackerApp.Services
{
    public class UserService
    {
        private readonly string _expensesFilePath;
        private List<User> _users;
        private readonly UserContext _userContext;

        public UserService(UserContext userContext)
        {
            string userHomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            _expensesFilePath = Path.Combine(userHomeDirectory, "expenses.json");
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
                _users = JsonSerializer.Deserialize<List<User>>(jsonData) ?? new List<User>();
            }
            else
            {
                _users = new List<User>();
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

        // Add a new expense for the currently logged-in user
        public void AddExpenseForUser(Expense expense)
        {
            var user = _userContext.CurrentUser;

            if (user == null)
            {
                throw new Exception("No user is logged in");
            }

            // Assign a unique ID to the expense
            expense.Id = user.Expenses.Any() ? user.Expenses.Max(e => e.Id) + 1 : 1;

            user.Expenses.Add(expense);
            SaveUsersToJson(); // Persist changes
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
    }
}
