using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{


    public class CvRequestClass
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? CreatedBy { get; set; }
    }
}
