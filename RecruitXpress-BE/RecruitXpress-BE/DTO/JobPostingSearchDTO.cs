﻿
namespace RecruitXpress_BE.DTO;

public class JobPostingSearchDTO
{
    public int? JobId { get; set; }
    public string? SearchString { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? Industry { get; set; }
    public double? MinSalary { get; set; }
    public double? MaxSalary { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
    public int? status { get; set; }
    
    public string? SortBy { get; set; }

    private readonly bool? _isSortAscending;

    public bool IsSortAscending
    {
        get => _isSortAscending ?? false;
        init => _isSortAscending = value;
    }

    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}