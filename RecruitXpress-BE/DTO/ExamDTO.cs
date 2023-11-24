namespace RecruitXpress_BE.DTO
{
    public class ExamDTO
    {
        public int ExamId { get; set; }
        public int? AccountId { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? TestDate { get; set; }
        public DateTime? TestTime { get; set; }
        public string? Point { get; set; }
        public string? Comment { get; set; }
        public string? MarkedBy { get; set; }
        public DateTime? MarkedDate { get; set; }
        public int? Status { get; set; }
        public int? SpecializedExamId { get; set; }

        public virtual AccountDTO? Account { get; set; }
    }
}
