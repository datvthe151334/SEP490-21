using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Configuration;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;
using System.Security.Cryptography;
using RecruitXpress_BE.Helper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata;
using Constant = RecruitXpress_BE.Helper.Constant;
using RecruitXpress_BE.IRepositories;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Question/")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly RecruitXpressContext _context;
        public IQuestionRepository _repository;
        public QuestionController(RecruitXpressContext context, IConfiguration configuration, IQuestionRepository repository)
        {
            _context = context;
            _configuration = configuration;
            _repository = repository;
        }
        [HttpGet("GetLevelOfTests")]
        public async Task<IActionResult> SetUpLevelOfTests()
        {
            try
            {
              
                return Ok( new
                {
                    easy = ConstantQuestion.easy,
                    medium = ConstantQuestion.medium,
                    hard = ConstantQuestion.hard,
                    totalQuestion = ConstantQuestion.easy + ConstantQuestion.medium + ConstantQuestion.hard,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("SetLevelOfTests")]
        public async Task<IActionResult> SetUpLevelOfTests(int easy, int medium, int hard)
        {
            try
            {
                await _repository.SetUpLevelOfGeneralTests(easy,medium,hard);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GenerateATest")]
        public async Task<IActionResult> StartATest()
        {
            try
            {
                var generalTests = await _repository.GenerateATest();
                return Ok(generalTests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // GET: api/questions/{id}
        [HttpGet("ListQuestion")]
        public async Task<IActionResult> GetQuestion([FromQuery] QuestionRequest request)
        {
            var question = await _repository.GetAllQuestions(request);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }
        // GET: api/questions/{id}
        [HttpGet("GetQuestionById/{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _repository.GetQuestionById(id);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }

        // POST: api/questions
        [HttpPost("CreateQuestion")]
        public async Task<IActionResult> CreateQuestion(Question question)
        {
            await _repository.CreateQuestion(question);

            return Ok();
        }

        // PUT: api/questions/{id}
        [HttpPut("UpdateQuestion/{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, Question question)
        {
            if (id != question.QuestionId)
            {
                return BadRequest();
            }

            var updatedQuestion = await _repository.UpdateQuestion(question);

            if (updatedQuestion == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/questions/{id}
        [HttpDelete("DeleteQuestion/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var deleted = await _repository.DeleteQuestion(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("DeleteOption/{id}")]
        public async Task<IActionResult> DeleteOption(int id)
        {
            var deleted = await _repository.DeleteOption(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}