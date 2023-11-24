using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class training
    {
        public int TrainingId { get; set; }
        public int? ProfileId { get; set; }
        public string? FormatName { get; set; }
        public string? Duration { get; set; }
        public string? Location { get; set; }
        public string? Language { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SkillsCovered { get; set; }
        public string? CertificationOffered { get; set; }

        public virtual Profile? Profile { get; set; }
    }
}
