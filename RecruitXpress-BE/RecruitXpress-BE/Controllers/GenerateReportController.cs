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
using System.Runtime.Intrinsics.X86;
using System;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/GenerateReport/")]
    [ApiController]

    public class GenerateReportController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public GenerateReportController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("ReportForJob")]
        public async Task<IActionResult> generateReportJob(int? month, int? year)
        {
            try
            {
                if (month == null && year == null)
                {
                    // so luong job trong he thong
                    var countJobNumber = await _context.JobPostings.CountAsync();

                    // so luong job da dong trong he thong
                    var countJobNumberClosed = await _context.JobPostings
                        .Where(x => x.Status == 0)
                        .CountAsync();

                    // so luong job dang hoat dong trong he thong
                    var countJobNumberOpen = await _context.JobPostings
                        .Where(x => x.Status == 1)
                        .CountAsync();

                    // so luong job het han trong he thong
                    var countJobNumberEnded = await _context.JobPostings
                        .Where(x => x.Status == 2)
                        .CountAsync();

                    // so luong nganh nghe cua job trong he thong
                    var industriesNumber = await _context.JobPostings
                        .Select(job => job.Industry)
                        .Distinct()
                        .CountAsync();

                    // so luong cong ty cua job trong he thong
                    var companyNumber = await _context.JobPostings
                        .Select(job => job.Company)
                        .Distinct()
                        .CountAsync();

                    // so luong dia chi cua job trong he thong
                    var locationNumber = await _context.JobPostings
                        .Select(job => job.Location)
                        .Distinct()
                        .CountAsync();

                    // so luong ung vien trong he thong
                    var candidateNumber = await _context.Profiles
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    // so luong ung vien da apply
                    var candidateNumberAppliedJob = await _context.JobApplications
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    // so luong ung vien duoc chap thuan 
                    var acceptedCandidateNumber = await _context.JobApplications
                        .Where(x => x.Status == 6)
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    // so luong ung vien bi tu choi
                    var rejectedCandidateNumber = await _context.JobApplications
                        .Where(x => x.Status == 0)
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    // so luong ung vien dang trong qua trinh apply
                    var onGoingCandidateNumber = await _context.JobApplications
                        .Where(x => x.Status != 0 && x.Status != 6)
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    var responseData = new
                    {
                        JobNumber = countJobNumber,
                        JobNumberClosed = countJobNumberClosed,
                        JobNumberOpen = countJobNumberOpen,
                        JobNumberEnded = countJobNumberEnded,
                        IndustriesNumber = industriesNumber,
                        LocationNumber = locationNumber,
                        CompanyNumber = companyNumber,
                        CandidateNumber = candidateNumber,
                        CandidateNumberAppliedJob = candidateNumberAppliedJob,
                        AcceptedCandidateNumber = acceptedCandidateNumber,
                        RejectedCandidateNumber = rejectedCandidateNumber,
                        OnGoingCandidateNumber = onGoingCandidateNumber

                    };
                    return Ok(responseData);
                }
                else
                {
                    if (month == null && year != null)
                    {
                        // so luong job trong he thong
                        var countJobNumber1 = await _context.JobPostings
                            .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Year == year)
                            .CountAsync();

                        // so luong job da dong trong he thong
                        var countJobNumberClosed1 = await _context.JobPostings
                            .Where(x => x.Status == 0 && x.DatePosted.HasValue && x.DatePosted.Value.Year == year)
                            .CountAsync();

                        var countJobNumberOpen1 = await _context.JobPostings
                            .Where(x => x.Status == 1 && x.DatePosted.HasValue && x.DatePosted.Value.Year == year)
                            .CountAsync();

                        var countJobNumberEnded1 = await _context.JobPostings
                            .Where(x => x.Status == 2 && x.DatePosted.HasValue && x.DatePosted.Value.Year == year)
                            .CountAsync();

                        var industriesNumber1 = await _context.JobPostings
                            .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Year == year)
                            .Select(job => job.Industry)
                            .Distinct()
                            .CountAsync();

                        var companyNumber1 = await _context.JobPostings
                            .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Year == year)
                            .Select(job => job.Company)
                            .Distinct()
                            .CountAsync();

                        var locationNumber1 = await _context.JobPostings
                            .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Year == year)
                            .Select(job => job.Location)
                            .Distinct()
                            .CountAsync();

                        var candidateNumber1 = await _context.Profiles
                            .Select(c => c.ProfileId)
                            .Distinct()
                            .CountAsync();

                        var candidateNumberAppliedJob1 = await _context.JobApplications
                            .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Year == year)
                            .Select(c => c.ProfileId)
                            .Distinct()
                            .CountAsync();

                        var acceptedCandidateNumber1 = await _context.JobApplications
                            .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Year == year && x.Status == 6)
                            .Select(c => c.ProfileId)
                            .Distinct()
                            .CountAsync();

                        var rejectedCandidateNumber1 = await _context.JobApplications
                            .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Year == year && x.Status == 0)
                            .Select(c => c.ProfileId)
                            .Distinct()
                            .CountAsync();

                        var onGoingCandidateNumber1 = await _context.JobApplications
                            .Where(x => x.Status != 0 && x.Status != 6 && x.CreatedAt.HasValue && x.CreatedAt.Value.Year == year)
                            .Select(c => c.ProfileId)
                            .Distinct()
                            .CountAsync();

                        var responseData1 = new
                        {
                            JobNumber = countJobNumber1,
                            JobNumberClosed = countJobNumberClosed1,
                            JobNumberOpen = countJobNumberOpen1,
                            JobNumberEnded = countJobNumberEnded1,
                            IndustriesNumber = industriesNumber1,
                            LocationNumber = locationNumber1,
                            CompanyNumber = companyNumber1,
                            CandidateNumber = candidateNumber1,
                            CandidateNumberAppliedJob = candidateNumberAppliedJob1,
                            AcceptedCandidateNumber = acceptedCandidateNumber1,
                            RejectedCandidateNumber = rejectedCandidateNumber1,
                            OnGoingCandidateNumber = onGoingCandidateNumber1
                        };

                        return Ok(responseData1);

                    }
                    //so luong job trong he thong
                    var countJobNumber = await _context.JobPostings
                    .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month && x.DatePosted.Value.Year == year)
                    .CountAsync();

                    //so luong job da dong trong he thong
                    var countJobNumberClosed = await _context.JobPostings
                    .Where(x => x.Status == 0 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                        x.DatePosted.Value.Year == year)
                    .CountAsync();

                    var countJobNumberOpen = await _context.JobPostings
                        .Where(x => x.Status == 1 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                                    x.DatePosted.Value.Year == year)
                        .CountAsync();

                    var countJobNumberEnded = await _context.JobPostings
                        .Where(x => x.Status == 2 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                                    x.DatePosted.Value.Year == year)
                        .CountAsync();

                    var industriesNumber = await _context.JobPostings
                        .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                                    x.DatePosted.Value.Year == year)
                        .Select(job => job.Industry)
                        .Distinct()
                        .CountAsync();

                    var companyNumber = await _context.JobPostings
                        .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                                    x.DatePosted.Value.Year == year)
                        .Select(job => job.Company)
                        .Distinct()
                        .CountAsync();

                    var locationNumber = await _context.JobPostings
                        .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                                    x.DatePosted.Value.Year == year)
                        .Select(job => job.Location)
                        .Distinct()
                        .CountAsync();

                    var candidateNumber = await _context.Profiles
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    var candidateNumberAppliedJob = await _context.JobApplications
                        .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                                    x.CreatedAt.Value.Year == year)
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    var acceptedCandidateNumber = await _context.JobApplications
                        .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                                    x.CreatedAt.Value.Year == year && x.Status == 6)
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    var rejectedCandidateNumber = await _context.JobApplications
                        .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                                    x.CreatedAt.Value.Year == year && x.Status == 0)
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    var onGoingCandidateNumber = await _context.JobApplications
                        .Where(x => x.Status != 0 && x.Status != 6 && x.CreatedAt.HasValue &&
                                    x.CreatedAt.Value.Month == month && x.CreatedAt.Value.Year == year)
                        .Select(c => c.ProfileId)
                        .Distinct()
                        .CountAsync();

                    var responseData = new
                    {
                        JobNumber = countJobNumber,
                        JobNumberClosed = countJobNumberClosed,
                        JobNumberOpen = countJobNumberOpen,
                        JobNumberEnded = countJobNumberEnded,
                        IndustriesNumber = industriesNumber,
                        LocationNumber = locationNumber,
                        CompanyNumber = companyNumber,
                        CandidateNumber = candidateNumber,
                        CandidateNumberAppliedJob = candidateNumberAppliedJob,
                        AcceptedCandidateNumber = acceptedCandidateNumber,
                        RejectedCandidateNumber = rejectedCandidateNumber,
                        OnGoingCandidateNumber = onGoingCandidateNumber

                    };
                    return Ok(responseData);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ReportForEvaluate")]
        public async Task<IActionResult> generateReportEvaluate(int profileId)
        {
            try
            {
                if (profileId == null)
                {
                    return BadRequest("Khong co du lieu");
                }

                var allEvaluate = await _context.Evaluates.Where(x => x.ProfileId == profileId && x.Status == 1).ToListAsync();

                //evaluate co diem >=5
                var allEvaluateGood = _context.Evaluates.Where(x => x.ProfileId == profileId && x.Status == 1 && x.Mark >= 5).Count();

                //evaluate co diem <5
                var totalEvaluateNotGood = _context.Evaluates.Where(x => x.ProfileId == profileId && x.Status == 1 && x.Mark < 5).Count();

                var allPoint = await _context.Evaluates.Where(x => x.ProfileId == profileId && x.Status == 1).Select(x => x.Mark).ToArrayAsync();
                var avgPoint = allPoint.Average();
                var responseData = new
                {
                    allEvaluate = allEvaluate,
                    totalEvaluate = allEvaluate.Count,
                    avgPoint = avgPoint,
                    totalGoodEvaluate = allEvaluateGood,
                    totalEvaluateNotGood = totalEvaluateNotGood

                };

                return Ok(responseData);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ReportForJobLast4Month")]
        public async Task<IActionResult> generateReportJobLast4Month()
        {
            try
            {
                    var month = DateTime.Today.Month;
                    var year = DateTime.Today.Year;
                     if (month < 4)
                        {
                            year = year - 1;
                        }
                    int[] monthArray;
                    if(month == 3)
                {
                    monthArray = new int[] { 12, 1, 2, 3 };

                } else if(month == 2)
                {
                    monthArray = new int[] { 11,12, 1, 2};
                }
                else if (month == 1)
                {
                    monthArray = new int[] { 10,11, 12, 1};
                }
                else monthArray = new int[] { month -3, month - 2, month - 1, month };
                List<int> jobNumber = new List<int>();
                List<int> JobNumberClosed = new List<int>();
                List<int> JobNumberOpen = new List<int>();
                List<int> JobNumberEnded = new List<int>();
                List<int> industriesNumber = new List<int>();
                List<int> locationNumber = new List<int>();
                List<int> companyNumber = new List<int>();
                List<int> candidateNumber = new List<int>();
                List<int> candidateNumberAppliedJob = new List<int>();
                List<int> acceptedCandidateNumber = new List<int>();
                List<int> rejectdCandidateNumber = new List<int>();
                List<int> onGoingCandidateNumber = new List<int>();
                foreach (var month1 in monthArray)
                {
                   var data = await getDataEachMonth(month1, year);
                    if(data != null)
                    {
                        jobNumber.Add(data.JobNumber);
                        JobNumberClosed.Add(data.JobNumberClosed);
                        JobNumberOpen.Add(data.JobNumberOpen);
                        JobNumberEnded.Add(data.JobNumberEnded);
                        industriesNumber.Add(data.IndustriesNumber);
                        locationNumber.Add(data.LocationNumber);
                        companyNumber.Add(data.CompanyNumber);
                        candidateNumber.Add(data.CandidateNumber);
                        candidateNumberAppliedJob.Add(data.CandidateNumberAppliedJob);
                        acceptedCandidateNumber.Add(data.CandidateNumberAppliedJob);
                        rejectdCandidateNumber.Add(data.CandidateNumberAppliedJob);
                        onGoingCandidateNumber.Add(data.CandidateNumberAppliedJob);
                    }
                }

                var responseData = new
                    {
                    JobNumber = jobNumber,
                    JobNumberClosed = JobNumberClosed,
                    JobNumberOpen = JobNumberOpen,
                    JobNumberEnded = JobNumberEnded,
                    IndustriesNumber = industriesNumber,
                    LocationNumber = locationNumber,
                    CompanyNumber = companyNumber,
                    CandidateNumber = candidateNumber,
                    CandidateNumberAppliedJob = candidateNumberAppliedJob,
                    AcceptedCandidateNumber = acceptedCandidateNumber,
                    RejectedCandidateNumber = rejectdCandidateNumber,
                    OnGoingCandidateNumber = onGoingCandidateNumber

                };
                    return Ok(responseData);
                
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private async Task<YourObject> getDataEachMonth(int month, int year)
        {
            var countJobNumber = await _context.JobPostings
                   .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month && x.DatePosted.Value.Year == year)
                   .CountAsync();

            //so luong job da dong trong he thong
            var countJobNumberClosed = await _context.JobPostings
            .Where(x => x.Status == 0 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                x.DatePosted.Value.Year == year)
            .CountAsync();

            var countJobNumberOpen = await _context.JobPostings
                .Where(x => x.Status == 1 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                            x.DatePosted.Value.Year == year)
                .CountAsync();

            var countJobNumberEnded = await _context.JobPostings
                .Where(x => x.Status == 2 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                            x.DatePosted.Value.Year == year)
                .CountAsync();

            var industriesNumber = await _context.JobPostings
                .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                            x.DatePosted.Value.Year == year)
                .Select(job => job.Industry)
                .Distinct()
                .CountAsync();

            var companyNumber = await _context.JobPostings
                .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                            x.DatePosted.Value.Year == year)
                .Select(job => job.Company)
                .Distinct()
                .CountAsync();

            var locationNumber = await _context.JobPostings
                .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                            x.DatePosted.Value.Year == year)
                .Select(job => job.Location)
                .Distinct()
                .CountAsync();

            var candidateNumber = await _context.Profiles
                .Select(c => c.ProfileId)
                .Distinct()
                .CountAsync();

            var candidateNumberAppliedJob = await _context.JobApplications
                .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                            x.CreatedAt.Value.Year == year)
                .Select(c => c.ProfileId)
                .Distinct()
                .CountAsync();

            var acceptedCandidateNumber = await _context.JobApplications
                .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                            x.CreatedAt.Value.Year == year && x.Status == 6)
                .Select(c => c.ProfileId)
                .Distinct()
                .CountAsync();

            var rejectedCandidateNumber = await _context.JobApplications
                .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                            x.CreatedAt.Value.Year == year && x.Status == 0)
                .Select(c => c.ProfileId)
                .Distinct()
                .CountAsync();

            var onGoingCandidateNumber = await _context.JobApplications
                .Where(x => x.Status != 0 && x.Status != 6 && x.CreatedAt.HasValue &&
                            x.CreatedAt.Value.Month == month && x.CreatedAt.Value.Year == year)
                .Select(c => c.ProfileId)
                .Distinct()
                .CountAsync();

            var responseData = new YourObject
            {
                JobNumber = countJobNumber,
                JobNumberClosed = countJobNumberClosed,
                JobNumberOpen = countJobNumberOpen,
                JobNumberEnded = countJobNumberEnded,
                IndustriesNumber = industriesNumber,
                LocationNumber = locationNumber,
                CompanyNumber = companyNumber,
                CandidateNumber = candidateNumber,
                CandidateNumberAppliedJob = candidateNumberAppliedJob,
                AcceptedCandidateNumber = acceptedCandidateNumber,
                RejectedCandidateNumber = rejectedCandidateNumber,
                OnGoingCandidateNumber = onGoingCandidateNumber

            };
            return responseData;
        }

        // Define YourObject class as needed
        public class YourObject
        {
            public int JobNumber { get; set; }
            public int JobNumberClosed { get; set; }
            public int JobNumberOpen { get; set; }
            public int JobNumberEnded { get; set; }
            public int IndustriesNumber { get; set; }
            public int LocationNumber { get; set; }
            public int CompanyNumber { get; set; }
            public int CandidateNumber { get; set; }
            public int CandidateNumberAppliedJob { get; set; }
            public int AcceptedCandidateNumber { get; set; }
            public int RejectedCandidateNumber { get; set; }
            public int OnGoingCandidateNumber { get; set; }
        }

    }
}
