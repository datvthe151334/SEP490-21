using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class EmailTemplate
    {
        public int TemplateId { get; set; }
        public string? Title { get; set; }
        public string? Header { get; set; }
        public string? Body { get; set; }
        public string? SendTo { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }
        public int? MailType { get; set; }
    }
}
