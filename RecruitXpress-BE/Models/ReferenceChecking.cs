using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class ReferenceChecking
    {
        public int ReferenceCheckingId { get; set; }
        public int? ProfileId { get; set; }
        public int? NameOf { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual Profile? Profile { get; set; }
    }
}
