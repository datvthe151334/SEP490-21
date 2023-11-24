
using Newtonsoft.Json;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class JobApplicationDTO
    {
        public int ApplicationId { get; set; }
        public int? JobId { get; set; }
        public int? ProfileId { get; set; }
        public int? TemplateId { get; set; }
        public int? Status { get; set; }
        public int? AssignedFor { get; set; }

        public virtual JobDTO? Job { get; set; }

        public virtual LoggingDTO? Profile { get; set; }

        public virtual CvtemplateDTO? Template { get; set; }

        public virtual ICollection<GeneralTestDetail> GeneralTestDetails { get; set; }
        public virtual GeneralTestDTO? GeneralTest { get; set; }
        public virtual ScheduleDTO? Schedule { get; set; }
        public virtual ScheduleDetailDTO? ScheduleDetail { get; set; }
        public virtual EvaluateDTO? Evaluate { get; set; }

        public virtual ExamDTO? Exam { get; set; }
        public virtual AssignedProfileDTO? AssignedForInfor { get; set; }


    }
    public partial class ScheduleDetailDTO
    {
        public int ScheduleDetailId { get; set; }
        public int? ScheduleId { get; set; }
        public int? CandidateId { get; set; }
        public int? ScheduleType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int? Status { get; set; }

    }
    public partial class LoggingDTO
    {
        public int? AccountId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

    }
    public partial class CvtemplateDTO
    {
        public int TemplateId { get; set; }
        public string? Url { get; set; }

    }
    public partial class AssignedProfileDTO
    {
        public int accountId { get; set; }
        public string? Name { get; set; }

    }
    public partial class JobDTO{
    public int JobId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? Industry { get; set; }
    public string? Requirements { get; set; }
    public long? MinSalary { get; set; }
    public long? MaxSalary { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
    public DateTime? DatePosted { get; set; }
    public string? ContactPerson { get; set; }
    public string? ApplicationInstructions { get; set; }
    public int? Status { get; set; }
    public bool IsPreferred { get; set; }
    }
}
