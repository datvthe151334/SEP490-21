using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface IEmailTemplateRepository
    {
        Task<ApiResponse<EmailTemplate>> GetAllEmailTemplates(EmailTemplateRequest request);

        Task<EmailTemplate> GetEmailTemplateById(int templateId);
        Task CreateEmailTemplate(EmailTemplate emailTemplate);

        Task UpdateEmailTemplate(EmailTemplate emailTemplate);

        Task DeleteEmailTemplate(int templateId);
        Task SendEmailSubmitJob(int jobApplicationID);
        Task SendEmailRefuse(int jobApplicationID, string reason);
        Task SendEmailInterviewSchedule(int jobApplicationID, string time, string location, string? interviewer);
        Task SendEmailUpdateProfile(int jobApplicationID);
        Task SendEmailExamSchedule(int jobApplicationID, string time, string location);
        Task SendEmailAccepted(int jobApplicationID);
        Task SendEmailCanceled(int jobApplicationID);

    }
}
