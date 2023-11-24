using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Configuration;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;
using System.Security.Cryptography;
using RecruitXpress_BE.Helper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata;
using Constant = RecruitXpress_BE.Helper.Constant;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Authentication/")]

    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly RecruitXpressContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IGoogleService _googleService;

        public AuthenticationController(RecruitXpressContext context, IConfiguration configuration,
            IEmailSender emailSender, IGoogleService googleService)
        {
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
            _googleService = googleService;
        }



        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            try
            {
                var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Account1 == model.Email);


                if (user == null || (user != null && user.Status == 0))
                {
                    if (user == null)
                    {
                        user = new Account
                        {
                            Account1 = model.Email,
                            Password = HashHelper.Encrypt(model.Password, _configuration),
                            RoleId = 2,
                            CreatedAt = DateTime.Now,
                            Status = 0,
                        };

                        _context.Accounts.Add(user);
                        await _context.SaveChangesAsync();
                    }

                    var token = TokenHelper.GenerateRandomToken(64);
                    string url = _configuration["Website:ClientUrl"] + "/verification-success/" + token;
                    string subject = "Xác nhận địa chỉ Email của bạn";
                    var emailtoken = new EmailToken
                    {
                        Token = token,
                        IssuedAt = DateTime.UtcNow,
                        ExpiredAt = DateTime.UtcNow.AddDays(1),
                        IsRevoked = false,
                        IsUsed = false,
                        AccountId = user.AccountId,
                    };

                    _context.Add(emailtoken);
                    await _context.SaveChangesAsync();

                    SendEmailConfirm(user.Account1, subject, url);

                    return Ok("Vui lòng xác thực email của bạn.");
                }
                else
                {
                    return BadRequest("Email đã tồn tại!");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost("Email")]
        public async Task<IActionResult> SendEmailConfirm(string to, string subject, string link)
        {
            try
            {
                var html =
                    "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"vi\" style=\"font-family:arial, 'helvetica neue', helvetica, sans-serif\"><head><meta charset=\"UTF-8\"><meta content=\"width=device-width, initial-scale=1\" name=\"viewport\"><meta name=\"x-apple-disable-message-reformatting\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta content=\"telephone=no\" name=\"format-detection\"><title>New email template 2023-10-28</title> <!--[if (mso 16)]><style type=\"text/css\">     a {text-decoration: none;}     </style><![endif]--> <!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]--> <!--[if gte mso 9]><xml> <o:OfficeDocumentSettings> <o:AllowPNG></o:AllowPNG> <o:PixelsPerInch>96</o:PixelsPerInch> </o:OfficeDocumentSettings> </xml>\r\n<![endif]--> <!--[if !mso]><!-- --><link href=\"https://fonts.googleapis.com/css2?family=Imprima&display=swap\" rel=\"stylesheet\"> <!--<![endif]--><style type=\"text/css\">#outlook a { padding:0;}.es-button { mso-style-priority:100!important; text-decoration:none!important;}a[x-apple-data-detectors] { color:inherit!important; text-decoration:none!important; font-size:inherit!important; font-family:inherit!important; font-weight:inherit!important; line-height:inherit!important;}.es-desk-hidden { display:none; float:left; overflow:hidden; width:0; max-height:0; line-height:0; mso-hide:all;}@media only screen and (max-width:600px) {p, ul li, ol li, a { line-height:150%!important } h1, h2, h3, h1 a, h2 a, h3 a { line-height:120% } h1 { font-size:30px!important; text-align:left } h2 { font-size:24px!important; text-align:left } h3 { font-size:20px!important; text-align:left }\r\n .es-header-body h1 a, .es-content-body h1 a, .es-footer-body h1 a { font-size:30px!important; text-align:left } .es-header-body h2 a, .es-content-body h2 a, .es-footer-body h2 a { font-size:24px!important; text-align:left } .es-header-body h3 a, .es-content-body h3 a, .es-footer-body h3 a { font-size:20px!important; text-align:left } .es-menu td a { font-size:14px!important } .es-header-body p, .es-header-body ul li, .es-header-body ol li, .es-header-body a { font-size:14px!important } .es-content-body p, .es-content-body ul li, .es-content-body ol li, .es-content-body a { font-size:14px!important } .es-footer-body p, .es-footer-body ul li, .es-footer-body ol li, .es-footer-body a { font-size:14px!important } .es-infoblock p, .es-infoblock ul li, .es-infoblock ol li, .es-infoblock a { font-size:12px!important } *[class=\"gmail-fix\"] { display:none!important }\r\n .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3 { text-align:center!important } .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3 { text-align:right!important } .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3 { text-align:left!important } .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img { display:inline!important } .es-button-border { display:block!important } a.es-button, button.es-button { font-size:18px!important; display:block!important; border-right-width:0px!important; border-left-width:0px!important; border-top-width:15px!important; border-bottom-width:15px!important } .es-adaptive table, .es-left, .es-right { width:100%!important } .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header { width:100%!important; max-width:600px!important } .es-adapt-td { display:block!important; width:100%!important } .adapt-img { width:100%!important; height:auto!important }\r\n .es-m-p0 { padding:0px!important } .es-m-p0r { padding-right:0px!important } .es-m-p0l { padding-left:0px!important } .es-m-p0t { padding-top:0px!important } .es-m-p0b { padding-bottom:0!important } .es-m-p20b { padding-bottom:20px!important } .es-mobile-hidden, .es-hidden { display:none!important } tr.es-desk-hidden, td.es-desk-hidden, table.es-desk-hidden { width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important } tr.es-desk-hidden { display:table-row!important } table.es-desk-hidden { display:table!important } td.es-desk-menu-hidden { display:table-cell!important } .es-menu td { width:1%!important } table.es-table-not-adapt, .esd-block-html table { width:auto!important } table.es-social { display:inline-block!important } table.es-social td { display:inline-block!important }\r\n .es-desk-hidden { display:table-row!important; width:auto!important; overflow:visible!important; max-height:inherit!important } }</style>\r\n </head> <body style=\"width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0\"><div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"vi\" style=\"background-color:#FFFFFF\"> <!--[if gte mso 9]><v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\"> <v:fill type=\"tile\" color=\"#ffffff\"></v:fill> </v:background><![endif]--><table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FFFFFF\"><tr>\r\n<td valign=\"top\" style=\"padding:0;Margin:0\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\"><tr><td align=\"center\" style=\"padding:0;Margin:0\"><table bgcolor=\"#bcb8b1\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\"><tr><td align=\"left\" style=\"Margin:0;padding-top:20px;padding-bottom:20px;padding-left:40px;padding-right:40px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr>\r\n<td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:520px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td> </tr></table></td></tr></table></td></tr></table></td></tr></table> <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\"><tr><td align=\"center\" style=\"padding:0;Margin:0\"><table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;border-radius:20px 20px 0 0;width:600px\" role=\"none\"><tr>\r\n<td align=\"left\" style=\"padding:0;Margin:0;padding-top:40px;padding-left:40px;padding-right:40px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:520px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"left\" class=\"es-m-txt-c\" style=\"padding:0;Margin:0;font-size:0px\"><img src=\"https://juctlf.stripocdn.email/content/guids/CABINET_ee77850a5a9f3068d9355050e69c76d26d58c3ea2927fa145f0d7a894e624758/images/group_4076323.png\" alt=\"Confirm email\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;border-radius:100px;font-size:12px\" width=\"100\" title=\"Confirm email\" height=\"100\"></td> </tr></table>\r\n</td></tr></table></td></tr><tr><td align=\"left\" style=\"padding:0;Margin:0;padding-top:20px;padding-left:40px;padding-right:40px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:520px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" bgcolor=\"#fafafa\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;background-color:#fafafa;border-radius:10px\" role=\"presentation\"><tr><td align=\"left\" style=\"padding:20px;Margin:0\"><h3 style=\"Margin:0;line-height:34px;mso-line-height-rule:exactly;font-family:Imprima, Arial, sans-serif;font-size:28px;font-style:normal;font-weight:bold;color:#2D3142\">Xin chào!</h3>\r\n <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\"><br></p><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">Bạn đang nhận được email này vì bạn vừa đăng ký tài khoản của RecruitXpress.</p><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\"><br></p>\r\n <p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">Vui lòng xác nhận địa chỉ email của bạn bằng cách bấm vào nút bên dưới. Nếu bạn không tạo tài khoản vui lòng bỏ qua email này.</p></td></tr></table></td></tr></table></td></tr></table></td></tr></table> <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\"><tr><td align=\"center\" style=\"padding:0;Margin:0\"><table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;width:600px\"><tr>\r\n<td align=\"left\" style=\"Margin:0;padding-top:30px;padding-bottom:40px;padding-left:40px;padding-right:40px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:520px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0\"> <!--[if mso]><a href=\"https://viewstripo.email\" target=\"_blank\" hidden> <v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" esdevVmlButton href=\"https://viewstripo.email\" style=\"height:56px; v-text-anchor:middle; width:520px\" arcsize=\"50%\" stroke=\"f\" fillcolor=\"#7630f3\"> <w:anchorlock></w:anchorlock> <center style='color:#ffffff; font-family:Imprima, Arial, sans-serif; font-size:22px; font-weight:700; line-height:22px; mso-text-raise:1px'>Xác nhận email</center> </v:roundrect></a>\r\n<![endif]--> <!--[if !mso]><!-- --><span class=\"msohide es-button-border\" style=\"border-style:solid;border-color:#2CB543;background:#7630f3;border-width:0px;display:block;border-radius:30px;width:auto;mso-hide:all\"><a href=\"" +
                    link +
                    "\" class=\"es-button msohide\" target=\"_blank\" style=\"mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:22px;padding:15px 20px 15px 20px;display:block;background:#7630f3;border-radius:30px;font-family:Imprima, Arial, sans-serif;font-weight:bold;font-style:normal;line-height:26px;width:auto;text-align:center;mso-padding-alt:0;mso-border-alt:10px solid #7630f3;mso-hide:all;padding-left:5px;padding-right:5px\">Xác nhận email</a> </span> <!--<![endif]--></td></tr></table></td></tr></table></td></tr><tr>\r\n<td align=\"left\" style=\"padding:0;Margin:0;padding-left:40px;padding-right:40px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:520px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"left\" style=\"padding:0;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">Trân trọng,<br><br>RecruitXpress</p></td></tr> <tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-bottom:20px;padding-top:40px;font-size:0\"><table border=\"0\" width=\"100%\" height=\"100%\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td style=\"padding:0;Margin:0;border-bottom:1px solid #666666;background:unset;height:1px;width:100%;margin:0px\"></td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table> <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\"><tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0\"><table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;border-radius:0 0 20px 20px;width:600px\" role=\"none\"><tr><td class=\"esdev-adapt-off\" align=\"left\" style=\"Margin:0;padding-top:20px;padding-bottom:20px;padding-left:40px;padding-right:40px\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:520px\"><tr><td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\"><table cellpadding=\"0\" cellspacing=\"0\" align=\"left\" class=\"es-left\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\"><tr>\r\n<td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:47px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" class=\"es-m-txt-l\" style=\"padding:0;Margin:0;font-size:0px\"><img src=\"https://juctlf.stripocdn.email/content/guids/CABINET_ee77850a5a9f3068d9355050e69c76d26d58c3ea2927fa145f0d7a894e624758/images/group_4076325.png\" alt=\"Demo\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;font-size:12px\" width=\"47\" title=\"Demo\" height=\"47\"></td> </tr></table></td></tr></table></td><td style=\"padding:0;Margin:0;width:20px\"></td>\r\n<td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\"><table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\" align=\"right\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\"><tr><td align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;width:453px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" style=\"padding:0;Margin:0\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:24px;color:#2D3142;font-size:16px\">Địa chỉ này sẽ hết hạn sau 24h.</p></td></tr> </table></td></tr></table></td></tr></table></td></tr></table></td></tr></table>\r\n <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\"><tr><td align=\"center\" style=\"padding:0;Margin:0\"><table bgcolor=\"#bcb8b1\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\"><tr><td align=\"left\" style=\"Margin:0;padding-left:20px;padding-right:20px;padding-bottom:30px;padding-top:40px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr>\r\n<td align=\"left\" style=\"padding:0;Margin:0;width:560px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" class=\"es-m-txt-c\" style=\"padding:0;Margin:0;padding-bottom:20px;font-size:0px\"><img src=\"https://juctlf.stripocdn.email/content/guids/CABINET_e0075a83086e72d11f342b3e29a0d40357691795c3b8e8a62708bbb046714fd2/images/recruit2_yRz.png\" alt=\"Logo\" style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;font-size:12px\" title=\"Logo\" height=\"100\" width=\"238\"></td> </tr><tr>\r\n<td align=\"center\" style=\"padding:0;Margin:0;padding-top:20px\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:21px;color:#2D3142;font-size:14px\"><a target=\"_blank\" href=\"\" style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#2D3142;font-size:14px\"></a>Copyright © 2023 RecruitXpress<a target=\"_blank\" href=\"\" style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#2D3142;font-size:14px\"></a></p></td></tr></table></td></tr></table></td></tr></table></td></tr></table>\r\n <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\"><tr><td align=\"center\" style=\"padding:0;Margin:0\"><table bgcolor=\"#ffffff\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\"><tr><td align=\"left\" style=\"padding:20px;Margin:0\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr>\r\n<td align=\"left\" style=\"padding:0;Margin:0;width:560px\"><table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\"><tr><td align=\"center\" style=\"padding:0;Margin:0;display:none\"></td> </tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></div></body></html>";
                var from = "recruitmentxpressofficial@gmail.com";

                _emailSender.Send(to, subject, html, from); // Assuming Send is asynchronous.

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        private async Task<IActionResult> SendEmailResetPassword(string to, string subject, string link)
        {
            try
            {
                var html =
                    "<!DOCTYPE html\r\n    PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html dir=\"ltr\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" lang=\"vi\"\r\n    style=\"font-family:arial, 'helvetica neue', helvetica, sans-serif\">\r\n\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta content=\"width=device-width, initial-scale=1\" name=\"viewport\">\r\n    <meta name=\"x-apple-disable-message-reformatting\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <meta content=\"telephone=no\" name=\"format-detection\">\r\n    <title>New email template 2023-10-28</title>\r\n    <!--[if (mso 16)]><style type=\"text/css\">     a {text-decoration: none;}     </style><![endif]-->\r\n    <!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]--> <!--[if gte mso 9]><xml> <o:OfficeDocumentSettings> <o:AllowPNG></o:AllowPNG> <o:PixelsPerInch>96</o:PixelsPerInch> </o:OfficeDocumentSettings> </xml>\r\n    <![endif]--> <!--[if !mso]><!-- -->\r\n    <link href=\"https://fonts.googleapis.com/css2?family=Imprima&display=swap\" rel=\"stylesheet\"> <!--<![endif]-->\r\n    <style type=\"text/css\">\r\n        #outlook a {\r\n            padding: 0;\r\n        }\r\n\r\n        .es-button {\r\n            mso-style-priority: 100 !important;\r\n            text-decoration: none !important;\r\n        }\r\n\r\n        a[x-apple-data-detectors] {\r\n            color: inherit !important;\r\n            text-decoration: none !important;\r\n            font-size: inherit !important;\r\n            font-family: inherit !important;\r\n            font-weight: inherit !important;\r\n            line-height: inherit !important;\r\n        }\r\n\r\n        .es-desk-hidden {\r\n            display: none;\r\n            float: left;\r\n            overflow: hidden;\r\n            width: 0;\r\n            max-height: 0;\r\n            line-height: 0;\r\n            mso-hide: all;\r\n        }\r\n\r\n        @media only screen and (max-width:600px) {\r\n\r\n            p,\r\n            ul li,\r\n            ol li,\r\n            a {\r\n                line-height: 150% !important\r\n            }\r\n\r\n            h1,\r\n            h2,\r\n            h3,\r\n            h1 a,\r\n            h2 a,\r\n            h3 a {\r\n                line-height: 120%\r\n            }\r\n\r\n            h1 {\r\n                font-size: 30px !important;\r\n                text-align: left\r\n            }\r\n\r\n            h2 {\r\n                font-size: 24px !important;\r\n                text-align: left\r\n            }\r\n\r\n            h3 {\r\n                font-size: 20px !important;\r\n                text-align: left\r\n            }\r\n\r\n            .es-header-body h1 a,\r\n            .es-content-body h1 a,\r\n            .es-footer-body h1 a {\r\n                font-size: 30px !important;\r\n                text-align: left\r\n            }\r\n\r\n            .es-header-body h2 a,\r\n            .es-content-body h2 a,\r\n            .es-footer-body h2 a {\r\n                font-size: 24px !important;\r\n                text-align: left\r\n            }\r\n\r\n            .es-header-body h3 a,\r\n            .es-content-body h3 a,\r\n            .es-footer-body h3 a {\r\n                font-size: 20px !important;\r\n                text-align: left\r\n            }\r\n\r\n            .es-menu td a {\r\n                font-size: 14px !important\r\n            }\r\n\r\n            .es-header-body p,\r\n            .es-header-body ul li,\r\n            .es-header-body ol li,\r\n            .es-header-body a {\r\n                font-size: 14px !important\r\n            }\r\n\r\n            .es-content-body p,\r\n            .es-content-body ul li,\r\n            .es-content-body ol li,\r\n            .es-content-body a {\r\n                font-size: 14px !important\r\n            }\r\n\r\n            .es-footer-body p,\r\n            .es-footer-body ul li,\r\n            .es-footer-body ol li,\r\n            .es-footer-body a {\r\n                font-size: 14px !important\r\n            }\r\n\r\n            .es-infoblock p,\r\n            .es-infoblock ul li,\r\n            .es-infoblock ol li,\r\n            .es-infoblock a {\r\n                font-size: 12px !important\r\n            }\r\n\r\n            *[class=\"gmail-fix\"] {\r\n                display: none !important\r\n            }\r\n\r\n            .es-m-txt-c,\r\n            .es-m-txt-c h1,\r\n            .es-m-txt-c h2,\r\n            .es-m-txt-c h3 {\r\n                text-align: center !important\r\n            }\r\n\r\n            .es-m-txt-r,\r\n            .es-m-txt-r h1,\r\n            .es-m-txt-r h2,\r\n            .es-m-txt-r h3 {\r\n                text-align: right !important\r\n            }\r\n\r\n            .es-m-txt-l,\r\n            .es-m-txt-l h1,\r\n            .es-m-txt-l h2,\r\n            .es-m-txt-l h3 {\r\n                text-align: left !important\r\n            }\r\n\r\n            .es-m-txt-r img,\r\n            .es-m-txt-c img,\r\n            .es-m-txt-l img {\r\n                display: inline !important\r\n            }\r\n\r\n            .es-button-border {\r\n                display: block !important\r\n            }\r\n\r\n            a.es-button,\r\n            button.es-button {\r\n                font-size: 18px !important;\r\n                display: block !important;\r\n                border-right-width: 0px !important;\r\n                border-left-width: 0px !important;\r\n                border-top-width: 15px !important;\r\n                border-bottom-width: 15px !important\r\n            }\r\n\r\n            .es-adaptive table,\r\n            .es-left,\r\n            .es-right {\r\n                width: 100% !important\r\n            }\r\n\r\n            .es-content table,\r\n            .es-header table,\r\n            .es-footer table,\r\n            .es-content,\r\n            .es-footer,\r\n            .es-header {\r\n                width: 100% !important;\r\n                max-width: 600px !important\r\n            }\r\n\r\n            .es-adapt-td {\r\n                display: block !important;\r\n                width: 100% !important\r\n            }\r\n\r\n            .adapt-img {\r\n                width: 100% !important;\r\n                height: auto !important\r\n            }\r\n\r\n            .es-m-p0 {\r\n                padding: 0px !important\r\n            }\r\n\r\n            .es-m-p0r {\r\n                padding-right: 0px !important\r\n            }\r\n\r\n            .es-m-p0l {\r\n                padding-left: 0px !important\r\n            }\r\n\r\n            .es-m-p0t {\r\n                padding-top: 0px !important\r\n            }\r\n\r\n            .es-m-p0b {\r\n                padding-bottom: 0 !important\r\n            }\r\n\r\n            .es-m-p20b {\r\n                padding-bottom: 20px !important\r\n            }\r\n\r\n            .es-mobile-hidden,\r\n            .es-hidden {\r\n                display: none !important\r\n            }\r\n\r\n            tr.es-desk-hidden,\r\n            td.es-desk-hidden,\r\n            table.es-desk-hidden {\r\n                width: auto !important;\r\n                overflow: visible !important;\r\n                float: none !important;\r\n                max-height: inherit !important;\r\n                line-height: inherit !important\r\n            }\r\n\r\n            tr.es-desk-hidden {\r\n                display: table-row !important\r\n            }\r\n\r\n            table.es-desk-hidden {\r\n                display: table !important\r\n            }\r\n\r\n            td.es-desk-menu-hidden {\r\n                display: table-cell !important\r\n            }\r\n\r\n            .es-menu td {\r\n                width: 1% !important\r\n            }\r\n\r\n            table.es-table-not-adapt,\r\n            .esd-block-html table {\r\n                width: auto !important\r\n            }\r\n\r\n            table.es-social {\r\n                display: inline-block !important\r\n            }\r\n\r\n            table.es-social td {\r\n                display: inline-block !important\r\n            }\r\n\r\n            .es-desk-hidden {\r\n                display: table-row !important;\r\n                width: auto !important;\r\n                overflow: visible !important;\r\n                max-height: inherit !important\r\n            }\r\n        }\r\n    </style>\r\n</head>\r\n\r\n<body\r\n    style=\"width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0\">\r\n    <div dir=\"ltr\" class=\"es-wrapper-color\" lang=\"vi\" style=\"background-color:#FFFFFF\">\r\n        <!--[if gte mso 9]><v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\"> <v:fill type=\"tile\" color=\"#ffffff\"></v:fill> </v:background><![endif]-->\r\n        <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" role=\"none\"\r\n            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;background-color:#FFFFFF\">\r\n            <tr>\r\n                <td valign=\"top\" style=\"padding:0;Margin:0\">\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#bcb8b1\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\" role=\"none\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"Margin:0;padding-top:20px;padding-bottom:20px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\"\r\n                                                                    style=\"padding:0;Margin:0;display:none\"></td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;border-radius:20px 20px 0 0;width:600px\"\r\n                                    role=\"none\">\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"padding:0;Margin:0;padding-top:40px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            role=\"presentation\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"left\" class=\"es-m-txt-c\"\r\n                                                                    style=\"padding:0;Margin:0;font-size:0px\"><img\r\n                                                                        src=\"https://juctlf.stripocdn.email/content/guids/CABINET_ee77850a5a9f3068d9355050e69c76d26d58c3ea2927fa145f0d7a894e624758/images/group_4076323.png\"\r\n                                                                        alt=\"Confirm email\"\r\n                                                                        style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;border-radius:100px;font-size:12px\"\r\n                                                                        width=\"100\" title=\"Confirm email\" height=\"100\">\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"padding:0;Margin:0;padding-top:20px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            bgcolor=\"#fafafa\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;background-color:#fafafa;border-radius:10px\"\r\n                                                            role=\"presentation\">\r\n                                                            <tr>\r\n                                                                <td align=\"left\" style=\"padding:20px;Margin:0\">\r\n                                                                    <h3\r\n                                                                        style=\"Margin:0;line-height:34px;mso-line-height-rule:exactly;font-family:Imprima, Arial, sans-serif;font-size:28px;font-style:normal;font-weight:bold;color:#2D3142\">\r\n                                                                        Xin chào!</h3>\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        <br>\r\n                                                                    </p>\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho\r\n                                                                        tài khoản của bạn tại RecruitXpress. Để đặt lại\r\n                                                                        mật khẩu, vui lòng truy cập liên kết bên dưới.\r\n                                                                    </p>\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        <br>\r\n                                                                    </p>\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        Nếu bạn không quên mật khẩu vui lòng bỏ qua\r\n                                                                        email này.</p>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\" role=\"none\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;width:600px\">\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"Margin:0;padding-top:30px;padding-bottom:40px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            role=\"presentation\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\" style=\"padding:0;Margin:0\"> <!--[if mso]><a href=\"https://viewstripo.email\" target=\"_blank\" hidden> <v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" esdevVmlButton href=\"https://viewstripo.email\" style=\"height:56px; v-text-anchor:middle; width:520px\" arcsize=\"50%\" stroke=\"f\" fillcolor=\"#7630f3\"> <w:anchorlock></w:anchorlock> <center style='color:#ffffff; font-family:Imprima, Arial, sans-serif; font-size:22px; font-weight:700; line-height:22px; mso-text-raise:1px'>Xác nhận email</center> </v:roundrect></a>\r\n    <![endif]--> <!--[if !mso]><!-- --><span class=\"msohide es-button-border\"\r\n                                                                        style=\"border-style:solid;border-color:#2CB543;background:#7630f3;border-width:0px;display:block;border-radius:30px;width:auto;mso-hide:all\"><a\r\n                                                                            href=\""
                    + link + "\"\r\n                                                                            class=\"es-button msohide\" target=\"_blank\"\r\n                                                                            style=\"mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:22px;padding:15px 20px 15px 20px;display:block;background:#7630f3;border-radius:30px;font-family:Imprima, Arial, sans-serif;font-weight:bold;font-style:normal;line-height:26px;width:auto;text-align:center;mso-padding-alt:0;mso-border-alt:10px solid #7630f3;mso-hide:all;padding-left:5px;padding-right:5px\">Đặt\r\n                                                                            lại mật khẩu</a> </span> <!--<![endif]-->\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"padding:0;Margin:0;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"center\" valign=\"top\"\r\n                                                        style=\"padding:0;Margin:0;width:520px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            role=\"presentation\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"left\" style=\"padding:0;Margin:0\">\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:27px;color:#2D3142;font-size:18px\">\r\n                                                                        Trân trọng,<br><br>RecruitXpress</p>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                            <tr>\r\n                                                                <td align=\"center\"\r\n                                                                    style=\"padding:0;Margin:0;padding-bottom:20px;padding-top:40px;font-size:0\">\r\n                                                                    <table border=\"0\" width=\"100%\" height=\"100%\"\r\n                                                                        cellpadding=\"0\" cellspacing=\"0\"\r\n                                                                        role=\"presentation\"\r\n                                                                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                                        <tr>\r\n                                                                            <td\r\n                                                                                style=\"padding:0;Margin:0;border-bottom:1px solid #666666;background:unset;height:1px;width:100%;margin:0px\">\r\n                                                                            </td>\r\n                                                                        </tr>\r\n                                                                    </table>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#efefef\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#EFEFEF;border-radius:0 0 20px 20px;width:600px\"\r\n                                    role=\"none\">\r\n                                    <tr>\r\n                                        <td class=\"esdev-adapt-off\" align=\"left\"\r\n                                            style=\"Margin:0;padding-top:20px;padding-bottom:20px;padding-left:40px;padding-right:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" class=\"esdev-mso-table\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;width:520px\">\r\n                                                <tr>\r\n                                                    <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" align=\"left\"\r\n                                                            class=\"es-left\" role=\"none\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\" valign=\"top\"\r\n                                                                    style=\"padding:0;Margin:0;width:47px\">\r\n                                                                    <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                                        role=\"presentation\"\r\n                                                                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                                        <tr>\r\n                                                                            <td align=\"center\" class=\"es-m-txt-l\"\r\n                                                                                style=\"padding:0;Margin:0;font-size:0px\">\r\n                                                                                <img src=\"https://juctlf.stripocdn.email/content/guids/CABINET_ee77850a5a9f3068d9355050e69c76d26d58c3ea2927fa145f0d7a894e624758/images/group_4076325.png\"\r\n                                                                                    alt=\"Demo\"\r\n                                                                                    style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;font-size:12px\"\r\n                                                                                    width=\"47\" title=\"Demo\" height=\"47\">\r\n                                                                            </td>\r\n                                                                        </tr>\r\n                                                                    </table>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                    <td style=\"padding:0;Margin:0;width:20px\"></td>\r\n                                                    <td class=\"esdev-mso-td\" valign=\"top\" style=\"padding:0;Margin:0\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-right\"\r\n                                                            align=\"right\" role=\"none\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\" valign=\"top\"\r\n                                                                    style=\"padding:0;Margin:0;width:453px\">\r\n                                                                    <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                                        role=\"presentation\"\r\n                                                                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                                        <tr>\r\n                                                                            <td align=\"center\"\r\n                                                                                style=\"padding:0;Margin:0\">\r\n                                                                                <p\r\n                                                                                    style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:24px;color:#2D3142;font-size:16px\">\r\n                                                                                    Địa chỉ này sẽ hết hạn sau 24h.</p>\r\n                                                                            </td>\r\n                                                                        </tr>\r\n                                                                    </table>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#bcb8b1\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\" role=\"none\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n                                    <tr>\r\n                                        <td align=\"left\"\r\n                                            style=\"Margin:0;padding-left:20px;padding-right:20px;padding-bottom:30px;padding-top:40px\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:560px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"\r\n                                                            role=\"presentation\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\" class=\"es-m-txt-c\"\r\n                                                                    style=\"padding:0;Margin:0;padding-bottom:20px;font-size:0px\">\r\n                                                                    <img src=\"https://juctlf.stripocdn.email/content/guids/CABINET_e0075a83086e72d11f342b3e29a0d40357691795c3b8e8a62708bbb046714fd2/images/recruit2_yRz.png\"\r\n                                                                        alt=\"Logo\"\r\n                                                                        style=\"display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;font-size:12px\"\r\n                                                                        title=\"Logo\" height=\"100\" width=\"238\">\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                            <tr>\r\n                                                                <td align=\"center\"\r\n                                                                    style=\"padding:0;Margin:0;padding-top:20px\">\r\n                                                                    <p\r\n                                                                        style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:'comic sans ms', 'marker felt-thin', arial, sans-serif;line-height:21px;color:#2D3142;font-size:14px\">\r\n                                                                        <a target=\"_blank\" href=\"\"\r\n                                                                            style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#2D3142;font-size:14px\"></a>Copyright\r\n                                                                        © 2023 RecruitXpress<a target=\"_blank\" href=\"\"\r\n                                                                            style=\"-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:underline;color:#2D3142;font-size:14px\"></a>\r\n                                                                    </p>\r\n                                                                </td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                    <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-footer\" align=\"center\" role=\"none\"\r\n                        style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top\">\r\n                        <tr>\r\n                            <td align=\"center\" style=\"padding:0;Margin:0\">\r\n                                <table bgcolor=\"#ffffff\" class=\"es-footer-body\" align=\"center\" cellpadding=\"0\"\r\n                                    cellspacing=\"0\" role=\"none\"\r\n                                    style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;width:600px\">\r\n                                    <tr>\r\n                                        <td align=\"left\" style=\"padding:20px;Margin:0\">\r\n                                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                <tr>\r\n                                                    <td align=\"left\" style=\"padding:0;Margin:0;width:560px\">\r\n                                                        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"none\"\r\n                                                            style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px\">\r\n                                                            <tr>\r\n                                                                <td align=\"center\"\r\n                                                                    style=\"padding:0;Margin:0;display:none\"></td>\r\n                                                            </tr>\r\n                                                        </table>\r\n                                                    </td>\r\n                                                </tr>\r\n                                            </table>\r\n                                        </td>\r\n                                    </tr>\r\n                                </table>\r\n                            </td>\r\n                        </tr>\r\n                    </table>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n    </div>\r\n</body>\r\n\r\n</html>";

                var from = "recruitmentxpressofficial@gmail.com";

                _emailSender.Send(to, subject, html, from); // Assuming Send is asynchronous.

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost("ResetPasswordByEmail")]
        public async Task<IActionResult> ResetPasswordByEmail(string email)
        {
            try
            {
                var user = await _context.Accounts.Where(u => u.Account1 == email).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("Tài khoản không tồn tại.");
                }

                if (user.Status == 0)
                {
                    return StatusCode(403, "Địa chỉ email của bạn chưa được xác thực");
                }

                if (user.Status == 2)
                {
                    return StatusCode(403, "Tài khoản của bạn đã bị khóa!");
                }

                var token = TokenHelper.GenerateRandomToken(64);
                string url = _configuration["Website:ClientUrl"] + "/set-password?token=" + token;
                string subject = "Quên mật khẩu";
                var emailtoken = new EmailToken
                {
                    Token = token,
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddDays(1),
                    IsRevoked = false,
                    IsUsed = false,
                    AccountId = user.AccountId,
                };

                _context.Add(emailtoken);
                await _context.SaveChangesAsync();

                SendEmailResetPassword(user.Account1, subject, url);

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost("ChangePasswordWithToken")]
        public async Task<IActionResult> ResetPassword(string token, string newPassword)
        {
            try
            {
                var emailtoken = await _context.EmailTokens.SingleOrDefaultAsync(t => t.Token == token);
                if (emailtoken != null && (bool)!emailtoken.IsUsed && (bool)!emailtoken.IsRevoked)
                {
                    var user = await _context.Accounts.SingleOrDefaultAsync(t => t.AccountId == emailtoken.AccountId);
                    user.Password = HashHelper.Encrypt(newPassword, _configuration);
                    emailtoken.IsUsed = true;
                    _context.Update(emailtoken);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid Token");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            try
            {
                var emailtoken = await _context.EmailTokens.SingleOrDefaultAsync(t => t.Token == token);
                if (emailtoken != null && (bool)!emailtoken.IsUsed && (bool)!emailtoken.IsRevoked)
                {
                    var user = await _context.Accounts.SingleOrDefaultAsync(t => t.AccountId == emailtoken.AccountId);
                    user.Status = 1;
                    emailtoken.IsUsed = true;
                    _context.Update(emailtoken);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid Token");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> ListAccount()
        {
            return Ok(_context.Accounts.ToList());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model is null)
            {
                return BadRequest("Invalid user request!!!");
            }

            var user = await _context.Accounts.Include(a => a.Role).Include(r => r.Profiles).Include(c => c.CandidateCvs).SingleOrDefaultAsync(u => u.Account1 == model.Username);


            if (user != null)
            {
                if (CheckPassword(user, model) == false)
                {
                    return StatusCode(403, "Sai mật khẩu");
                }

                if (user.Status == 0)
                {
                    return StatusCode(403, "Địa chỉ email của bạn chưa được xác thực");
                }

                if (user.Status == 2)
                {
                    return StatusCode(403, "Tài khoản đã bị khóa!");
                }

                // Retrieve the secret key from appsettings.json
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Audience"],
                    audience: _configuration["Jwt:Issuer"],
                    claims: new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.AccountId.ToString()),
                        new Claim(ClaimTypes.Name, user.Account1),
                        new Claim(ClaimTypes.Role, user.Role.RoleName),

                    },
                    expires: DateTime.Now.AddHours(6),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new
                {
                    Token = tokenString,
                    AccountId = user.AccountId,
                    UserName = user.Account1,
                    RoleId = user.RoleId,
                    IsProfile = (user.Profiles != null && user.Profiles.Any()).ToString().ToLower(),
                    IsCV = (user.CandidateCvs != null && user.CandidateCvs.Any()).ToString().ToLower(),
                });
            }

            return Unauthorized();
        }

        [HttpPost("LoginExpert")]
        public async Task<IActionResult> GrantAccess(AccessModel model)
        {
            if (model is null)
            {
                return BadRequest("Invalid user request!!!");
            }

            var user = await _context.AccessCodes.SingleOrDefaultAsync(u => u.Email == model.email && u.Code.Equals(model.code));

            if (user == null)
            {
                return StatusCode(403, "Sai code đăng nhập");

            }
            else if (user.ExpirationTimestamp < DateTime.Now)
            {
                return StatusCode(403, "Code của bạn đã hết hạn để đăng nhập");
            }
            else
            {

                // Retrieve the secret key from appsettings.json
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Audience"],
                    audience: _configuration["Jwt:Issuer"],
                    claims: new List<Claim>()
                    {
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.Email),
                        new Claim(ClaimTypes.Role, "Expert"),
                    },
                    expires: DateTime.Now.AddHours(6),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new
                {
                    Token = tokenString,
                    Email = user.Email,
                    RoleId = 5,
                    ExamCode = user.ExamCode,
                });
            }

            return Unauthorized();
        }


        private bool CheckPassword(Account? user, LoginModel? login)
        {
            if (user != null && HashHelper.Decrypt(user.Password, _configuration) == login.Password)
            {
                return true;
            }


            else return false;
        }

        [HttpPost("RevokeToken")]
        public async Task<IActionResult> RevokeToken(string token)
        {
            try
            {
                var emailtoken = await _context.EmailTokens.SingleOrDefaultAsync(t => t.Token == token);
                if (emailtoken != null)
                {
                    emailtoken.IsRevoked = true;
                    _context.Update(emailtoken);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid Token");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("auth/google")]
        public ActionResult<string> GoogleAuth(string redirectUrl)
        {
            return _googleService.GetAuthUrl(redirectUrl);
        }

        [HttpGet]
        [Route("/auth/callback")]
        public async Task<IActionResult> Callback()
        {
            string code = HttpContext.Request.Query["code"];
            string scope = HttpContext.Request.Query["scope"];

            //get token method
            var token = await _googleService.GetTokens(code);
            return Ok(token);
        }
    }
}