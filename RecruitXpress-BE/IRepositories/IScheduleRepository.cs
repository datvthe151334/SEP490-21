using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IScheduleRepository
{
    Task<List<Schedule>> GetListSchedules();
    Task<ScheduleResponse> GetListSchedules(int accountId, DateTime? startDate, DateTime? endDate);
    Task<Schedule?> GetSchedule(int id);
    Task<ScheduleDTO> AddSchedule(ScheduleDTO Schedule);
    Task<ScheduleDTO> UpdateSchedules(int id, ScheduleDTO scheduleDTO);
    Task<bool> DeleteSchedule(int scheduleId);
}