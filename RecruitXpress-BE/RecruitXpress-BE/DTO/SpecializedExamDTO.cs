using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class SpecializedExamDTO
    {
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
        public virtual AccountDTO? CreatedByAccount { get; set; }
        public string JobTitle { get; set; }
    }
}
