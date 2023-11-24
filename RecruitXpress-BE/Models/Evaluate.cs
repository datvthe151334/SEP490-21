using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Evaluate
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

        public virtual Profile Profile { get; set; } = null!;
    }
}
