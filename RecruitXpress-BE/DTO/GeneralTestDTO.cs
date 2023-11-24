using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class GeneralTestDTO
    {
        public int GeneralTestId { get; set; }
        public int? ProfileId { get; set; }
        public string? TestName { get; set; }
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public AccountDTO? CreatedByAccount { get; set; }
        public Profile? Profile { get; set; }
        public List<GeneralTestDetailDTO> GeneralTestDetails { get; set; }
    }
    public partial class GeneralTestDetailDTO
    {
        public int DetailId { get; set; }
        public int? GeneralTestId { get; set; }
        public int? QuestionId { get; set; }
        public int? Answer { get; set; }
        public int? Point { get; set; }
        public int? Status { get; set; }


    }



}
