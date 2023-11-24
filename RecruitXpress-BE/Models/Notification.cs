using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string? Title { get; set; }
        public bool? Seen { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string? TargetUrl { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }

        public virtual Account? Receiver { get; set; }
        public virtual Account? Sender { get; set; }
    }
}
