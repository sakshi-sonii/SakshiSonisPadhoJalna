using System.Text.Json;
using QuizApi.Models;

namespace QuizApi.Data
{
    public class JsonDataContext
    {
        private readonly string _usersFile = "Data/users.json";
        private readonly string _testsFile = "Data/tests.json";
        private readonly string _resultsFile = "Data/results.json";

        public JsonDataContext()
        {
            // Ensure data directory exists
            Directory.CreateDirectory("Data");

            // Initialize files if they don't exist
            if (!File.Exists(_usersFile)) File.WriteAllText(_usersFile, "[]");
            if (!File.Exists(_testsFile)) File.WriteAllText(_testsFile, "[]");
            if (!File.Exists(_resultsFile)) File.WriteAllText(_resultsFile, "[]");
        }

        private List<T> Read<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        private void Write<T>(string filePath, List<T> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        // Users
        public List<User> GetUsers() => Read<User>(_usersFile);
        public void SaveUsers(List<User> users) => Write(_usersFile, users);

        // Tests
        public List<Test> GetTests() => Read<Test>(_testsFile);
        public void SaveTests(List<Test> tests) => Write(_testsFile, tests);

        // Results
        public List<Result> GetResults() => Read<Result>(_resultsFile);
        public void SaveResults(List<Result> results) => Write(_resultsFile, results);
    }
}