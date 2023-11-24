using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IJobPostingManagementRepository
{
    Task<List<JobPosting>> GetListJobPostings();
    Task<JobPosting?> GetJobPosting(int id);
    Task<JobPosting> AddJobPosting(JobPosting jobPosting);
    Task<JobPosting> UpdateJobPostings(int id, JobPosting jobPosting);
    void DeleteJobPosting(int jobId);
}