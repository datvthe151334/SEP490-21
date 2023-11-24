using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Question
    {
        public Question()
        {
            Options = new HashSet<Option>();
        }

        public int QuestionId { get; set; }
        public string? Question1 { get; set; }
        public string? Type { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }

        public virtual Account? CreatedByNavigation { get; set; }
        public virtual ICollection<Option> Options { get; set; }
    }
}
