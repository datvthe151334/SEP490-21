using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class FamilyInformationDTO
    {
        public int FamilyId { get; set; }
        public int? ProfileId { get; set; }
        public string? FamilyName { get; set; }
        public string? RelationshipStatus { get; set; }
        public string? EducationLevel { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? Birthdays { get; set; }
        public int? Status { get; set; }

    }
   
}
