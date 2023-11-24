using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly RecruitXpressContext _context;
    private readonly IJobApplicationRepository _jobApplicationRepository;

    public ScheduleRepository(RecruitXpressContext context, IJobApplicationRepository jobApplicationRepository)
    {
        _context = context;
        _jobApplicationRepository = jobApplicationRepository;
    }


    public async Task<List<Schedule>> GetListSchedules()
    {
        return null;
    }

    public async Task<ScheduleResponse> GetListSchedules(int accountId, DateTime? startDate,
        DateTime? endDate)
    {
        try
        {
            startDate ??= DateTime.Parse("1/1/1753");
            endDate ??= DateTime.Parse("31/12/9999");
            var account = _context.Accounts
                .SingleOrDefault(a => a.AccountId == accountId);
            if (account == null)
            {
                throw new Exception("Account is not exxist!");
            }

            var query = _context.Schedules
                .Include(s => s.HumanResource)
                .Include(s => s.ScheduleDetails)
                .ThenInclude(sd => sd.Candidate)
                .ThenInclude(ja => ja.Profile)
                .Include(s => s.Interviewers)
                .ThenInclude(i => i.InterviewerNavigation)
                .Select(s => new ScheduleDTO()
                {
                    ScheduleId = s.ScheduleId,
                    HumanResource = new Profile()
                    {
                        ProfileId = s.HumanResource.ProfileId,
                        AccountId = s.HumanResource.AccountId,
                        Name = s.HumanResource.Name
                    },
                    Interviewers = s.Interviewers.Select(i => new Interviewer
                    {
                        InterviewerNavigation = new Profile()
                        {
                            ProfileId = i.InterviewerNavigation.ProfileId,
                            AccountId = i.InterviewerNavigation.AccountId,
                            Name = i.InterviewerNavigation.Name
                        }
                    }).ToList(),
                    ScheduleDetails = s.ScheduleDetails.Where(sd => sd.StartDate > startDate && sd.EndDate < endDate)
                        .ToList()
                })
                .Where(s => s.ScheduleDetails.Count > 0)
                .AsQueryable();
            switch (account.RoleId)
            {
                case Constant.ROLE.INTERVIEWER:
                    query = query.Where(s =>
                        s.Interviewers.Select(i => i.InterviewerNavigation.AccountId).Contains(accountId));
                    break;
                case Constant.ROLE.CANDIDATE:
                    query = query.Where(s =>
                        s.ScheduleDetails.Select(sd => sd.Candidate.Profile.AccountId).Contains(accountId));
                    break;
                default:
                    query = query.Where(s => s.HumanResource.AccountId == accountId);
                    break;
            }

            var scheduleDTOResult = await query.ToListAsync();
            var scheduleAdditionDataResult = new List<ScheduleAdditionDataYear>();

            foreach (var scheduleDto in scheduleDTOResult)
            {
                var scheduleAdditionDataDTO = new ScheduleAdditionDataDTO()
                {
                    HumanResourceName = scheduleDto.HumanResource.Name,
                    InterviewerName = scheduleDto.Interviewers.Select(i => i.InterviewerNavigation.Name).ToList()
                };
                foreach (var scheduleDetail in scheduleDto.ScheduleDetails)
                {
                    scheduleAdditionDataDTO.CandidateName = scheduleDetail.Candidate.Profile.Name;
                    scheduleAdditionDataDTO.type = scheduleDetail.ScheduleType ??
                                                   (scheduleAdditionDataDTO.InterviewerName.Count > 0 ? 1 : 2);
                    var scheduleDate = scheduleDetail.StartDate.Value;
                    if (!scheduleAdditionDataResult.Exists(result => result.Year == scheduleDate.Year))
                    {
                        scheduleAdditionDataResult.Add(new ScheduleAdditionDataYear()
                        {
                            Year = scheduleDate.Year,
                            ScheduleAdditionDataMonths = new List<ScheduleAdditionDataMonth>()
                        });
                    }

                    var scheduleAdditionDataYear =
                        scheduleAdditionDataResult.Find(result => result.Year == scheduleDate.Year);

                    if (scheduleAdditionDataYear != null &&
                        !scheduleAdditionDataYear.ScheduleAdditionDataMonths.Exists(result =>
                            result.Month == scheduleDate.Month))
                    {
                        scheduleAdditionDataYear.ScheduleAdditionDataMonths.Add(new ScheduleAdditionDataMonth()
                        {
                            Month = scheduleDate.Month,
                            ScheduleAdditionDataDays = new List<ScheduleAdditionDataDay>()
                        });
                    }

                    var scheduleAdditionDataMonth =
                        scheduleAdditionDataYear?.ScheduleAdditionDataMonths.Find(result =>
                            result.Month == scheduleDate.Month);

                    if (scheduleAdditionDataMonth != null &&
                        !scheduleAdditionDataMonth.ScheduleAdditionDataDays.Exists(result =>
                            result.Day == scheduleDate.Day))
                    {
                        scheduleAdditionDataMonth.ScheduleAdditionDataDays.Add(new ScheduleAdditionDataDay()
                        {
                            Day = scheduleDate.Day,
                            ScheduleAdditionDataDTOs = new List<ScheduleAdditionDataDTO>()
                        });
                    }

                    var scheduleAdditionDataDay =
                        scheduleAdditionDataMonth?.ScheduleAdditionDataDays.Find(result =>
                            result.Day == scheduleDate.Day);
                    scheduleAdditionDataDay?.ScheduleAdditionDataDTOs.Add(scheduleAdditionDataDTO);
                }
            }

            return new ScheduleResponse()
            {
                ScheduleAdditionData = scheduleAdditionDataResult
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<Schedule?> GetSchedule(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ScheduleDTO> AddSchedule(ScheduleDTO scheduleDTO)
    {
        try
        {
            var hrProfile = await _context.Profiles.Where(p => p.AccountId == scheduleDTO.HumanResourceId)
                .FirstOrDefaultAsync();
            if (hrProfile == null)
            {
                throw new Exception("Human Resource is not exist!");
            }

            var schedule = new Schedule()
            {
                HumanResourceId = hrProfile.ProfileId,
                Status = scheduleDTO.Status,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                CreatedBy = scheduleDTO.CreatedBy,
                UpdatedBy = scheduleDTO.UpdatedBy
            };
            _context.Entry(schedule).State = EntityState.Added;
            await _context.SaveChangesAsync();

            foreach (var interviewer in scheduleDTO.Interviewers)
            {
                var interviewerProfile = await _context.Profiles.Where(p => p.AccountId == interviewer.InterviewerId)
                    .FirstOrDefaultAsync();
                if (interviewerProfile == null)
                {
                    throw new Exception("Interviewer is not exist!");
                }

                interviewer.ScheduleId = schedule.ScheduleId;
                interviewer.InterviewerId = interviewerProfile.ProfileId;
                if (!_context.Interviewers.Any(i =>
                        i.ScheduleId == interviewer.ScheduleId && i.InterviewerId == interviewerProfile.ProfileId))
                {
                    _context.Entry(interviewer).State = EntityState.Added;
                }
            }

            foreach (var scheduleDetail in scheduleDTO.ScheduleDetails)
            {
                var candidateApplication = await _context.JobApplications
                    .Include(ja => ja.Profile)
                    .Where(ja => ja.Profile.AccountId == scheduleDetail.CandidateId).FirstOrDefaultAsync();
                if (candidateApplication == null)
                {
                    throw new Exception("Candidate is not exist!");
                }

                var scheduleDetailEntity = new ScheduleDetail
                {
                    ScheduleId = schedule.ScheduleId,
                    CandidateId = candidateApplication.ApplicationId,
                    Status = scheduleDetail.Status,
                    ScheduleType = scheduleDetail.ScheduleType,
                    StartDate = scheduleDetail.StartDate,
                    EndDate = scheduleDetail.EndDate,
                    Note = scheduleDetail.Note,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    CreatedBy = scheduleDTO.CreatedBy,
                    UpdatedBy = scheduleDTO.UpdatedBy
                };
                _context.Entry(scheduleDetailEntity).State = EntityState.Added;
                _jobApplicationRepository.UpdateJobApplicationStatus(candidateApplication.ApplicationId,
                    scheduleDetail.CandidateId, scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.EXAM ? 6 : 2);
            }

            await _context.SaveChangesAsync();
            return scheduleDTO;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ScheduleDTO> UpdateSchedules(int id, ScheduleDTO scheduleDTO)
    {
        try
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
            {
                throw new Exception("Schedule not found!");
            }

            var hrProfile = await _context.Profiles.Where(p => p.AccountId == scheduleDTO.HumanResourceId)
                .FirstOrDefaultAsync();
            if (hrProfile == null)
            {
                throw new Exception("Human Resource is not exist!");
            }

            schedule.HumanResourceId = hrProfile.ProfileId;
            schedule.Status = scheduleDTO.Status;
            schedule.UpdatedTime = DateTime.Now;
            schedule.UpdatedBy = scheduleDTO.UpdatedBy;

            _context.Entry(schedule).State = EntityState.Modified;

            foreach (var interviewer in scheduleDTO.Interviewers)
            {
                var interviewerProfile = await _context.Profiles.Where(p => p.AccountId == interviewer.InterviewerId)
                    .FirstOrDefaultAsync();
                if (interviewerProfile == null)
                {
                    throw new Exception("Interviewer is not exist!");
                }

                interviewer.ScheduleId = schedule.ScheduleId;
                interviewer.InterviewerId = interviewerProfile.ProfileId;
                _context.Entry(interviewer).State = _context.Interviewers.Any(i =>
                    i.ScheduleId == interviewer.ScheduleId && i.InterviewerId == interviewerProfile.ProfileId)
                    ? EntityState.Modified
                    : EntityState.Added;
            }

            foreach (var scheduleDetail in scheduleDTO.ScheduleDetails)
            {
                var candidateApplication = await _context.JobApplications
                    .Include(ja => ja.Profile)
                    .Where(ja => ja.Profile.AccountId == scheduleDetail.CandidateId).FirstOrDefaultAsync();
                if (candidateApplication == null)
                {
                    throw new Exception("Candidate is not exist!");
                }

                var scheduleDetailEntity = await _context.ScheduleDetails.Where(sd =>
                        sd.ScheduleId == schedule.ScheduleId && sd.CandidateId == candidateApplication.ApplicationId)
                    .FirstOrDefaultAsync();

                if (scheduleDetailEntity == null)
                {
                    scheduleDetailEntity = new ScheduleDetail
                    {
                        ScheduleId = schedule.ScheduleId,
                        CandidateId = candidateApplication.ApplicationId,
                        Status = scheduleDetail.Status,
                        ScheduleType = scheduleDetail.ScheduleType,
                        StartDate = scheduleDetail.StartDate,
                        EndDate = scheduleDetail.EndDate,
                        Note = scheduleDetail.Note,
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        CreatedBy = scheduleDTO.CreatedBy,
                        UpdatedBy = scheduleDTO.UpdatedBy
                    };
                    _context.Entry(scheduleDetailEntity).State = EntityState.Added;
                    _jobApplicationRepository.UpdateJobApplicationStatus(candidateApplication.ApplicationId,
                        scheduleDetail.CandidateId, scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.INTERVIEW ? 6 : 2);
                }
                else
                {
                    scheduleDetailEntity.UpdatedTime = DateTime.Now;
                    scheduleDetailEntity.Status = scheduleDetail.Status;
                    scheduleDetailEntity.ScheduleType = scheduleDetail.ScheduleType;
                    scheduleDetailEntity.StartDate = scheduleDetail.StartDate;
                    scheduleDetailEntity.EndDate = scheduleDetail.EndDate;
                    scheduleDetailEntity.Note = scheduleDetail.Note;
                    scheduleDetailEntity.UpdatedBy = scheduleDetail.UpdatedBy;
                    scheduleDetailEntity.UpdatedTime = DateTime.Now;
                    _context.Entry(scheduleDetail).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
            return scheduleDTO;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteSchedule(int scheduleId)
    {
        var schedule = await _context.Schedules.FindAsync(scheduleId);
        if (schedule == null)
        {
            return false;
        }

        var scheduleDetails = await _context.ScheduleDetails.Where(sd => sd.ScheduleId == scheduleId).ToListAsync();
        var interviewers = await _context.Interviewers.Where(i => i.ScheduleId == scheduleId).ToListAsync();
        foreach (var scheduleDetail in scheduleDetails)
        {
            _context.Entry(scheduleDetail).State = EntityState.Deleted;
        }

        foreach (var interviewer in interviewers)
        {
            _context.Entry(interviewer).State = EntityState.Deleted;
        }

        _context.Entry(schedule).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }
}