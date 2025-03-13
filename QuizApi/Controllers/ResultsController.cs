using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApi.Data;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly JsonDataContext _dataContext;

        public ResultsController(JsonDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student")]
        public IActionResult GetResult(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = _dataContext.GetResults().FirstOrDefault(r => r.Id == id && r.StudentId == userId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}