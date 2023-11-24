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
    [Route("api/ShortListing/")]
    [ApiController]

    public class ShortListingController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
       
        public ShortListingController(RecruitXpressContext context)
        {
            _context = context;
        }

        [HttpGet("ShortListing")]
        public async Task<IActionResult> GetShortListing(int jobId)
        {
            try
            {
                if (jobId == null)
                {
                    return BadRequest("Thieu jobId");
                }
                var profile = await _context.ShortListings.Include(x => x.Profile).ThenInclude(x => x.Account).ThenInclude(x => x.CandidateCvs).Include(x => x.Job).Where(x => x.Status == 1).ToListAsync();
                if (profile == null)
                {
                    return NotFound("Không kết quả");
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllShortListing")]
        public async Task<IActionResult> GetAllShortListing()
        {
            try
            {
               
                var profile = await _context.ShortListings.Include(x => x.Profile).ThenInclude(x=>x.Account).ThenInclude(x=>x.CandidateCvs).Include(x => x.Job).Where(x => x.Status == 1).ToListAsync();
                if (profile == null)
                {
                    return NotFound("Không kết quả");
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST: api/ProfileManagement
        [HttpPost]
        public async Task<IActionResult> AddToShortListing(ShortListing shortlisting)
        {
            try
            {
               
                if(shortlisting != null )
                {
                    var data = shortlisting;
                    data.CreatedAt = DateTime.Now;
                    data.Status = 1;
                    _context.ShortListings.Add(data);
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
   
       
    }
}
