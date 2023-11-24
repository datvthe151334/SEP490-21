using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface IExamRepository
    {
        Task<ApiResponse<ExamDTO>> GetAllExams(ExamRequest request);
        Task<ApiResponse<ExamDTO>> GetListExamWithSpecializedExamId(ExamRequest request, int sid);
        Task<ApiResponse<ExamDTO>> GetListExamWithSpecializedExamCode(ExamRequest request, string code, string expertEmail);
        Task<ExamDTO> GetExamById(int examId);
        Task<Exam> CreateExamWithFile(ExamRequestClass exam, IFormFile fileData);
        Task UpdateExam(Exam exam);
        Task GradeExam(GradeExamRequest e);
        Task AssignExpertToSystem(string email, string examCode);
        Task<bool> DeleteExam(int examId);
    }
}
