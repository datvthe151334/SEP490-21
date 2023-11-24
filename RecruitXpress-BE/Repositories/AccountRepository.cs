using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class AccountRepository : IAccountRepository
{
  
    private readonly RecruitXpressContext _context;
    private readonly IMapper _mapper;
    public IConfiguration _configuration;
    public AccountRepository(RecruitXpressContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }
    public async Task<List<Account>> GetListAccount()
    {
        var result = await _context.Accounts.ToListAsync();
        return result;
    }

    public async Task<Account?> GetAccount(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> AddAccount(Account account)
    {
        try
        {
            var check = await _context.Accounts.FirstOrDefaultAsync(x => x.Account1 == account.Account1);

            if (check == null)
            {
                throw new Exception("Tài khoản đã tồn tại");
            }

            var user = new Account
            {
                Account1 = account.Account1,
                Password = HashHelper.Encrypt(account.Password, _configuration),
                RoleId = account.RoleId,
                CreatedAt = DateTime.Now,
                Status = account.Status,
            };

            _context.Entry(user).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Account> UpdateAccount(int id, Account account)
    {
        _context.Entry(account).State = EntityState.Modified;
        try
        {
            var editAccount = await _context.Accounts.SingleOrDefaultAsync(x => x.AccountId == account.AccountId);
            if (editAccount != null)
            {
                var result = account;
                _context.Update(result);
                await _context.SaveChangesAsync();
                return result;
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public void DeleteAccount(int AccountId)
    {
        var account = _context.Accounts.Find(AccountId);
        if (account == null)
        {
            throw new Exception();
        }

        account.Status = 2;
        _context.SaveChanges();

    }
}