using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/emailtemplates")]
    [ApiController]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public EmailTemplateController(IEmailTemplateRepository emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        // Define your controller actions here.
        // For example, you can define actions to get all email templates, get an email template by ID, create, update, and delete email templates.

        [HttpGet("GetList")]
        public async Task<IActionResult> GetAllEmailTemplates([FromQuery] EmailTemplateRequest request)
        {
            var emailTemplates = await _emailTemplateRepository.GetAllEmailTemplates(request);
            return Ok(emailTemplates);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetEmailTemplateById(int templateId)
        {
            var emailTemplate = await _emailTemplateRepository.GetEmailTemplateById(templateId);
            if (emailTemplate == null)
            {
                return NotFound();
            }
            return Ok(emailTemplate);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateEmailTemplate([FromBody] EmailTemplate emailTemplate)
        {
            await _emailTemplateRepository.CreateEmailTemplate(emailTemplate);
            return CreatedAtAction("GetEmailTemplateById", new { templateId = emailTemplate.TemplateId }, emailTemplate);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateEmailTemplate(int templateId, [FromBody] EmailTemplate emailTemplate)
        {
            if (templateId != emailTemplate.TemplateId)
            {
                return BadRequest();
            }

            await _emailTemplateRepository.UpdateEmailTemplate(emailTemplate);
            return NoContent();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteEmailTemplate(int templateId)
        {
            var emailTemplate = await _emailTemplateRepository.GetEmailTemplateById(templateId);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            await _emailTemplateRepository.DeleteEmailTemplate(templateId);
            return NoContent();
        }


        [HttpPost("Refuse")]
        public async Task<IActionResult> SendEmailRefuse(int jobApplicationID, string reason)
        {
            await _emailTemplateRepository.SendEmailRefuse(jobApplicationID, reason);
            return Ok("Email sent successfully");
        }
        [HttpPost("SubmitJob")]
        public async Task<IActionResult> SendEmailSubmitJob(int jobApplicationID)
        {
            await _emailTemplateRepository.SendEmailSubmitJob(jobApplicationID);
            return Ok("Email sent successfully");
        }

        [HttpPost("ExamSchedule")]
        public async Task<IActionResult> SendEmailExamSchedule(int jobApplicationID, string time, string location)
        {
            await _emailTemplateRepository.SendEmailExamSchedule(jobApplicationID, time, location);
            return Ok("Email sent successfully");
        }
        [HttpPost("Interview")]
        public async Task<IActionResult> SendEmailInterview(int jobApplicationID, string time, string location, string? interviewer)
        {
            await _emailTemplateRepository.SendEmailInterviewSchedule(jobApplicationID, time, location, interviewer);
            return Ok("Email sent successfully");
        }

        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> SendEmailUpdateProfile(int jobApplicationID)
        {
            await _emailTemplateRepository.SendEmailUpdateProfile(jobApplicationID);
            return Ok("Email sent successfully");
        }

        [HttpPost("Accepted")]
        public async Task<IActionResult> SendEmailAccepted(int jobApplicationID)
        {
            await _emailTemplateRepository.SendEmailAccepted(jobApplicationID);
            return Ok("Email sent successfully");
        }

        [HttpPost("Canceled")]
        public async Task<IActionResult> SendEmailCanceled(int jobApplicationID)
        {
            await _emailTemplateRepository.SendEmailCanceled(jobApplicationID);
            return Ok("Email sent successfully");
        }
    }
}
