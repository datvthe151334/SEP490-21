using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IJobApplicationRepository
{
    Task<JobApplication?> UpdateJobApplicationStatus(int jobApplyId, int? accountId, int? status);
    Task<JobApplication?> AutoAddStatus(int jobApplyId);
    Task<JobApplication?> FindJobApplicationAndUpdateStatus(int jobId, int accountId);
}