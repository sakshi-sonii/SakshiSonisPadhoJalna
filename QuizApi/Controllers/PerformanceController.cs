using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApi.Data;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceController : ControllerBase
    {
        private readonly JsonDataContext _dataContext;

        public PerformanceController(JsonDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("teacher")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetTeacherPerformance()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var tests = _dataContext.GetTests().Where(t => t.TeacherId == userId).ToList();
            var testIds = tests.Select(t => t.Id).ToList();
            var results = _dataContext.GetResults().Where(r => testIds.Contains(r.TestId)).ToList();

            var performance = new
            {
                AverageScore = results.Any() ? results.Average(r => r.Score) : 0,
                TotalStudents = results.Select(r => r.StudentId).Distinct().Count()
            };

            return Ok(performance);
        }
    }
}