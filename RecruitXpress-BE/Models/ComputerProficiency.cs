using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class ComputerProficiency
    {
        public int ComputerProficiencyId { get; set; }
        public int? ProfileId { get; set; }
        public string? TechnicalSkills { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public string? SkillLevel { get; set; }
        public int? ExperienceYears { get; set; }
        public int? Status { get; set; }

        public virtual Profile? Profile { get; set; }
    }
}
