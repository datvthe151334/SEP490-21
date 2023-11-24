using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Cvtemplate
    {
        public int CvtemplateId { get; set; }
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }

        public virtual Account? CreatedByNavigation { get; set; }
    }
}
