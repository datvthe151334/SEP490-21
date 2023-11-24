using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class LanguageProficiency
    {
        public int LanguageProficiencyId { get; set; }
        public int? ProfileId { get; set; }
        public string? Language { get; set; }
        public string? ProficiencyLevel { get; set; }
        public string? Certifications { get; set; }
        public string? LanguageExperiences { get; set; }
        public string? TestScores { get; set; }
        public string? Notes { get; set; }

        public virtual Profile? Profile { get; set; }
    }
}
