using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class MaritalStatusRepository : IMaritalStatusRepository
{
    private readonly RecruitXpressContext _context;
    private readonly IMapper _mapper;
    public IConfiguration _configuration;
    public MaritalStatusRepository(RecruitXpressContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }
    public async Task<List<MaritalStatus>> GetListMaritalStatus()
    {
        return await _context.MaritalStatuses.ToListAsync();
    }

    public async Task<MaritalStatus?> GetMaritalStatus(int id)
    {
        return await _context.MaritalStatuses.FindAsync(id);
    }

    public async Task<MaritalStatus> AddMaritalStatus(MaritalStatus maritalStatus)
    {
        try
        {
            _context.Entry(maritalStatus).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return maritalStatus;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<MaritalStatus> UpdateMaritalStatus(int id, MaritalStatus maritalStatus)
    {

        _context.Entry(maritalStatus).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            return maritalStatus;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void DeleteMaritalStatus(int msId)
    {
        var maritalStatus = _context.MaritalStatuses.Find(msId);
        if (maritalStatus == null)
        {
            throw new Exception();
        }

        _context.Entry(maritalStatus).State = EntityState.Deleted;
        _context.SaveChanges();
    }
}