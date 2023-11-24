using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IMaritalStatusRepository
{
    Task<List<MaritalStatus>> GetListMaritalStatus();
    Task<MaritalStatus?> GetMaritalStatus(int id);
    Task<MaritalStatus> AddMaritalStatus(MaritalStatus maritalStatus);
    Task<MaritalStatus> UpdateMaritalStatus(int id, MaritalStatus maritalStatus);
    void DeleteMaritalStatus(int jobId);
}