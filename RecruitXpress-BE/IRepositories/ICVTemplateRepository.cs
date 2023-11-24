using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.IRepositories
{
    public interface ICVTemplateRepository
    {

        Task<List<Cvtemplate>> GetAllCvtemplates();

        Task<Cvtemplate> GetCvtemplateById(int cvtemplateId);

        Task<Cvtemplate> CreateCvtemplate(Cvtemplate cvtemplate);

        Task<CvtemplateDTO> UpdateCvtemplate(int cvtemplateId, Cvtemplate cvtemplateDTO);

        Task<bool> DeleteCvtemplate(int cvtemplateId);
        Task<Cvtemplate> CreateCvTemplateWithFile(CvRequestClass cvtemplate, IFormFile fileData);
        Task<Cvtemplate> CvTemplateThumbNail(int id, IFormFile fileData);
    }
}
