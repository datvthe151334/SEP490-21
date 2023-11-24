using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class ShortListing
    {
        public int ListId { get; set; }
        public int? ProfileId { get; set; }
        public int? JobId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual JobPosting? Job { get; set; }
        public virtual Profile? Profile { get; set; }
    }
}
