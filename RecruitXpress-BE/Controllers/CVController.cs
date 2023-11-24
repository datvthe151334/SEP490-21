using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace RecruitXpress_BE.Controllers
{
    [Route("api/CV/")]
    [ApiController]

    public class CVController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        public CVController(RecruitXpressContext context)
        {
            _context = context;
        }
        [HttpPost("accountId")]
        public async Task<IActionResult> addCV(IFormFile fileData, int accountId)
        {
            try
            {
                if (accountId != null)
                {
                    var check = await _context.CandidateCvs.SingleOrDefaultAsync(x => x.AccountId == accountId);
                    if(check != null)
                    {
                        await DeleteCVEsxit(accountId);
                    }
                }else
                {
                    return BadRequest("Invalid AccountId");
                }

                if(fileData == null)
                {
                    return BadRequest("Please upload file");
                }
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                int Timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var fileName = "";
                if(fileData.FileName.Contains(" "))
                {
                     fileName = Timestamp + "_"  + accountId + fileData.FileName.Replace(" ", "_");
                }
                else
                {
                    fileName = Timestamp + "_"+ accountId + fileData.FileName ;
                }
                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await fileData.CopyToAsync(fileStream);
                }
               
                var fileMeterial = new CandidateCv()
                {
                    AccountId = accountId,
                    Status = 1,
                    Url = "\\" + fileName,
                    CreatedAt = DateTime.Now,

                };
               
                    var result = _context.CandidateCvs.Add(fileMeterial);
                    await _context.SaveChangesAsync();
                    return Ok("Thêm CV thành công ");
                

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("cvtemplateId")]
        public async Task<IActionResult> DownloadCV(int cvId)
        {
            var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.TemplateId == cvId);
            if (result == null)
            {
                return NotFound("File not found!");
            }
            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
            
            var filePath = path + result.Url;
           
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found!" + filePath);
            }
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            var contentType = "application/octet-stream";
            return File(fileContent, contentType, fileName);
        }
        [HttpDelete("cvtemplateId")]
        public async Task<IActionResult> DeleteCV(int cvId)
        {
            try
            {
                var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.TemplateId == cvId);
                if(result == null)
                {
                    return NotFound("File not found!");
                }
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
                var filePath = path + result.Url;
                System.IO.File.Delete(filePath);
                _context.CandidateCvs.Remove(result);
                await _context.SaveChangesAsync();
                return Ok("Xóa thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        } 
        private async Task<IActionResult> DeleteCVEsxit(int accountId)
        {
            try
            {
                var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.AccountId == accountId);
                if (result == null)
                {
                    return BadRequest("Xoa CV fail");
                }
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
                var filePath = path + result.Url;
                System.IO.File.Delete(filePath);
                _context.CandidateCvs.Remove(result);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("myCV")]
        private async Task<IActionResult> getMyCV(int accountId)
        {
            if (accountId == null) return BadRequest("AccountId dau ?");
            
            var cv = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.AccountId == accountId);
            if (cv == null)
            {
                return BadRequest("Khong tim thay CV");
            }
            return Ok(cv);
        }
    }
}
