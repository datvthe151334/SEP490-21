using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class EducationalBackgroundDTO
    {
        public int EducationalBackgroundId { get; set; }
        public int? ProfileId { get; set; }
        public string? InstitutionName { get; set; }
        public string? DegreeEarned { get; set; }
        public string? FieldOfStudy { get; set; }
        public double? Gpa { get; set; }
        public string? EducationalLevel { get; set; }
        public string? Certifications { get; set; }
        public string? ResearchProjects { get; set; }
        public string? Awards { get; set; }
        public string? Time { get; set; }

    }
   
}
