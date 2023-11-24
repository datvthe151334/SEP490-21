using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Schedule
    {
        public Schedule()
        {
            Interviewers = new HashSet<Interviewer>();
            ScheduleDetails = new HashSet<ScheduleDetail>();
        }

        public int ScheduleId { get; set; }
        public int? HumanResourceId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Profile? HumanResource { get; set; }
        public virtual ICollection<Interviewer> Interviewers { get; set; }
        public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
    }
}
