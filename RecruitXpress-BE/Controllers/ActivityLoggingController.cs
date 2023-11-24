using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using Org.BouncyCastle.Asn1.Ocsp;
using AutoMapper;
using RecruitXpress_BE.DTO;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/ActivityLogging/")]
    [ApiController]

    public class ActivityLoggingController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public ActivityLoggingController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
       
        [HttpGet("activityLogging")]
        public async Task<IActionResult> listJobApplication(string? searchString)
        {
            try
            {
               
                var query = _context.Profiles.AsQueryable();
               
               if(searchString!= null)
                {
                    query = query.Where(s => s.Email.Contains(searchString) ||
                     s.PhoneNumber.Contains(searchString) ||
                     s.Name.Contains(searchString));
                }

                var activityLogging = await query.ToListAsync();

                //var activityLoggingDTO = _mapper.Map<List<ActivityLoggingDTO>>(activityLogging);

                return Ok(activityLogging);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
    }
}
