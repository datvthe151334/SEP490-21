using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.Hub;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly RecruitXpressContext _context;
    private readonly IHubContext<JobApplicationStatusHub> _hubContext;

    public JobApplicationRepository(RecruitXpressContext context, IHubContext<JobApplicationStatusHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<JobApplication?> UpdateJobApplicationStatus(int jobApplyId, int? accountId, int? status)
    {
        try
        {
            var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
            if (detailJob == null || status == null) return detailJob;
            detailJob.Status = status;
            if (accountId != null)
            {
                detailJob.AssignedFor = accountId;
            }
            _context.Update(detailJob);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("StatusChanged", jobApplyId, status);
            return detailJob;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    public async Task<JobApplication?> AutoAddStatus(int jobApplyId)
    {
        try
        {
            var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
            var newStatus = detailJob.Status + 1;
            await _hubContext.Clients.All.SendAsync("StatusChanged", jobApplyId, newStatus);
            return detailJob;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    public async Task<JobApplication?> FindJobApplicationAndUpdateStatus(int jobId, int accountId)
    {
        try
        {
            var profile = await _context.Profiles.Where(p => p.AccountId == accountId).FirstOrDefaultAsync();
             if(profile==null)
            {
                throw new Exception("Không có profile");
            }    
            var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.JobId == jobId && x.ProfileId==profile.ProfileId);
            return await UpdateJobApplicationStatus(detailJob.ApplicationId, null, 5);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

}