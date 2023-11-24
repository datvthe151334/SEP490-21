using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO;

public class ScheduleDTO
{
    public ScheduleDTO()
    {
        Interviewers = new HashSet<Interviewer>();
        ScheduleDetails = new HashSet<ScheduleDetail>();
    }

    public int ScheduleId { get; set; }
    public int? HumanResourceId { get; set; }
    public int? Status { get; set; }
    public DateTime? CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public virtual Profile? HumanResource { get; set; }
    public virtual ICollection<Interviewer> Interviewers { get; set; }
    public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
    // public Dictionary<int, Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>> ScheduleAdditionData { get; set; }
}

public class ScheduleResponse
{
    public List<ScheduleAdditionDataYear> ScheduleAdditionData { get; set; }
}

public class ScheduleAdditionDataDTO
{
    public string HumanResourceName { get; set; }
    public string CandidateName { get; set; }
    public List<string?> InterviewerName { get; set; }
    public int type { get; set; }
    public string content { get; set; }
}

public class ScheduleAdditionDataYear
{
    public int Year { get; set; }
    public List<ScheduleAdditionDataMonth> ScheduleAdditionDataMonths { get; set; }
}

public class ScheduleAdditionDataMonth
{
    public int Month { get; set; }
    public List<ScheduleAdditionDataDay> ScheduleAdditionDataDays { get; set; }
}

public class ScheduleAdditionDataDay
{
    public int Day { get; set; }
    public List<ScheduleAdditionDataDTO> ScheduleAdditionDataDTOs { get; set; }
}