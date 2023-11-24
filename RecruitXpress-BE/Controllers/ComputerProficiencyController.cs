using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.Contracts;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using System.Security.Principal;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/ComputerProficiency/")]
    [ApiController]

    public class ComputerProficiencyController : ControllerBase
    {

        private readonly RecruitXpressContext _context;
        public IMapper _mapper;
        public ComputerProficiencyController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
           
        }

        //GET: api/ComputerProficiencyManagement/{id}
        [HttpGet("getbyId")]
        public async Task<IActionResult> GetComputerProficiency(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");
                var result = await _context.ComputerProficiencies.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
                var listComputer = _mapper.Map<List<ComputerProficiencyDTO>>(result);
                if (listComputer == null)
                {
                    return NotFound("Không kết quả");
                }
                return Ok(listComputer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST: api/ComputerProficiencyManagement
        [HttpPost]
        public async Task<IActionResult> AddComputerProficiency(List<ComputerProficiency> computerProficiency, int accountId)
        {
            try
            {
               
                if (computerProficiency != null && accountId != null)
                {
                    var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                    if (profile == null) return NotFound("Account chua co profile");
                    var listUpdate = new List<ComputerProficiency>();
                    foreach (var com in computerProficiency)
                    {
                    var computer = com;
                        if (computer.ComputerProficiencyId != null)
                        {
                            listUpdate.Add(computer);
                        }
                        else
                        {
                        computer.ProfileId = profile.ProfileId;
                        _context.ComputerProficiencies.Add(computer);
                        }

                    }
                    if (listUpdate != null)
                    {
                        foreach (var compu in listUpdate)
                        {
                            _context.Update(compu);
                        }
                    }
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

        //PUT: api/ComputerProficiencyManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateComputerProficiency(List<ComputerProficiency> computerProficiency)
        {
            try
            {
                if(computerProficiency != null)
                {
                    foreach(var com in computerProficiency)
                    {
                        _context.Entry(com).State = EntityState.Modified;
                    }
               
                await _context.SaveChangesAsync();
                }
                return Ok("Cập nhật thành công");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //DELETE: api/ComputerProficiencyManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteComputerProficiency(int Id)
        {
            try
            {
                var result = await _context.ComputerProficiencies.FirstOrDefaultAsync(x => x.ComputerProficiencyId == Id);
                if (result != null)
                {
                    _context.Remove(result);
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return NotFound("Không tồn tại");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
