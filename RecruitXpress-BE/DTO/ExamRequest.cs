using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class ExamRequestClass
    {
        public int? AccountId { get; set; }
        public string? FileUrl { get; set; }
        public int? SpecializedExamId { get; set; }

    }
    public class GradeExamRequest
    {
        public int ExamId { get; set; }
        public string? Point { get; set; }
        public string? Comment { get; set; }
        public string? MarkedBy { get; set; }

    }
}
