namespace RecruitXpress_BE.DTO
{
    public class ReportDTO
    {
        public int EvaluateId { get; set; }
        public int ProfileId { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public double? Mark { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? EvaluatedBy { get; set; }
        public string? EvaluaterPhoneContact { get; set; }
        public string? EvaluaterEmailContact { get; set; }
        public int? EvaluaterAccountId { get; set; }
        public int? Status { get; set; }
    }
}
