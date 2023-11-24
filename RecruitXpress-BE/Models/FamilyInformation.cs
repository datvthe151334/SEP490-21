using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class FamilyInformation
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

        public virtual Profile? Profile { get; set; }
    }
}
