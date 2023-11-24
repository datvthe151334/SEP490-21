using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class WorkExperienceDTO
    {
        public int WorkExperienceId { get; set; }
        public int? ProfileId { get; set; }
        public string? JobTitle { get; set; }
        public string? Company { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Responsibilities { get; set; }
        public string? Achievements { get; set; }
        public string? SkillsUsed { get; set; }
        public string? EmploymentType { get; set; }

    }
   
}
