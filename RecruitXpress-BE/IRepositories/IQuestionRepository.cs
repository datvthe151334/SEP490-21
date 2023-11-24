using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface IQuestionRepository
    {
        Task SetUpLevelOfGeneralTests(int easy, int medium, int hard);
        Task<IEnumerable<ExamQuestionDTO>> GenerateATest();
        Task<ApiResponse<QuestionDTO>> GetAllQuestions(QuestionRequest request);

        Task CreateQuestion(Question question);

        Task<QuestionDTO> GetQuestionById(int questionId);

        Task<Question> UpdateQuestion(Question question);

        Task<bool> DeleteQuestion(int questionId);
        Task<bool> DeleteOption(int optionId);
    }
}
