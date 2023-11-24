using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class GeneralTest
    {
        public GeneralTest()
        {
            GeneralTestDetails = new HashSet<GeneralTestDetail>();
        }

        public int GeneralTestId { get; set; }
        public int? ProfileId { get; set; }
        public string? TestName { get; set; }
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Score { get; set; }

        public virtual Account? CreatedByNavigation { get; set; }
        public virtual Profile? Profile { get; set; }
        public virtual ICollection<GeneralTestDetail> GeneralTestDetails { get; set; }
    }
}
