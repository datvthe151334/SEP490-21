using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class ScheduleDetail
    {
        public int ScheduleDetailId { get; set; }
        public int? ScheduleId { get; set; }
        public int? CandidateId { get; set; }
        public int? ScheduleType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int? Status { get; set; }

        public virtual JobApplication? Candidate { get; set; }
        public virtual Schedule? Schedule { get; set; }
    }
}
