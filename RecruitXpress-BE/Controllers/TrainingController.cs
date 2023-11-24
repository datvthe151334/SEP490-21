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
    [Route("api/Training/")]
    [ApiController]

    public class TrainingController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
        public IMapper _mapper;
        public TrainingController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("getbyId")]
        public async Task<IActionResult> GetTraining(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");

                var result = await _context.training.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
                var listTraning = _mapper.Map<List<TrainigDTO>>(result);

                if (listTraning == null)
                {
                    return NotFound("Không kết quả");
                }
                return Ok(listTraning);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST: api/TrainingManagement
        [HttpPost]
        public async Task<IActionResult> AddTraining(List<training> data, int accountId)
        {
            try
            {
                if (data != null && accountId != null)
                {
                    var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                    if (profile == null) return NotFound("Account chua co profile");

                    var listUpdate = new List<training>();
                    foreach (var trainingData in data)
                    {
                        var data1 = trainingData;
                        if (data1.TrainingId != null)
                        {
                            listUpdate.Add(data1);
                        }
                        else
                        {
                            data1.ProfileId = profile.ProfileId;
                            _context.training.Add(data1);
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

        //PUT: api/TrainingManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateTraining(List<training> data)
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
        //DELETE: api/TrainingManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteTraining(int Id)
        {
            try
            {
                var result = await _context.training.FirstOrDefaultAsync(x => x.TrainingId == Id);
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
