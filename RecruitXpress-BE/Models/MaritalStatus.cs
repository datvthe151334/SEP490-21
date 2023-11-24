using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class MaritalStatus
    {
        public MaritalStatus()
        {
            Profiles = new HashSet<Profile>();
        }

        public int StatusId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }
    }
}
