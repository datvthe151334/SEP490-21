using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Account
    {
        public Account()
        {
            CandidateCvs = new HashSet<CandidateCv>();
            Cvtemplates = new HashSet<Cvtemplate>();
            EmailTokens = new HashSet<EmailToken>();
            Exams = new HashSet<Exam>();
            GeneralTests = new HashSet<GeneralTest>();
            NotificationReceivers = new HashSet<Notification>();
            NotificationSenders = new HashSet<Notification>();
            Profiles = new HashSet<Profile>();
            Questions = new HashSet<Question>();
            SpecializedExams = new HashSet<SpecializedExam>();
            WishLists = new HashSet<WishList>();
        }

        public int AccountId { get; set; }
        public string? Account1 { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public string? Token { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<CandidateCv> CandidateCvs { get; set; }
        public virtual ICollection<Cvtemplate> Cvtemplates { get; set; }
        public virtual ICollection<EmailToken> EmailTokens { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<GeneralTest> GeneralTests { get; set; }
        public virtual ICollection<Notification> NotificationReceivers { get; set; }
        public virtual ICollection<Notification> NotificationSenders { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<SpecializedExam> SpecializedExams { get; set; }
        public virtual ICollection<WishList> WishLists { get; set; }
    }
}
