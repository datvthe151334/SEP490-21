using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class WishListRepository : IWishListRepository
{
    private readonly RecruitXpressContext _context;

    public WishListRepository(RecruitXpressContext context)
    {
        _context = context;
    }

    public async Task<List<WishList>> GetListWishLists()
        => await _context.WishLists.ToListAsync();

    public async Task<List<WishListDTO>> GetListWishLists(int accountId, string? searchString, string? sortBy,
        bool? isSortAscending, int page, int size)
    {
        var query = GetAdvancedSearchWishListQuery(
            accountId,
            new JobPostingSearchDTO()
            {
                SearchString = searchString,
                SortBy = sortBy,
                IsSortAscending = isSortAscending == true
            }
        );

        return await query
            .Select(w => new WishListDTO()
            {
                WishlistId = w.WishlistId,
                AccountId = w.AccountId,
                Status = w.Status,
                JobId = w.JobId,
                Job = w.Job,
                TotalCount = query.Count()
            })
            .Skip((page - 1) * size).Take(size)
            .ToListAsync();
    }

    public async Task<List<WishList?>> GetWishList(int accountId)
        => await _context.WishLists.Where(wishList => wishList.AccountId == accountId).ToListAsync();

    public async Task<WishList> AddWishList(WishList wishList)
    {
        try
        {
            var oldWishList = _context.WishLists.Where(w => w.AccountId == wishList.AccountId)
                .SingleOrDefault(w => w.JobId == wishList.JobId);
            if (oldWishList == null)
            {
                _context.Entry(wishList).State = EntityState.Added;
            }
            else
            {
                _context.Entry(oldWishList).State = EntityState.Deleted;
            }

            await _context.SaveChangesAsync();
            return wishList;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteWishList(int jobId)
    {
        var wishList = await _context.WishLists.FindAsync(jobId);
        if (wishList == null)
        {
            return false;
        }

        _context.Entry(wishList).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<WishList> GetAdvancedSearchWishListQuery(int accountId, JobPostingSearchDTO searchDto)
    {
        var query = _context.WishLists
            .Include(w => w.Job)
            .Where(w => w.AccountId == accountId && w.Job.Status == Constant.ENTITY_STATUS.ACTIVE)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchDto.SearchString))
        {
            query = query.Where(w =>
                w.Job.Title.Contains(searchDto.SearchString) || w.Job.Description.Contains(searchDto.SearchString));
        }

        if (!string.IsNullOrEmpty(searchDto.Location))
        {
            query = query.Where(w => w.Job.Location.Contains(searchDto.Location));
        }

        if (!string.IsNullOrEmpty(searchDto.EmploymentType))
        {
            query = query.Where(w => w.Job.EmploymentType == searchDto.EmploymentType);
        }

        if (!string.IsNullOrEmpty(searchDto.Industry))
        {
            query = query.Where(w => w.Job.Industry == searchDto.Industry);
        }

        if (searchDto.MinSalary != null && searchDto.MaxSalary != null)
        {
            query = query.Where(w => w.Job.MinSalary >= searchDto.MinSalary && w.Job.MaxSalary <= searchDto.MaxSalary);
        }

        if (searchDto.ApplicationDeadline.HasValue)
        {
            query = query.Where(w => w.Job.ApplicationDeadline <= searchDto.ApplicationDeadline);
        }

        if (!string.IsNullOrEmpty(searchDto.SortBy))
        {
            query = searchDto.SortBy switch
            {
                "Location" => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.Location)
                    : query.OrderByDescending(w => w.Job.Location),
                "EmploymentType" => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.EmploymentType)
                    : query.OrderByDescending(w => w.Job.EmploymentType),
                "Industry" => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.Industry)
                    : query.OrderByDescending(w => w.Job.Industry),
                "ApplicationDeadline" => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.ApplicationDeadline)
                    : query.OrderByDescending(w => w.Job.ApplicationDeadline),
                _ => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.JobId)
                    : query.OrderByDescending(w => w.Job.JobId)
            };
        }

        return query;
    }
}