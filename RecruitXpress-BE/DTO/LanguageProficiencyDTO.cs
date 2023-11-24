using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class LanguageProficiencyDTO
    {
        public int LanguageProficiencyId { get; set; }
        public int? ProfileId { get; set; }
        public string? Language { get; set; }
        public string? ProficiencyLevel { get; set; }
        public string? Certifications { get; set; }
        public string? LanguageExperiences { get; set; }
        public string? TestScores { get; set; }
        public string? Notes { get; set; }

    }
   
}
