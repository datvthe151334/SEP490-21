using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class UserAnalytic
    {
        public int AnalyticId { get; set; }
        public string? Type { get; set; }
        public string? Data { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
