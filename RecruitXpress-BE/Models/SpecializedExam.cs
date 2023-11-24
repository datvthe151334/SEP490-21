using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class SpecializedExam
    {
        public SpecializedExam()
        {
            Exams = new HashSet<Exam>();
        }

        public int ExamId { get; set; }
        public string ExamName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }
        public string? Code { get; set; }
        public string? ExpertEmail { get; set; }
        public int? JobId { get; set; }

        public virtual Account? CreatedByNavigation { get; set; }
        public virtual JobPosting? Job { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
    }
}
