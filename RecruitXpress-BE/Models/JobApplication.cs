using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class JobApplication
    {
        public JobApplication()
        {
            ScheduleDetails = new HashSet<ScheduleDetail>();
        }

        public int ApplicationId { get; set; }
        public int? JobId { get; set; }
        public int? ProfileId { get; set; }
        public int? TemplateId { get; set; }
        public int? Status { get; set; }
        public int? AssignedFor { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual JobPosting? Job { get; set; }
        public virtual Profile? Profile { get; set; }
        public virtual CandidateCv? Template { get; set; }
        public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
    }
}
