using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using Org.BouncyCastle.Asn1.Ocsp;
using AutoMapper;
using RecruitXpress_BE.DTO;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/JobApplication/")]
    [ApiController]

    public class JobApplicationController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IMapper _mapper;


        public JobApplicationController(RecruitXpressContext context, IMapper mapper, IEmailTemplateRepository emailTemplateRepository)
        {
            _context = context;
            _mapper = mapper;
            _emailTemplateRepository = emailTemplateRepository;
        }
        [HttpPost("PostJobApplication")]
        public async Task<IActionResult> submitJobApplication(int jobId, int accountId)
        {
            try
            {
                if (accountId == null) return BadRequest("Account is not null");
                var profile = _context.Profiles.FirstOrDefault(x => x.AccountId == accountId);
                var CV = _context.CandidateCvs.FirstOrDefault(x => x.AccountId == accountId);
                if (profile == null)
                {
                    return BadRequest("Hãy cập nhật thông tin cá nhân đầy đủ trước khi nộp hồ sơ");
                }
                if (CV == null)
                {
                    return BadRequest("Hãy cập nhật thông tin CV đầy đủ trước khi nộp hồ sơ ");
                }

                var jobApp = new JobApplication
                {
                    JobId = jobId,
                    ProfileId = profile.ProfileId,
                    TemplateId = CV.TemplateId,
                    Status = 1
                };
                _context.Add(jobApp);
                await _context.SaveChangesAsync();
                return Ok("Nộp hồ sơ thành công");


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("AllJobApplication")]
        public async Task<IActionResult> listJobApplication([FromQuery] JobApplicationRequest request, int? accountId)
        {
            try
            {

                var query = _context.JobApplications
                .Include(q => q.Profile).ThenInclude(x=> x.Schedules).ThenInclude(x => x.ScheduleDetails)
                .Include(q => q.Profile).ThenInclude(x => x.Evaluates)
                .Include(q => q.Profile).ThenInclude(x => x.Schedules)
                .Include(q => q.Profile).ThenInclude(x => x.GeneralTests).ThenInclude(x => x.GeneralTestDetails)
                .Include(q => q.Job)
                .Include(q => q.Template).AsQueryable();

                if (accountId != null)
                {
                    query = query.Where(x => x.AssignedFor == accountId);
                }

                if (request.Location != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Location != null && s.Job.Location.Contains(request.Location));
                }

                if (request.EmploymentType != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.EmploymentType != null && s.Job.EmploymentType.Contains(request.EmploymentType));
                }

                if (request.Industry != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Industry != null && s.Job.Industry.Contains(request.Industry));
                }

                if (request is { MinSalary: not null, MaxSalary: not null })
                {
                    query = query.Where(s => s.Job != null && s.Job.MinSalary >= request.MinSalary && s.Job.MaxSalary <= request.MaxSalary);
                }

                if (request.ApplicationDeadline != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.ApplicationDeadline != null && s.Job.ApplicationDeadline.Equals(request.ApplicationDeadline));
                }
                if (request.Title != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Title != null && s.Job.Title.Equals(request.Title));
                }
                if (request.Company != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Company != null && s.Job.Company.Equals(request.Company));
                }
                if (request.NameCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Name != null && s.Profile.Name.Contains(request.NameCandidate));
                }
                if (request.PhoneCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.PhoneNumber != null && s.Profile.PhoneNumber.Contains(request.PhoneCandidate));
                }
                if (request.EmailCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Email != null && s.Profile.Email.Contains(request.EmailCandidate));
                }
                if (request.Status != null)
                {
                    query = query.Where(s => s.Status != null && s.Status == (request.Status));
                }
                if (request.SortBy != null)
                {
                    switch (request.SortBy)
                    {
                        case "Location":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Location)
                                : query.OrderByDescending(j => j.Job.Location);
                            break;
                        case "Status":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Status)
                                : query.OrderByDescending(j => j.Status);
                            break;
                        case "EmploymentType":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.EmploymentType)
                                : query.OrderByDescending(j => j.Job.EmploymentType);
                            break;
                        case "Industry":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Industry)
                                : query.OrderByDescending(j => j.Job.Industry);
                            break;
                        case "SalaryRange":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.MinSalary)
                                : query.OrderByDescending(j => j.Job.MinSalary);
                            break;
                        case "ApplicationDeadline":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                        case "NameCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Name)
                                : query.OrderByDescending(j => j.Profile.Name);
                            break;
                        case "PhoneCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.PhoneNumber)
                                : query.OrderByDescending(j => j.Profile.PhoneNumber);
                            break;
                        case "EmailCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Email)
                                : query.OrderByDescending(j => j.Profile.Email);
                            break;
                        case "Title":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Title)
                                : query.OrderByDescending(j => j.Job.Title);
                            break;
                        case "Company":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Company)
                                : query.OrderByDescending(j => j.Job.Company);
                            break;
                        case "DatePosted":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.DatePosted)
                                : query.OrderByDescending(j => j.Job.DatePosted);
                            break;
                        default:
                            query = request.OrderByAscending
                                   ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                   : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    query = query.Where(s => s.Profile.Email.Contains(request.SearchString) ||
                     s.Profile.PhoneNumber.Contains(request.SearchString) ||
                     s.Profile.Name.Contains(request.SearchString) ||
                     // s.Job.SalaryRange.Contains(request.SearchString) ||
                     s.Job.Industry.Contains(request.SearchString) ||
                     s.Job.Location.Contains(request.SearchString) ||
                     s.Job.Title.Contains(request.SearchString) ||
                     s.Job.Company.Contains(request.SearchString));

                }
                var totalCount = await query.CountAsync();
                var pageNumber = request.Page > 0 ? request.Page : 1;
                var pageSize = request.Size > 0 ? request.Size : 20;
                var jobApplications = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var jobApplicationDTOs = _mapper.Map<List<JobApplicationDTO>>(jobApplications);
                foreach(var jobApplicationDTO in jobApplicationDTOs)
                {
                   var acc = jobApplicationDTO.AssignedFor;
                         if (acc != null) {
                        var profile = _context.Profiles.FirstOrDefault(x => x.AccountId == acc);
                        if (profile != null)
                        {
                            var profileDTO = new AssignedProfileDTO
                            {
                                accountId = (int)acc,
                                Name = profile.Name,
                            };
                            jobApplicationDTO.AssignedForInfor = profileDTO;
                        }
                        }

                        }
                var response = new ApiResponse<JobApplicationDTO>
                {
                    Items = jobApplicationDTOs,
                    TotalCount = totalCount,
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetSumited")]
        public async Task<IActionResult> getJobApplicationSubmitted([FromQuery] JobApplicationRequest request, int accountId)
        {
            try
            {
                var profileId = 0;
                if (accountId != null)
                {
                    var getAccountId = _context.Profiles.Where(x => x.AccountId == accountId).FirstOrDefault();
                    if (getAccountId != null)
                    {
                        profileId = getAccountId.ProfileId;
                    }
                    else return BadRequest("Khong tim thay du lieu profile cua user nay");
                }
                var query = _context.JobApplications
                .Include(q => q.Profile).ThenInclude(x => x.Schedules).ThenInclude(x => x.ScheduleDetails)
                .Include(q => q.Profile).ThenInclude(x => x.Evaluates)
                .Include(q => q.Profile).ThenInclude(x => x.Schedules)
                .Include(q => q.Profile).ThenInclude(x => x.GeneralTests).ThenInclude(x => x.GeneralTestDetails)
                .Include(q => q.Job)
                .Include(q => q.Template).AsQueryable();

                if (request.Location != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Location != null && s.Job.Location.Contains(request.Location));
                }

                if (request.EmploymentType != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.EmploymentType != null && s.Job.EmploymentType.Contains(request.EmploymentType));
                }

                if (request.Industry != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Industry != null && s.Job.Industry.Contains(request.Industry));
                }

                if (request is { MinSalary: not null, MaxSalary: not null })
                {
                    query = query.Where(s => s.Job != null && s.Job.MinSalary >= request.MinSalary && s.Job.MaxSalary <= request.MaxSalary);
                }

                if (request.ApplicationDeadline != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.ApplicationDeadline != null && s.Job.ApplicationDeadline.Equals(request.ApplicationDeadline));
                }
                if (request.Title != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Title != null && s.Job.Title.Equals(request.Title));
                }
                if (request.Company != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Company != null && s.Job.Company.Equals(request.Company));
                }
                if (request.NameCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Name != null && s.Profile.Name.Contains(request.NameCandidate));
                }
                if (request.PhoneCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.PhoneNumber != null && s.Profile.PhoneNumber.Contains(request.PhoneCandidate));
                }
                if (request.EmailCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Email != null && s.Profile.Email.Contains(request.EmailCandidate));
                }
                if (request.Status != null)
                {
                    query = query.Where(s => s.Status != null && s.Status == (request.Status));
                }
                if (request.SortBy != null)
                {
                    switch (request.SortBy)
                    {
                        case "Location":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Location)
                                : query.OrderByDescending(j => j.Job.Location);
                            break;
                        case "Status":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Status)
                                : query.OrderByDescending(j => j.Status);
                            break;
                        case "EmploymentType":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.EmploymentType)
                                : query.OrderByDescending(j => j.Job.EmploymentType);
                            break;
                        case "Industry":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Industry)
                                : query.OrderByDescending(j => j.Job.Industry);
                            break;
                        case "SalaryRange":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.MinSalary)
                                : query.OrderByDescending(j => j.Job.MinSalary);
                            break;
                        case "ApplicationDeadline":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                        case "NameCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Name)
                                : query.OrderByDescending(j => j.Profile.Name);
                            break;
                        case "PhoneCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.PhoneNumber)
                                : query.OrderByDescending(j => j.Profile.PhoneNumber);
                            break;
                        case "EmailCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Email)
                                : query.OrderByDescending(j => j.Profile.Email);
                            break;
                        case "Title":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Title)
                                : query.OrderByDescending(j => j.Job.Title);
                            break;
                        case "Company":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Company)
                                : query.OrderByDescending(j => j.Job.Company);
                            break;
                        case "DatePosted":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.DatePosted)
                                : query.OrderByDescending(j => j.Job.DatePosted);
                            break;
                        default:
                            query = request.OrderByAscending
                                   ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                   : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    query = query.Where(s => s.Profile.Email.Contains(request.SearchString) ||
                     s.Profile.PhoneNumber.Contains(request.SearchString) ||
                     s.Profile.Name.Contains(request.SearchString) ||
                     // s.Job.SalaryRange.Contains(request.SearchString) ||
                     s.Job.Industry.Contains(request.SearchString) ||
                     s.Job.Location.Contains(request.SearchString) ||
                     s.Job.Title.Contains(request.SearchString) ||
                     s.Job.Company.Contains(request.SearchString));

                }
                var totalCount = await query.CountAsync();
                var pageNumber = request.Page > 0 ? request.Page : 1;
                var pageSize = request.Size > 0 ? request.Size : 20;
                var jobApplications = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var jobApplicationDTOs = _mapper.Map<List<JobApplicationDTO>>(jobApplications);
                foreach (var jobApplicationDTO in jobApplicationDTOs)
                {
                    var acc = jobApplicationDTO.AssignedFor;
                    if (acc != null)
                    {
                        var profile = _context.Profiles.FirstOrDefault(x => x.AccountId == acc);
                        if (profile != null)
                        {
                            var profileDTO = new AssignedProfileDTO
                            {
                                accountId = (int)acc,
                                Name = profile.Name,
                            };
                            jobApplicationDTO.AssignedForInfor = profileDTO;
                        }
                    }

                }
                var response = new ApiResponse<JobApplicationDTO>
                {
                    Items = jobApplicationDTOs,
                    TotalCount = totalCount,
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetDetail")]
        public async Task<IActionResult> jobApplicationDetail(int jobApplyId)
        {
            try
            {
                var detailJob = await _context.JobApplications.Include(x => x.Job).Include(x => x.Template)
                    .FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (detailJob == null)
                {
                    return NotFound("Khong co ket qua");
                }
                return Ok(detailJob);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdatejobApplicationStatus(int jobApplyId, int? accountId, int? Status)
        {
            try
            {
                var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (detailJob == null)
                {
                    return NotFound("Khong co ket qua");
                }
                if (Status != null)
                {
                    detailJob.Status = Status;
                    if (accountId != null)
                    {
                        detailJob.AssignedFor = accountId;
                    }

                    switch (Status)
                    {

                        case 1:
                            _emailTemplateRepository.SendEmailSubmitJob(jobApplyId);
                            break;
                        case 7:
                            _emailTemplateRepository.SendEmailUpdateProfile(jobApplyId);
                            break;
                        case 8:
                            _emailTemplateRepository.SendEmailAccepted(jobApplyId);
                            break;
                        case 9:
                            _emailTemplateRepository.SendEmailCanceled(jobApplyId);
                            break;
                    }

                    _context.Update(detailJob);
                    await _context.SaveChangesAsync();
                    return Ok("Cập nhật trạng thái thành công");
                }

                return Ok(detailJob);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Resubmit")]
        public async Task<IActionResult> ResubmitjobApplication(int jobApplyId, int profileId)
        {
            try
            {
                var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (detailJob == null)
                {
                    return NotFound("Khong tim thay ban ghi submit");
                }
                var account = _context.Profiles.FirstOrDefaultAsync(x => x.ProfileId == profileId);
                if (account == null)
                {
                    return BadRequest("Account khong co profile ");
                }
                var check = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.AccountId == account.Id);
                if (check == null)
                {
                    if (check.TemplateId == detailJob.TemplateId)
                    {
                        return BadRequest("Ban da submit job nay roi, de submit lai voi CV moi hay update CV cua ban");
                    }
                    else
                    {
                        await submitJobApplication(jobApplyId, account.Id);
                        return Ok("Cap nhat CV cho job thanh cong");
                    }
                }
                return Ok("Cap nhat CV cho job thanh cong");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
