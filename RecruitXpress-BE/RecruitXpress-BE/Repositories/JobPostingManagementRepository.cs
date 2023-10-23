﻿using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class JobPostingManagementRepository : IJobPostingManagementRepository
{
    private readonly RecruitXpressContext _context = new();
    public async Task<List<JobPosting>> GetListJobPostings()
    {
        return await _context.JobPostings.ToListAsync();
    }
    
    public async Task<JobPosting?> GetJobPosting(int id)
    {
        return await _context.JobPostings.FindAsync(id);
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
            jobPosting.JobId = id;
            await _context.SaveChangesAsync();
            return jobPosting;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DeleteJobPosting(int jobId)
    {
        var jobPosting = _context.JobPostings.Find(jobId);
        if (jobPosting == null)
        {
            throw new Exception();
        }

        _context.Entry(jobPosting).State = EntityState.Deleted;
        _context.SaveChanges();
    }
}