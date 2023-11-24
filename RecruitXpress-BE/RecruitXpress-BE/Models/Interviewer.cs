using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Interviewer
    {
        public int InterviewerId { get; set; }
        public int? ScheduleId { get; set; }
        public int? Status { get; set; }

        public virtual Profile? InterviewerNavigation { get; set; } = null!;
        public virtual Schedule? Schedule { get; set; } = null!;
    }
}
