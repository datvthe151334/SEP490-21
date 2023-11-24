using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface IGeneralTestRepository
    {
        Task<ApiResponse<GeneralTestDTO>> GetAllGeneralTests(GeneralTestRequest request);
        Task<GeneralTestDTO> GetGeneralTestById(int generalTestId);
        Task CreateGeneralTest(GeneralTest generalTest);
        Task UpdateGeneralTest(int generalTestId, GeneralTest generalTest);
        Task<bool> DeleteGeneralTest(int generalTestId);
        Task SubmitGeneralTest(GeneralTest generalTest);
    }
}
