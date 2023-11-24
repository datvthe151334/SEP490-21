using Org.BouncyCastle.Utilities;
using System.Text.RegularExpressions;

namespace RecruitXpress_BE.Helper
{
    public static class Constant
    {
     
        public static readonly Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[0-9]).{8,32}$");
        public static readonly int MaxFileSize = 25 * 1024 * 1024; //(Bytes = 25mb)
        public static readonly int ExpireExamDays = 5;
        public static class GOOGLE_SERVICE
        {
            public const string SCOPE_URL = "https://accounts.google.com/o/oauth2/auth/oauthchooseaccount?";
            public const string CLIENT_ID = "738514220360-t74atgpqn4p0ooho0c9trgnukd1b13n8.apps.googleusercontent.com";
            public const string CLIENT_SERCRET = "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";
            public const string RESPONSE_TYPE = "code";
            public const string ACCESS_TYPE = "offline";
            public static class SCOPE
            {
                // public const string EMAIL = "https://www.googleapis.com/auth/userinfo.email";
                public const string EMAIL = "email";
            }
        }
        
        public static class ENTITY_STATUS
        {
            public const int ACTIVE = 1;
            public const int INACTIVE = 0;
            
        }
        
        public static class ROLE
        {
            public const int ADMIN = 1;
            public const int CANDIDATE = 2;
            public const int HUMAN_RESOURCE = 3;
            public const int INTERVIEWER = 4;
            public const int EXPERT = 5;
        }

        public static class MailType
        {
            public static int REFUSED = 0;
            public static int SUBMIT = 1;
            public static int PASSCV = 2;
            public static int EXAMSCHEDULE = 3;
            public static int GRADERESULT = 4;
            public static int INTERVIEWSCHEDULE = 6;
            public static int PASSINTERVIEW = 7;
            public static int ACCEPTED = 8;
            public static int CANCEL = 9;
        }
        
        public static class SCHEDULE_TYPE
        {
            public static int EXAM = 2;
            public static int INTERVIEW = 1;
        }
    }
    public static class ConstantQuestion
    {
        public static int easy = 5;
        public static int medium = 3;
        public static int hard = 2;
    }
}
