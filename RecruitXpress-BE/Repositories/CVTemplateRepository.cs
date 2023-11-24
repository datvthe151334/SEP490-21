using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class CVTemplateRepository : ICVTemplateRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public CVTemplateRepository(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Cvtemplate>> GetAllCvtemplates()
        {
            return await _context.Cvtemplates.ToListAsync();
        }

        public async Task<Cvtemplate> GetCvtemplateById(int cvtemplateId)
        {
            return await _context.Cvtemplates.FirstOrDefaultAsync(c=> c.CvtemplateId == cvtemplateId);

        }

        public async Task<Cvtemplate> CreateCvtemplate(Cvtemplate cvtemplate)
        {
            
            // Set the creation timestamp
            cvtemplate.CreateAt = DateTime.Now;

            _context.Cvtemplates.Add(cvtemplate);
            await _context.SaveChangesAsync();

            return cvtemplate;
        }

        public async Task<CvtemplateDTO> UpdateCvtemplate(int cvtemplateId, Cvtemplate cvtemplateDTO)
        {
            var existingCvtemplate = await _context.Cvtemplates.FindAsync(cvtemplateId);

            if (existingCvtemplate == null)
            {
                // Handle not found scenario
                return null;
            }

            // Update the existing Cvtemplate entity with new data
            _mapper.Map(cvtemplateDTO, existingCvtemplate);

            // Set the update timestamp
            existingCvtemplate.CreateAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return _mapper.Map<CvtemplateDTO>(existingCvtemplate);
        }

        public async Task<bool> DeleteCvtemplate(int cvtemplateId)
        {
            var cvtemplate = await _context.Cvtemplates.FindAsync(cvtemplateId);

            if (cvtemplate == null)
            {
                // Handle not found scenario
                return false;
            }

            _context.Cvtemplates.Remove(cvtemplate);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Cvtemplate> CreateCvTemplateWithFile(CvRequestClass cvtemplate, IFormFile fileData)
        {
            try
            {
         
                if (string.IsNullOrEmpty(cvtemplate.Title))
                {
                    throw new ArgumentException("Invalid Title");
                }
                if (fileData == null || fileData.Length == 0)
                {
                    throw new ArgumentException("Please upload a file");
                }

                if (fileData.Length > Constant.MaxFileSize)
                {
                    throw new ArgumentException("File size exceeds the maximum allowed (25MB)");
                }
         
                // Get the file extension
                var fileExtension = Path.GetExtension(fileData.FileName);
                int timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var fileName = $"thubmnail_{timestamp}_{fileExtension}";

                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"Upload\\CvTemplates"));
          
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Save the file bytes
                var filePath = Path.Combine(path, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileData.CopyToAsync(fileStream);
                }

                // Set the FileUrl property in the exam object
                var newCv = new Cvtemplate
                {
                    Title = cvtemplate.Title,
                    Thumbnail = Path.Combine(fileName),
                    Content = cvtemplate.Content,
                    CreateAt = DateTime.Now,
                    CreatedBy = cvtemplate.CreatedBy,
                    Status = 1
                };

                // Add the exam to the database
                _context.Cvtemplates.Add(newCv);
                await _context.SaveChangesAsync();
                return newCv;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Cvtemplate> CvTemplateThumbNail(int id, IFormFile fileData)
        {
            try
            {

                if (id==0)
                {
                    throw new ArgumentException("Invalid Id");
                }
                if (fileData == null || fileData.Length == 0)
                {
                    throw new ArgumentException("Please upload a file");
                }

                if (fileData.Length > Constant.MaxFileSize)
                {
                    throw new ArgumentException("File size exceeds the maximum allowed (25MB)");
                }

                // Get the file extension
                var fileExtension = Path.GetExtension(fileData.FileName);
                int timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var fileName = $"thubmnail_{timestamp}_{fileExtension}";

                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"Upload\\CvTemplates"));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Save the file bytes
                var filePath = Path.Combine(path, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileData.CopyToAsync(fileStream);
                }

                var cv = _context.Cvtemplates.FirstOrDefault(c=> c.CvtemplateId==id);
                cv.Thumbnail = Path.Combine(fileName);

                // Set the FileUrl property in the exam object
                // Add the exam to the database
   
                await _context.SaveChangesAsync();
                return cv;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
