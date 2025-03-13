using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApi.Data;
using QuizApi.Models;
using QuizApi.Services;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly JsonDataContext _dataContext;
        private readonly ExcelParserService _excelParserService;

        public TestsController(JsonDataContext dataContext, ExcelParserService excelParserService)
        {
            _dataContext = dataContext;
            _excelParserService = excelParserService;
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult GetTests()
        {
            var tests = _dataContext.GetTests();
            return Ok(tests);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student")]
        public IActionResult GetTest(int id)
        {
            var test = _dataContext.GetTests().FirstOrDefault(t => t.Id == id);
            if (test == null)
            {
                return NotFound();
            }
            return Ok(test);
        }

        [HttpGet("teacher")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetTeacherTests()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            var userId = int.Parse(userIdClaim);
            var tests = _dataContext.GetTests().Where(t => t.TeacherId == userId).ToList();
            return Ok(tests);
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Teacher")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UploadTest([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var questions = _excelParserService.ParseExcel(file.OpenReadStream());
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            var userId = int.Parse(userIdClaim);

            var test = new Test
            {
                Id = _dataContext.GetTests().Any() ? _dataContext.GetTests().Max(t => t.Id) + 1 : 1,
                Title = file.FileName, // Use file name as test title (can be customized)
                Duration = 60, // Default duration (can be customized)
                Questions = questions,
                TeacherId = userId
            };

            var tests = _dataContext.GetTests();
            tests.Add(test);
            _dataContext.SaveTests(tests);

            return Ok(test);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public IActionResult DeleteTest(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID claim not found or invalid");
            }
            var tests = _dataContext.GetTests();
            var test = tests.FirstOrDefault(t => t.Id == id && t.TeacherId == userId);
            if (test == null)
            {
                return NotFound();
            }
            tests.Remove(test);
            _dataContext.SaveTests(tests);
            return Ok();
        }

        [HttpPost("submit")]
        [Authorize(Roles = "Student")]
        public IActionResult SubmitTest([FromBody] TestSubmission submission)
        {
            var test = _dataContext.GetTests().FirstOrDefault(t => t.Id == submission.TestId);
            if (test == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            var userId = int.Parse(userIdClaim);
            var result = new Result
            {
                Id = _dataContext.GetResults().Any() ? _dataContext.GetResults().Max(r => r.Id) + 1 : 1,
                TestId = submission.TestId,
                StudentId = userId,
                Score = 0,
                Total = test.Questions.Count,
                Questions = new List<QuestionResult>()
            };

            for (int i = 0; i < test.Questions.Count; i++)
            {
                var question = test.Questions[i];
                var selectedOption = submission.Responses[i];
                var isCorrect = selectedOption != null && selectedOption == question.CorrectOptionIndex;

                if (isCorrect)
                {
                    result.Score++;
                }

                result.Questions.Add(new QuestionResult
                {
                    QuestionId = question.Id,
                    SelectedOptionIndex = selectedOption ?? -1, // -1 for unanswered
                    IsCorrect = isCorrect
                });
            }

            var results = _dataContext.GetResults();
            results.Add(result);
            _dataContext.SaveResults(results);

            return Ok(new { id = result.Id });
        }
    }

    public class TestSubmission
    {
        public int TestId { get; set; }
        public List<int?> Responses { get; set; } = new List<int?>();
    }
}