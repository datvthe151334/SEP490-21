using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class EducationalBackground
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

        public virtual Profile? Profile { get; set; }
    }
}
