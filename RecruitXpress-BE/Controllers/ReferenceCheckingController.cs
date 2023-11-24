using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using System.Security.Principal;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/ReferenceChecking/")]
    [ApiController]

    public class ReferenceCheckingController : ControllerBase
    {

        private readonly RecruitXpressContext _context;

        public ReferenceCheckingController(RecruitXpressContext context)
        {
            _context = context;
        }


        //GET: api/ProfileManagement/{id}
        [HttpGet("getlistbyaccount")]
        public async Task<IActionResult> GetList(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.AccountId == accountId);

                if (profile == null)
                {
                    return NotFound("Không kết quả");
                }
                var data = await _context.ReferenceCheckings.Include(x => x.Profile).Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
                if (data == null)
                {
                    return NotFound("Không kết quả");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getAll")]
     public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.ReferenceCheckings.Include(x=>x.Profile).ToListAsync();
                if (data == null)
                {
                    return NotFound("Không kết quả");
    }
                return Ok(data);
}
            catch (Exception ex)
{
    return BadRequest(ex.Message);
}
        }
        //POST: api/ProfileManagement
        [HttpPost]
        public async Task<IActionResult> AddReference(ReferenceChecking referenceChecking, int accountId)
{
    try
    {
        var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.AccountId == accountId);

        if (profile == null)
        {
            return BadRequest("fail");
        }
        if (referenceChecking != null && accountId != null)
        {
            var data = referenceChecking;
            data.ProfileId = profile.AccountId;
             _context.ReferenceCheckings.Add(data);
                  await _context.SaveChangesAsync();
            return Ok("Thành công");
        }
        else
        {
            return BadRequest("Không có dữ liệu");
        }

    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }

}

//PUT: api/ProfileManagement/{id}
[HttpPut("id")]
public async Task<IActionResult> UpdateReference(ReferenceChecking referenceChecking, int accountId)
{
    try
    {
        _context.Entry(referenceChecking).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok("Thành công");

    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}
    }
}
