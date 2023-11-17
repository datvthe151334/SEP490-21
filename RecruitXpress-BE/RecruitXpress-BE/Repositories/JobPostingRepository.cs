﻿using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using Constant = RecruitXpress_BE.Helper.Constant;

namespace RecruitXpress_BE.Repositories;

public class JobPostingRepository : IJobPostingRepository
{
    private readonly RecruitXpressContext _context;

    public JobPostingRepository(RecruitXpressContext context)
    {
        _context = context;
    }

    public async Task<List<JobPosting>> GetListJobPostings()
        => await _context.JobPostings.ToListAsync();

    public async Task<List<JobPostingDTO>> GetListJobPostings(string? searchString, string? sortBy,
        bool? isSortAscending, int? accountId, int page, int size)
    {
        var jobPostings = GetAdvancedSearchJobPostingQuery(
            new JobPostingSearchDTO()
            {
                SearchString = searchString,
                SortBy = sortBy,
                IsSortAscending = isSortAscending == true
            },
            accountId
        );
        return await jobPostings
            .Select(jobPosting => new JobPostingDTO()
            {
                JobId = jobPosting.JobId,
                Title = jobPosting.Title,
                Company = jobPosting.Company,
                Location = jobPosting.Location,
                EmploymentType = jobPosting.EmploymentType,
                Industry = jobPosting.Industry,
                ApplicationDeadline = jobPosting.ApplicationDeadline,
                Requirements = jobPosting.Requirements,
                DatePosted = jobPosting.DatePosted,
                Status = jobPosting.Status,
                IsPreferred = jobPosting.WishLists.Any(w => w.AccountId == accountId),
                TotalCount = jobPostings.Count()
            })
            .Skip((page - 1) * size).Take(size)
            .ToListAsync();
    }

    public async Task<List<JobPostingDTO>> GetListJobPostingAdvancedSearch(JobPostingSearchDTO jobPostingSearchDto, int? accountId,
        int page, int size)
    {
        var query = GetAdvancedSearchJobPostingQuery(jobPostingSearchDto, accountId);
        return await query
            .Select(jobPosting => new JobPostingDTO()
            {
                JobId = jobPosting.JobId,
                Title = jobPosting.Title,
                Company = jobPosting.Company,
                Location = jobPosting.Location,
                EmploymentType = jobPosting.EmploymentType,
                Industry = jobPosting.Industry,
                ApplicationDeadline = jobPosting.ApplicationDeadline,
                Requirements = jobPosting.Requirements,
                DatePosted = jobPosting.DatePosted,
                Status = jobPosting.Status,
                IsPreferred = jobPosting.WishLists.Any(w => w.AccountId == accountId),
                TotalCount = query.Count()
            })
            .Skip((page - 1) * size).Take(size)
            .ToListAsync();
    }

    public async Task<JobPostingDTO?> GetJobPosting(int id, int? accountId)
    {
        var jobPosting = await GetAdvancedSearchJobPostingQuery(
            new JobPostingSearchDTO() { JobId = id },
            accountId
        ).SingleOrDefaultAsync();
        if (jobPosting == null) return null;
        return  new JobPostingDTO()
        {
            JobId = jobPosting.JobId,
            Title = jobPosting.Title,
            Company = jobPosting.Company,
            Location = jobPosting.Location,
            EmploymentType = jobPosting.EmploymentType,
            Industry = jobPosting.Industry,
            ApplicationDeadline = jobPosting.ApplicationDeadline,
            Requirements = jobPosting.Requirements,
            DatePosted = jobPosting.DatePosted,
            Status = jobPosting.Status,
            IsPreferred = jobPosting.WishLists.Any(w => w.AccountId == accountId)
        };;

        return null;
    }

    public async Task<JobPosting> AddJobPosting(JobPosting jobPosting)
    {
        try
        {
            _context.Entry(jobPosting).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return jobPosting;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<JobPosting> UpdateJobPostings(int id, JobPosting jobPosting)
    {
        jobPosting.JobId = id;
        _context.Entry(jobPosting).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            return jobPosting;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteJobPosting(int jobId)
    {
        var jobPosting = await _context.JobPostings.FindAsync(jobId);
        if (jobPosting == null)
        {
            return false;
        }

        _context.Entry(jobPosting).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }

    public IQueryable<JobPosting> GetAdvancedSearchJobPostingQuery(JobPostingSearchDTO searchDto, int? accountId)
    {
        var query = _context.JobPostings.Where(j => j.Status == Constant.ENTITY_STATUS.ACTIVE).AsQueryable();

        if (accountId != null)
        {
            query = query.Include(j => j.WishLists);
        }
        
        if (searchDto.JobId != null)
        {
            query = query.Where(j =>
                j.JobId == searchDto.JobId);
        }
        
        if (!string.IsNullOrEmpty(searchDto.SearchString))
        {
            query = query.Where(j =>
                j.Title.Contains(searchDto.SearchString) || j.Description.Contains(searchDto.SearchString));
        }

        if (!string.IsNullOrEmpty(searchDto.Location))
        {
            query = query.Where(j => j.Location.Contains(searchDto.Location));
        }

        if (!string.IsNullOrEmpty(searchDto.EmploymentType))
        {
            query = query.Where(j => j.EmploymentType == searchDto.EmploymentType);
        }

        if (!string.IsNullOrEmpty(searchDto.Industry))
        {
            query = query.Where(j => j.Industry == searchDto.Industry);
        }

        // if (!string.IsNullOrEmpty(searchDto.SalaryRange))
        // {
        //     var salaryRange = searchDto.SalaryRange.Split("-");
        //     query = query.Where(j => j.MinSalary >= double.Parse(salaryRange[0]) && j.MaxSalary <= double.Parse(salaryRange[1]));
        // }

        if (searchDto.ApplicationDeadline.HasValue)
        {
            query = query.Where(j => j.ApplicationDeadline <= searchDto.ApplicationDeadline);
        }

        if (!string.IsNullOrEmpty(searchDto.SortBy))
        {
            query = searchDto.SortBy switch
            {
                "Location" => searchDto.IsSortAscending
                    ? query.OrderBy(j => j.Location)
                    : query.OrderByDescending(j => j.Location),
                "EmploymentType" => searchDto.IsSortAscending
                    ? query.OrderBy(j => j.EmploymentType)
                    : query.OrderByDescending(j => j.EmploymentType),
                "Industry" => searchDto.IsSortAscending
                    ? query.OrderBy(j => j.Industry)
                    : query.OrderByDescending(j => j.Industry),
                "ApplicationDeadline" => searchDto.IsSortAscending
                    ? query.OrderBy(j => j.ApplicationDeadline)
                    : query.OrderByDescending(j => j.ApplicationDeadline),
                _ => searchDto.IsSortAscending ? query.OrderBy(j => j.JobId) : query.OrderByDescending(j => j.JobId)
            };
        }
        
        return query;
    }
    
}