using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class MaritalStatusDTO
    {
        public int StatusId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
   
}
