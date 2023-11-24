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
    [Route("api/FamilyInformation/")]
    [ApiController]

    public class FamilyInformationController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
        public IMapper _mapper;
        public FamilyInformationController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("getbyId")]
        public async Task<IActionResult> GetFamilyInformation(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");

                var result = await _context.FamilyInformations.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
                var listComputer = _mapper.Map<List<FamilyInformationDTO>>(result);
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

        //POST: api/FamilyInformationManagement
        [HttpPost]
        public async Task<IActionResult> AddFamilyInformation(List<FamilyInformation> familyInformation, int accountId)
        {
            try
            {
                if (familyInformation != null && accountId != null)
                {
                    var profile =await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                    if (profile == null) return NotFound("Account chua co profile");
                    var listUpdate = new List<FamilyInformation>();
                    foreach (var familyInfor in familyInformation)
                    {
                        var familyInfor1 = familyInfor;
                        if (familyInfor1.FamilyId != null)
                        {
                            listUpdate.Add(familyInfor1);
                        }
                        else { 
                        familyInfor1.ProfileId = profile.ProfileId;
                        _context.FamilyInformations.Add(familyInfor1);
                        }

                    }
                    if(listUpdate != null)
                    {
                        foreach(var family in listUpdate)
                        {
                            _context.Update(family);
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

        //PUT: api/FamilyInformationManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateFamilyInformation(List<FamilyInformation> data)
        {
            try
            {
                if (data != null)
                {
                    foreach (var com in data)
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
        //DELETE: api/FamilyInformationManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteFamilyInformation(int Id)
        {
            try
            {
                var result = await _context.FamilyInformations.FirstOrDefaultAsync(x => x.FamilyId == Id);
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
