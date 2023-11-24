using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class CandidateCv
    {
        public CandidateCv()
        {
            JobApplications = new HashSet<JobApplication>();
        }

        public int TemplateId { get; set; }
        public int? AccountId { get; set; }
        public string? Url { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ICollection<JobApplication> JobApplications { get; set; }
    }
}
