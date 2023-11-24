using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class GeneralTestDetail
    {
        public int DetailId { get; set; }
        public int? GeneralTestId { get; set; }
        public int? QuestionId { get; set; }
        public int? Answer { get; set; }
        public int? Point { get; set; }
        public int? Status { get; set; }

        public virtual Option? AnswerNavigation { get; set; }
        public virtual GeneralTest? GeneralTest { get; set; }
    }
}
