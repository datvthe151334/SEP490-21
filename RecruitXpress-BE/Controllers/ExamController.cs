using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.ComponentModel.DataAnnotations;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Exam")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamRepository _examRepository;

        public ExamController(IExamRepository examRepository)
        {
            _examRepository = examRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExams([FromQuery] ExamRequest request)
        {
            try
            {
                var exams = await _examRepository.GetAllExams(request);
                return Ok(exams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetExamBySpecializedExamId/{specializedId}")]
        public async Task<IActionResult> GetAllExams([FromQuery] ExamRequest request,int specializedId)
        {
            try
            {
                var exams = await _examRepository.GetListExamWithSpecializedExamId(request, specializedId);
                return Ok(exams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetExamBySpecializedExamCode/{code}")]
        public async Task<IActionResult> GetAllExams([FromQuery] ExamRequest request, string code,[Required] string expertEmail)
        {
            try
            {
                var exams = await _examRepository.GetListExamWithSpecializedExamCode(request, code, expertEmail);
                return Ok(exams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{examId}")]
        public async Task<IActionResult> GetExamById(int examId)
        {
            try
            {
                var exam = await _examRepository.GetExamById(examId);
                if (exam == null)
                {
                    return NotFound();
                }
                return Ok(exam);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("CreateExam")]
        public async Task<IActionResult> CreateExam([FromForm] ExamWithFileRequest examRequest)
        {
            try
            {
                var exam = examRequest.Exam;
                var fileData = examRequest.FileData;

                var createdExam = await _examRepository.CreateExamWithFile(exam, fileData);

                return CreatedAtAction(nameof(GetExamById), new { examId = createdExam.ExamId }, createdExam);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("DownloadExam/{examId}")]
        public async Task<IActionResult> DownloadExam(int examId)
        {
            var exam = await _examRepository.GetExamById(examId);
            if (exam == null || string.IsNullOrEmpty(exam.FileUrl))
            {
                throw new Exception("Không tìm thấy id");
            }

            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\ExamFiles"));
            var filePath = Path.Combine(path, exam.FileUrl);

            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("Không tìm thấy file");
            }

            var fileContent = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            var contentType = "application/octet-stream";
            return File(fileContent, contentType, fileName);
        }
        /*[HttpGet("Download/{examId}")]
        public async Task<IActionResult> DownloadFile(int examId)
        {
            try
            {
                var exam = await _examRepository.GetExamById(examId);
             
                if (exam == null)
                {
                    return NotFound("Không tìm thấy id");
                }

                var filePath = Path.Combine(Environment.CurrentDirectory, "Upload\\ExamFiles", exam.FileUrl) ;

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Không tìm thấy file");
                }

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                return File(fileStream, "application/octet-stream", exam.FileUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }*/

        [HttpPut("UpdateExam/{examId}")]
        public async Task<IActionResult> UpdateExam(int examId, Exam exam)
        {
            try
            {
                var existingExam = await _examRepository.GetExamById(examId);
                if (existingExam == null)
                {
                    return NotFound();
                }

                if (existingExam.ExamId != examId)
                {
                    return BadRequest("ExamId in the URL does not match the one in the request body.");
                }

                await _examRepository.UpdateExam(exam);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("GradeExam")]
        public async Task<IActionResult> GradeExam(GradeExamRequest exam)
        {
            try
            {
                await _examRepository.GradeExam(exam);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("DeleteExam/{id}")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var deleted = await _examRepository.DeleteExam(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpGet("AssignExpert")]
        public async Task<IActionResult> AssignExpert(string email, string examCode)
        {
            try
            {
                await _examRepository.AssignExpertToSystem(email, examCode);
            
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class ExamWithFileRequest
{
    public ExamRequestClass Exam { get; set; } // The exam information in JSON format
    public IFormFile FileData { get; set; } // The uploaded file data
}
}
