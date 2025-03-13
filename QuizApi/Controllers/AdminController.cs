using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApi.Data;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly JsonDataContext _dataContext;

        public AdminController(JsonDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("teachers")]
        public IActionResult GetTeachers()
        {
            var teachers = _dataContext.GetUsers().Where(u => u.Role == "Teacher").ToList();
            return Ok(teachers);
        }

        [HttpPost("teachers")]
        public IActionResult AddTeacher([FromBody] User teacher)
        {
            var users = _dataContext.GetUsers();
            teacher.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            teacher.Role = "Teacher";
            users.Add(teacher);
            _dataContext.SaveUsers(users);
            return Ok(teacher);
        }

        [HttpDelete("teachers/{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            var users = _dataContext.GetUsers();
            var teacher = users.FirstOrDefault(u => u.Id == id && u.Role == "Teacher");
            if (teacher == null)
            {
                return NotFound();
            }
            users.Remove(teacher);
            _dataContext.SaveUsers(users);
            return Ok();
        }

        [HttpPost("tests")]
        public IActionResult CreateTest([FromBody] Test test)
        {
            var tests = _dataContext.GetTests();
            test.Id = tests.Any() ? tests.Max(t => t.Id) + 1 : 1;
            tests.Add(test);
            _dataContext.SaveTests(tests);
            return Ok(test);
        }

        [HttpGet("analytics")]
        public IActionResult GetAnalytics()
        {
            var results = _dataContext.GetResults();
            var tests = _dataContext.GetTests();
            var users = _dataContext.GetUsers();

            var analytics = new
            {
                AverageScore = results.Any() ? results.Average(r => r.Score) : 0,
                TotalTests = tests.Count,
                TotalStudents = users.Count(u => u.Role == "Student")
            };

            return Ok(analytics);
        }
    }
}