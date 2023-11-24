using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public string? Question1 { get; set; }
        public string? Type { get; set; }
        public int? Status { get; set; }
        public virtual AccountDTO? CreatedByAccount { get; set; }
        public List<OptionDTO> Options { get; set; }
    }

    public partial class OptionDTO
    {
        public int OptionId { get; set; }
        public int? QuestionId { get; set; }
        public string? OptionText { get; set; }
        public bool? IsCorrect { get; set; }
        public int? Status { get; set; }

    }

    public class ExamQuestionDTO
    {
        public int QuestionId { get; set; }
        public string? Question1 { get; set; }
        public List<ExamOptionDTO> Options { get; set; }
    }

    public partial class ExamOptionDTO
    {
        public int OptionId { get; set; }
        public int? QuestionId { get; set; }
        public string? OptionText { get; set; }

    }


}
