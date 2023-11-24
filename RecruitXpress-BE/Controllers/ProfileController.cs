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
    [Route("api/Profile/")]
    [ApiController]

    public class ProfileController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
       
        public ProfileController(RecruitXpressContext context)
        {
            _context = context;
        }
        //GET: api/ProfileManagement
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Profile>>> GetListProfile()
        //{
        //     var result =  await _context.GetListProfile();
        //   return Ok(result);
        //    return Ok(new ResponseData<Models.Profile>
        //    {

        //        Data = result.Skip(offset).Take(result.Count()).ToList(),
        //        Offset = offset,
        //        Limit = limit,
        //        Total = result.Count()

        //    });
        //}

        //GET: api/ProfileManagement/{id}
        [HttpGet("id")]
        public async Task<IActionResult> GetProfile(int id)
        {
            try
            {

            var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.AccountId == id);
            if (profile == null)
            {
                return NotFound("Không kết quả");
            }

            return Ok(profile);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //POST: api/ProfileManagement
        [HttpPost]
        public async Task<IActionResult> AddProfile(Profile Profile, int accountId)
        {
            try
            {
               
                if(Profile != null && accountId != null)
                {
                    var profile = Profile;
                    profile.AccountId = accountId;
                    _context.Profiles.Add(Profile);
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
        public async Task<IActionResult> UpdateProfile(int accountId, Profile Profile)
        {
            try
            {
                _context.Entry(Profile).State = EntityState.Modified;
                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.ProfileId == Profile.ProfileId && x.AccountId == accountId);
                if(profile != null)
                {
                    var result = profile;
                    _context.Update(result);
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
        //DELETE: api/ProfileManagement
   
       
    }
}
