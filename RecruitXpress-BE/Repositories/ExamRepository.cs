using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _sender;
        private readonly IConfiguration _configuration;
        IJobApplicationRepository _jobApplicationRepository;
        public ExamRepository(RecruitXpressContext context, IMapper mapper, IEmailSender sender, IConfiguration configuration, IJobApplicationRepository jobApplicationRepository)
        {
            _context = context;
            _mapper = mapper;
            _sender = sender;
            _configuration = configuration;
            _jobApplicationRepository = jobApplicationRepository;
        }

        public async Task<ApiResponse<ExamDTO>> GetAllExams(ExamRequest request)
        {
            var query = _context.Exams
       .Include(e => e.Account)
            .AsQueryable();

            if (!string.IsNullOrEmpty(request.FileUrl))
            {
                query = query.Where(e => e.FileUrl.Contains(request.FileUrl));
            }

            if (request.TestDate.HasValue)
            {
                var testDate = request.TestDate.Value.Date;
                query = query.Where(e => e.TestDate != null && e.TestDate.Value.Date == testDate);
            }

            if (!string.IsNullOrEmpty(request.Point))
            {
                query = query.Where(e => e.Point.Contains(request.Point));
            }

            if (!string.IsNullOrEmpty(request.Comment))
            {
                query = query.Where(e => e.Comment != null && e.Comment.Contains(request.Comment));
            }

            if (!string.IsNullOrEmpty(request.MarkedBy))
            {
                query = query.Where(e => e.MarkedBy != null && e.MarkedBy.Contains(request.MarkedBy));
            }

            if (request.MarkedDate.HasValue)
            {
                var markedDate = request.MarkedDate.Value.Date;
                query = query.Where(e => e.MarkedDate != null && e.MarkedDate.Value.Date == markedDate);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status);
            }

            if (request.AccountId.HasValue)
            {
                query = query.Where(e => e.AccountId == request.AccountId);
            }

            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "testDate":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.TestDate)
                            : query.OrderByDescending(e => e.TestDate);
                        break;

                    case "point":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.Point)
                            : query.OrderByDescending(e => e.Point);
                        break;

                    // Add other sorting options if needed.
                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.ExamId)
                            : query.OrderByDescending(e => e.ExamId);
                        break;
                }
            }
            var totalCount = await query.CountAsync();

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;

            var exams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var examDTOs = _mapper.Map<List<ExamDTO>>(exams);


            var response = new ApiResponse<ExamDTO>
            {
                Items = examDTOs,
                TotalCount = totalCount,
            };
            return response;
        }

        public async Task<ApiResponse<ExamDTO>> GetListExamWithSpecializedExamId(ExamRequest request, int sid)
        {
            var query = _context.Exams
            .Where(e => e.SpecializedExamId == sid)
            .Include(e => e.Account)
            .AsQueryable();

            if (!string.IsNullOrEmpty(request.FileUrl))
            {
                query = query.Where(e => e.FileUrl.Contains(request.FileUrl));
            }

            if (request.TestDate.HasValue)
            {
                var testDate = request.TestDate.Value.Date;
                query = query.Where(e => e.TestDate != null && e.TestDate.Value.Date == testDate);
            }

            if (!string.IsNullOrEmpty(request.Point))
            {
                query = query.Where(e => e.Point.Contains(request.Point));
            }

            if (!string.IsNullOrEmpty(request.Comment))
            {
                query = query.Where(e => e.Comment != null && e.Comment.Contains(request.Comment));
            }

            if (!string.IsNullOrEmpty(request.MarkedBy))
            {
                query = query.Where(e => e.MarkedBy != null && e.MarkedBy.Contains(request.MarkedBy));
            }

            if (request.MarkedDate.HasValue)
            {
                var markedDate = request.MarkedDate.Value.Date;
                query = query.Where(e => e.MarkedDate != null && e.MarkedDate.Value.Date == markedDate);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status);
            }

            if (request.AccountId.HasValue)
            {
                query = query.Where(e => e.AccountId == request.AccountId);
            }

            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "testDate":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.TestDate)
                            : query.OrderByDescending(e => e.TestDate);
                        break;

                    case "point":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.Point)
                            : query.OrderByDescending(e => e.Point);
                        break;

                    // Add other sorting options if needed.
                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.ExamId)
                            : query.OrderByDescending(e => e.ExamId);
                        break;
                }
            }
            var totalCount = await query.CountAsync();
            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;

            var exams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var examDTOs = _mapper.Map<List<ExamDTO>>(exams);
            var response = new ApiResponse<ExamDTO>
            {
                Items = examDTOs,
                TotalCount = totalCount,
            };
            return response;
        }

        public async Task<ExamDTO> GetExamById(int examId)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.ExamId == examId);
            return _mapper.Map<ExamDTO>(exam);
        }

        public async Task<ApiResponse<ExamDTO>> GetListExamWithSpecializedExamCode(ExamRequest request, string code, string expertEmail)
        {
            var allowed = await _context.AccessCodes
                 .Where(s => s.ExamCode.Contains(code) && s.Email.Equals(expertEmail) && s.ExpirationTimestamp > DateTime.Now).FirstOrDefaultAsync();

            if (allowed == null || !allowed.ExamCode.Equals(code))
            {
                throw new ArgumentException("Không có quyền truy cập");
            }

            var specializedExam = await _context.SpecializedExams
                 .Include(s => s.CreatedByNavigation)
                 .FirstOrDefaultAsync(s => s.Code.Contains(code));

            var query = _context.Exams
            .Where(e => e.SpecializedExamId == specializedExam.ExamId)
            .Include(e => e.Account)
            .AsQueryable();

            if (!string.IsNullOrEmpty(request.FileUrl))
            {
                query = query.Where(e => e.FileUrl.Contains(request.FileUrl));
            }

            if (request.TestDate.HasValue)
            {
                var testDate = request.TestDate.Value.Date;
                query = query.Where(e => e.TestDate != null && e.TestDate.Value.Date == testDate);
            }

            if (!string.IsNullOrEmpty(request.Point))
            {
                query = query.Where(e => e.Point.Contains(request.Point));
            }

            if (!string.IsNullOrEmpty(request.Comment))
            {
                query = query.Where(e => e.Comment != null && e.Comment.Contains(request.Comment));
            }

            if (!string.IsNullOrEmpty(request.MarkedBy))
            {
                query = query.Where(e => e.MarkedBy != null && e.MarkedBy.Contains(request.MarkedBy));
            }

            if (request.MarkedDate.HasValue)
            {
                var markedDate = request.MarkedDate.Value.Date;
                query = query.Where(e => e.MarkedDate != null && e.MarkedDate.Value.Date == markedDate);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status);
            }

            if (request.AccountId.HasValue)
            {
                query = query.Where(e => e.AccountId == request.AccountId);
            }

            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "testDate":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.TestDate)
                            : query.OrderByDescending(e => e.TestDate);
                        break;

                    case "point":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.Point)
                            : query.OrderByDescending(e => e.Point);
                        break;

                    // Add other sorting options if needed.
                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.ExamId)
                            : query.OrderByDescending(e => e.ExamId);
                        break;
                }
            }
            var totalCount = await query.CountAsync();
            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;

            var exams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var examDTOs = _mapper.Map<List<ExamDTO>>(exams);
            var response = new ApiResponse<ExamDTO>
            {
                Items = examDTOs,
                TotalCount = totalCount,
            };
            return response;
        }
        public async Task<Exam> CreateExamWithFile(ExamRequestClass exam, IFormFile fileData)
        {
            try
            {
                if (exam.AccountId == 0)
                {
                    throw new ArgumentException("Invalid AccountId");
                }
                if (exam.SpecializedExamId == 0)
                {
                    throw new ArgumentException("Invalid ExamId");
                }
                if (fileData == null || fileData.Length == 0)
                {
                    throw new ArgumentException("Please upload a file");
                }

                if (fileData.Length > Constant.MaxFileSize)
                {
                    throw new ArgumentException("File size exceeds the maximum allowed (25MB)");
                }

                var specExam = _context.SpecializedExams.Where(e => e.ExamId == exam.SpecializedExamId).FirstOrDefault();
                if (string.IsNullOrEmpty(specExam.Code))
                {
                    throw new ArgumentException("Invalid ExamId");
                }
                // Get the file extension
                var fileExtension = Path.GetExtension(fileData.FileName);
                int timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var fileName = $"{timestamp}_{exam.AccountId}{fileExtension}";

                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"Upload\\ExamFiles"));
                string folder = $"{specExam.Code}";
                var newPath = Path.Combine(path, folder);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                // Save the file bytes
                var filePath = Path.Combine(path, folder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileData.CopyToAsync(fileStream);
                }

                // Set the FileUrl property in the exam object
                var newExam = new Exam
                {
                    AccountId = exam.AccountId,
                    SpecializedExamId = exam.SpecializedExamId,
                    TestDate = DateTime.Now.Date,
                    TestTime = DateTime.Now,
                    FileUrl = Path.Combine(folder, fileName),
                    Status = specExam.EndDate > DateTime.Now ? 1 : 0,
                };


                // Add the exam to the database
                _context.Exams.Add(newExam);
                await _context.SaveChangesAsync();

                return newExam;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AssignExpertToSystem(string email, string examCode)
        {
            var a = new AccessCode
            {
                Email = email,
                Code = GenerateUniqueCode(),
                ExamCode = examCode,
                ExpirationTimestamp = DateTime.Now.AddDays(Constant.ExpireExamDays),
            };
            _context.AccessCodes.AddAsync(a);
            var sExam = _context.SpecializedExams.Where(s => s.Code == examCode).FirstOrDefault();
            if (sExam == null)
                throw new Exception("Không tìm thấy bài thi chuyên môn");
            if (sExam.ExpertEmail != null)
            {
                var oldExperts = _context.AccessCodes.Where(a => a.ExamCode == examCode).ToList();
                if (oldExperts != null)
                    _context.RemoveRange(oldExperts);
            }
            sExam.ExpertEmail = email;
            await _context.SaveChangesAsync();
            string url = _configuration["Website:ClientUrl"] + "/loginforexpert";
            string urlExam = _configuration["Website:ClientUrl"] + "/Exam/" + examCode;

            var emailContent = "<!DOCTYPE html\r\n    PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"vi\"\r\n    style=\"font-family:arial, 'helvetica neue', helvetica, sans-serif\">\r\n\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\r\n    <meta name=\"x-apple-disable-message-reformatting\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <meta content=\"telephone=no\" name=\"format-detection\">\r\n    <title>New email template 2023-10-28</title>\r\n    <!--[if (mso 16)]><style type=\"text/css\">     a {text-decoration: none;}     </style><![endif]-->\r\n    <!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]--> <!--[if gte mso 9]><xml> <o:OfficeDocumentSettings> <o:AllowPNG></o:AllowPNG> <o:PixelsPerInch>96</o:PixelsPerInch> </o:OfficeDocumentSettings> </xml>\r\n    <![endif]--> <!--[if !mso]><!-- -->\r\n    <link href=\"https://fonts.googleapis.com/css2?family=Imprima&display=swap\" rel=\"stylesheet\"> <!--<![endif]-->\r\n    <style type=\"text/css\">\r\n        #outlook a {\r\n            padding: 0;\r\n        }\r\n\r\n        .es-button {\r\n            mso-style-priority: 100 !important;\r\n            text-decoration: none !important;\r\n        }\r\n\r\n        a[x-apple-data-detectors] {\r\n            color: inherit !important;\r\n            text-decoration: none !important;\r\n            font-size: inherit !important;\r\n            font-family: inherit !important;\r\n            font-weight: inherit !important;\r\n            line-height: inherit !important;\r\n        }\r\n\r\n        .es-desk-hidden {\r\n            display: none;\r\n            float: left;\r\n            overflow: hidden;\r\n            width: 0;\r\n            max-height: 0;\r\n            line-height: 0;\r\n            mso-hide: all;\r\n        }\r\n\r\n        @media only screen and (max-width:600px) {\r\n\r\n            p,\r\n            ul li,\r\n            ol li,\r\n            a {\r\n                line-height: 150% !important\r\n            }\r\n\r\n            h1,\r\n            h2,\r\n            h3,\r\n            h1 a,\r\n            h2 a,\r\n            h3 a {\r\n                line-height: 120%\r\n            }\r\n\r\n            h1 {\r\n                font-size: 30px !important;\r\n                text-align: left\r\n            }\r\n\r\n            h2 {\r\n                font-size: 24px !important;\r\n                text-align: left\r\n            }\r\n\r\n            h3 {\r\n                font-size: 20px !important;\r\n                text-align: left\r\n            }\r\n\r\n            .es-header-body h1 a,\r\n            .es-content-body h1 a,\r\n            .es-footer-body h1 a {\r\n                font-size: 30px !important;\r\n                text-align: left\r\n            }\r\n\r\n            .es-header-body h2 a,\r\n            .es-content-body h2 a,\r\n            .es-footer-body h2 a {\r\n                font-size: 24px !important;\r\n                text-align: left\r\n            }\r\n\r\n            .es-header-body h3 a,\r\n            .es-content-body h3 a,\r\n            .es-footer-body h3 a {\r\n                font-size: 20px !important;\r\n                text-align: left\r\n            }\r\n\r\n            .es-menu td a {\r\n                font-size: 14px !important\r\n            }\r\n\r\n            .es-header-body p,\r\n            .es-header-body ul li,\r\n            .es-header-body ol li,\r\n            .es-header-body a {\r\n                font-size: 14px !important\r\n            }\r\n\r\n            .es-content-body p,\r\n            .es-content-body ul li,\r\n            .es-content-body ol li,\r\n            .es-content-body a {\r\n                font-size: 14px !important\r\n            }\r\n\r\n            .es-footer-body p,\r\n            .es-footer-body ul li,\r\n            .es-footer-body ol li,\r\n            .es-footer-body a {\r\n                font-size: 14px !important\r\n            }\r\n\r\n            .es-infoblock p,\r\n            .es-infoblock ul li,\r\n            .es-infoblock ol li,\r\n            .es-infoblock a {\r\n                font-size: 12px !important\r\n            }\r\n\r\n            *[class=\"gmail-fix\"] {\r\n                display: none !important\r\n            }\r\n\r\n            .es-m-txt-c,\r\n            .es-m-txt-c h1,\r\n            .es-m-txt-c h2,\r\n            .es-m-txt-c h3 {\r\n                text-align: center !important\r\n            }\r\n\r\n            .es-m-txt-r,\r\n            .es-m-txt-r h1,\r\n            .es-m-txt-r h2,\r\n            .es-m-txt-r h3 {\r\n                text-align: right !important\r\n            }\r\n\r\n            .es-m-txt-l,\r\n            .es-m-txt-l h1,\r\n            .es-m-txt-l h2,\r\n            .es-m-txt-l h3 {\r\n                text-align: left !important\r\n            }\r\n\r\n            .es-m-txt-r img,\r\n            .es-m-txt-c img,\r\n            .es-m-txt-l img {\r\n                display: inline !important\r\n            }\r\n\r\n            .es-button-border {\r\n                display: block !important\r\n            }\r\n\r\n            a.es-button,\r\n            button.es-button {\r\n                font-size: 18px !important;\r\n                display: block !important;\r\n                border-right-width: 0px !important;\r\n                border-left-width: 0px !important;\r\n                border-top-width: 15px !important;\r\n                border-bottom-width: 15px !important\r\n            }\r\n\r\n            .es-adaptive table,\r\n            .es-left,\r\n            .es-right {\r\n                width: 100% !important\r\n            }\r\n\r\n            .es-content table,\r\n            .es-header table,\r\n            .es-footer table,\r\n            .es-content,\r\n            .es-footer,\r\n            .es-header {\r\n                width: 100% !important;\r\n                max-width: 600px !important\r\n            }\r\n\r\n            .es-adapt-td {\r\n                display: block !important;\r\n                width: 100% !important\r\n            }\r\n\r\n            .adapt-img {\r\n                width: 100% !important;\r\n                height: auto !important\r\n            }\r\n\r\n            .es-m-p0 {\r\n                padding: 0px !important\r\n            }\r\n\r\n            .es-m-p0r {\r\n                padding-right: 0px !important\r\n            }\r\n\r\n            .es-m-p0l {\r\n                padding-left: 0px !important\r\n            }\r\n\r\n            .es-m-p0t {\r\n                padding-top: 0px !important\r\n            }\r\n\r\n            .es-m-p0b {\r\n                padding-bottom: 0 !important\r\n            }\r\n\r\n            .es-m-p20b {\r\n                padding-bottom: 20px !important\r\n            }\r\n\r\n            .es-mobile-hidden,\r\n            .es-hidden {\r\n                display: none !important\r\n            }\r\n\r\n            tr.es-desk-hidden,\r\n            td.es-desk-hidden,\r\n            table.es-desk-hidden {\r\n                width: auto !important;\r\n                overflow: visible !important;\r\n                float: none !important;\r\n                max-height: inherit !important;\r\n                line-height: inherit !important\r\n            }\r\n\r\n            tr.es-desk-hidden {\r\n                display: table-row !important\r\n            }\r\n\r\n            table.es-desk-hidden {\r\n                display: table !important\r\n            }\r\n\r\n            td.es-desk-menu-hidden {\r\n                display: table-cell !important\r\n            }\r\n\r\n            .es-menu td {\r\n                width: 1% !important\r\n            }\r\n\r\n            table.es-table-not-adapt,\r\n            .esd-block-html table {\r\n                width: auto !important\r\n            }\r\n\r\n            table.es-social {\r\n                display: inline-block !important\r\n            }\r\n\r\n            table.es-social td {\r\n                display: inline-block !important\r\n            }\r\n\r\n            .es-desk-hidden {\r\n                display: table-row !important;\r\n                width: auto !important;\r\n                overflow: visible !important;\r\n                max-height: inherit !important\r\n            }\r\n        }\r\n    </style>\r\n</head>\r\n\r\n<body\r\n    style=\"width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0\">\r\n    <div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"vi\" style=\"background-color:#FFFFFF\">\r\n        <!--[if gte mso 9]><v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\"> <v:fill type=\"tile\" color=\"#ffffff\"></v:fill> </v:background><![endif]-->\r\n        <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" role=\"none\"\r\n            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FFFFFF\">\r\n            <tr>\r\n                <td valign=\"top\" style=\"padding:0;Margin:0\">\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#bcb8b1\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\" role=\"none\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"Margin:0;padding-top:20px;padding-bottom:20px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\"\r\n                                                                    style=\"padding:0;Margin:0;display:none\"></td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;border-radius:20px 20px 0 0;width:600px\"\r\n                                    role=\"none\">\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"padding:0;Margin:0;padding-top:40px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            role=\"presentation\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"left\" class=\"es-m-txt-c\"\r\n                                                                    style=\"padding:0;Margin:0;font-size:0px\"><img\r\n                                                                        src=\"https://juctlf.stripocdn.email/content/guids/CABINET_ee77850a5a9f3068d9355050e69c76d26d58c3ea2927fa145f0d7a894e624758/images/group_4076323.png\"\r\n                                                                        alt=\"Confirm email\"\r\n                                                                        style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;border-radius:100px;font-size:12px\"\r\n                                                                        width=\"100\" title=\"Confirm email\" height=\"100\">\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"padding:0;Margin:0;padding-top:20px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            bgcolor=\"#fafafa\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;background-color:#fafafa;border-radius:10px\"\r\n                                                            role=\"presentation\">\r\n                                                            <tr>\r\n                                                                <td align=\"left\" style=\"padding:20px;Margin:0\">\r\n                                                                    <h3\r\n                                                                        style=\"Margin:0;line-height:34px;mso-line-height-rule:exactly;font-family:Imprima, Arial, sans-serif;font-size:28px;font-style:normal;font-weight:bold;color:#2D3142\">\r\n                                                                        Xin chào!</h3>\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        <br>\r\n                                                                    </p>\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        Bạn đã được thêm vào làm người chấm bài thi trên hệ thống RecruitXpress cho bài thi có code là: " +
            examCode + ". Thông tin đăng nhập của bạn như sau:\r\n                                                                    </p>\r\n                                                                    <p\r\n                                                                    style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                    Tên tài khoản: <strong>" +
            email + "</strong>\r\n                                                                     </p>\r\n                                                                <p\r\n                                                                style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                Mã xác thực: <strong>" +
            a.Code + "</strong>\r\n                                                            </p>\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        <br>\r\n                                                                    </p>\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        Vui lòng sử dụng thông tin này để truy cập vào hệ thống. Mã xác thực này được tạo tự động và sẽ hết hạn trong " +
            Constant.ExpireExamDays + " ngày. Cảm ơn sự hợp tác của bạn!</p>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\" role=\"none\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;width:600px\">\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"Margin:0;padding-top:30px;padding-bottom:40px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            role=\"presentation\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\" style=\"padding:0;Margin:0\"> <!--[if mso]><a href=\"https://viewstripo.email\" target=\"_blank\" hidden> <v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" esdevVmlButton href=\"https://viewstripo.email\" style=\"height:56px; v-text-anchor:middle; width:520px\" arcsize=\"50%\" stroke=\"f\" fillcolor=\"#7630f3\"> <w:anchorlock></w:anchorlock> <center style='color:#ffffff; font-family:Imprima, Arial, sans-serif; font-size:22px; font-weight:700; line-height:22px; mso-text-raise:1px'>Xác nhận email</center> </v:roundrect></a>\r\n    <![endif]--> <!--[if !mso]><!-- --><span class=\"msohide es-button-border\"\r\n                                                                        style=\"border-style:solid;border-color:#2CB543;background:#7630f3;border-width:0px;display:block;border-radius:30px;width:auto;mso-hide:all\"><a\r\n                                                                            href=\"" +
            url + "\"\r\n                                                                            class=\"es-button msohide\" target=\"_blank\"\r\n                                                                            style=\"mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:22px;padding:15px 20px 15px 20px;display:block;background:#7630f3;border-radius:30px;font-family:Imprima, Arial, sans-serif;font-weight:bold;font-style:normal;line-height:26px;width:auto;text-align:center;mso-padding-alt:0;mso-border-alt:10px solid #7630f3;mso-hide:all;padding-left:5px;padding-right:5px\">Truy cập vào hệ thống</a> </span> <!--<![endif]-->\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"padding:0;Margin:0;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            role=\"presentation\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"left\" style=\"padding:0;Margin:0\">\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        Trân trọng,<br><br>RecruitXpress</p>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                            <tr>\r\n                                                                <td align=\"center\"\r\n                                                                    style=\"padding:0;Margin:0;padding-bottom:20px;padding-top:40px;font-size:0\">\r\n                                                                    <table border=\"0\" width=\"100%\" height=\"100%\"\r\n                                                                        cellpadding=\"0\" cellspacing=\"0\"\r\n                                                                        role=\"presentation\"\r\n                                                                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                                        <tr>\r\n                                                                            <td\r\n                                                                                style=\"padding:0;Margin:0;border-bottom:1px solid #666666;background:unset;height:1px;width:100%;margin:0px\">\r\n                                                                            </td>\r\n                                                                        </tr>\r\n                                                                    </table>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;border-radius:0 0 20px 20px;width:600px\"\r\n                                    role=\"none\">\r\n                                    <tr>\r\n                                        <td class=\"esdev-adapt-off\" align=\"left\"\r\n                                            style=\"Margin:0;padding-top:20px;padding-bottom:20px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:520px\">\r\n                                                <tr>\r\n                                                    <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" align=\"left\"\r\n                                                            class=\"es-left\" role=\"none\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\" valign=\"top\"\r\n                                                                    style=\"padding:0;Margin:0;width:47px\">\r\n                                                                    <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                                        role=\"presentation\"\r\n                                                                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                                        <tr>\r\n                                                                            <td align=\"center\" class=\"es-m-txt-l\"\r\n                                                                                style=\"padding:0;Margin:0;font-size:0px\">\r\n                                                                                <img src=\"https://juctlf.stripocdn.email/content/guids/CABINET_ee77850a5a9f3068d9355050e69c76d26d58c3ea2927fa145f0d7a894e624758/images/group_4076325.png\"\r\n                                                                                    alt=\"Demo\"\r\n                                                                                    style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;font-size:12px\"\r\n                                                                                    width=\"47\" title=\"Demo\" height=\"47\">\r\n                                                                            </td>\r\n                                                                        </tr>\r\n                                                                    </table>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                    <td style=\"padding:0;Margin:0;width:20px\"></td>\r\n                                                    <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\"\r\n                                                            align=\"right\" role=\"none\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\" valign=\"top\"\r\n                                                                    style=\"padding:0;Margin:0;width:453px\">\r\n                                                                    <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                                        role=\"presentation\"\r\n                                                                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                                        <tr>\r\n                                                                            <td align=\"center\"\r\n                                                                                style=\"padding:0;Margin:0\">\r\n                                                                                <p\r\n                                                                                    style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:24px;color:#2D3142;font-size:16px\">\r\n                                                                                    Địa chỉ này sẽ hết hạn sau 24h.</p>\r\n                                                                            </td>\r\n                                                                        </tr>\r\n                                                                    </table>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#bcb8b1\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\" role=\"none\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"Margin:0;padding-left:20px;padding-right:20px;padding-bottom:30px;padding-top:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:560px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            role=\"presentation\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\" class=\"es-m-txt-c\"\r\n                                                                    style=\"padding:0;Margin:0;padding-bottom:20px;font-size:0px\">\r\n                                                                    <img src=\"https://juctlf.stripocdn.email/content/guids/CABINET_e0075a83086e72d11f342b3e29a0d40357691795c3b8e8a62708bbb046714fd2/images/recruit2_yRz.png\"\r\n                                                                        alt=\"Logo\"\r\n                                                                        style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;font-size:12px\"\r\n                                                                        title=\"Logo\" height=\"100\" width=\"238\">\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                            <tr>\r\n                                                                <td align=\"center\"\r\n                                                                    style=\"padding:0;Margin:0;padding-top:20px\">\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:21px;color:#2D3142;font-size:14px\">\r\n                                                                        <a target=\"_blank\" href=\"\"\r\n                                                                            style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#2D3142;font-size:14px\"></a>Copyright\r\n                                                                        © 2023 RecruitXpress<a target=\"_blank\" href=\"\"\r\n                                                                            style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#2D3142;font-size:14px\"></a>\r\n                                                                    </p>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#ffffff\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\" role=\"none\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n                                    <tr>\r\n                                        <td align=\"left\" style=\"padding:20px;Margin:0\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:560px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\"\r\n                                                                    style=\"padding:0;Margin:0;display:none\"></td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n    </div>\r\n</body>\r\n\r\n</html>";

            _sender.Send(email, "Thông tin xác thực chấm bài thi", emailContent);
        }
        public async Task GradeExam(GradeExamRequest e)
        {
            var exam = _context.Exams.FirstOrDefault(ex => ex.ExamId == e.ExamId);
            if (exam == null)
            {
                throw new Exception("Không tìm được bài thi");
            }

            exam.Comment = e.Comment;
            exam.Point = e.Point;
            exam.MarkedBy = e.MarkedBy;
            exam.MarkedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            var specializedExam = _context.SpecializedExams.Where(s => s.ExamId == exam.SpecializedExamId).FirstOrDefault();
            if (specializedExam.JobId == null)
                throw new Exception("Chưa có jobId trong bài thi này");
            await _jobApplicationRepository.FindJobApplicationAndUpdateStatus((int)specializedExam.JobId, (int)exam.AccountId);

        }


        public async Task UpdateExam(Exam exam)
        {
            _context.ChangeTracker.Clear();
            _context.Entry(exam).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteExam(int examId)
        {

            var exam = await _context.Exams.Where(o => o.ExamId == examId).FirstOrDefaultAsync();
            if (exam == null)
            {
                return false;
            }
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateUniqueCode()
        {
            int maxAttempts = 10; // Set a maximum number of attempts.
            int attemptCount = 0;
            string generatedCode;

            do
            {
                // Generate a random code.
                generatedCode = TokenHelper.GenerateRandomToken(10);
                attemptCount++;

                // Check if the generated code is unique.
                var isCodeUnique = !_context.AccessCodes.Any(e => e.Code == generatedCode);

                if (isCodeUnique)
                {
                    return generatedCode;
                }
            } while (attemptCount < maxAttempts);

            // Handle the case where a unique code couldn't be generated.
            throw new Exception("Failed to generate a unique code after multiple attempts.");
        }
    }
}
