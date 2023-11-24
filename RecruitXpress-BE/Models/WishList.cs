using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class WishList
    {
        public int WishlistId { get; set; }
        public int? AccountId { get; set; }
        public int? JobId { get; set; }
        public int? Status { get; set; }

        public virtual Account? Account { get; set; }
        public virtual JobPosting? Job { get; set; }
    }
}
