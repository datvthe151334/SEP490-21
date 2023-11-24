using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using System.Security.Principal;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Evaluate/")]
    [ApiController]

    public class EvaluateController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
        public IMapper _mapper;

        public EvaluateController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //GET: api/ProfileManagement/{id}
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetDetailEvaluate(int evaluareId)
        {
            try
            {

            var evaluate = await _context.Evaluates.FirstOrDefaultAsync(x => x.EvaluateId == evaluareId);
            if (evaluate == null)
            {
                return NotFound("Không kết quả");
            }

            return Ok(evaluate);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        //GET: api/ProfileManagement/{id}
        [HttpGet("evaluateByAccount")]
        public async Task<IActionResult> GetAllEvaluateOfAccount(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(x=> x.AccountId== accountId);
                if (profile == null){
                    return BadRequest("Account chua co du profile");
                }
                var evaluate = await _context.Evaluates.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
                if (evaluate == null)
                {
                    return NotFound("Không kết quả");
                }

                return Ok(evaluate);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        [HttpGet("myEvaluated")]
        public async Task<IActionResult> GetAllmyEvaluated(int evaluaterAccountId)
        {
            try
            {
               
                if (evaluaterAccountId == null)
                {
                    return BadRequest("evaluaterAccountId khong co");
                }
                var evaluate = await _context.Evaluates.Include(x=>x.Profile).Where(x => x.EvaluaterAccountId == evaluaterAccountId).ToListAsync();
                if (evaluate == null)
                {
                    return NotFound("Không kết quả");
                }

                return Ok(evaluate);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        //POST: api/ProfileManagement
        [HttpPost]
        public async Task<IActionResult> AddEvaluate(EvaluateDTO evaluate, int EvaluaterAccountId, string? emailContact, string? phonelContact)
        {
            try
            {
               
                if(evaluate != null && EvaluaterAccountId != null)
                {
                    var data = evaluate;
                    data.EvaluaterAccountId = EvaluaterAccountId;
                    if(emailContact != null)
                    {
                        data.EvaluaterEmailContact = emailContact;
                    }
                    if (phonelContact != null)
                    {
                        data.EvaluaterPhoneContact = phonelContact;
                    }
                    data.Status = 1;
                    var create = _mapper.Map<Evaluate>(data);
                    _context.Evaluates.Add(create);
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
        [HttpPut("updateEvaluate")]
        public async Task<IActionResult> Updateevaluate(Evaluate evaluate)
        {
            try
            {
                    _context.Entry(evaluate).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        //DELETE: api/ProfileManagement
   
       
    }
}
