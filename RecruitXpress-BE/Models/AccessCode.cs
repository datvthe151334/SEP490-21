using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class AccessCode
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DateTime ExpirationTimestamp { get; set; }
        public string? ExamCode { get; set; }
    }
}
