using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.Collections.Generic;
using System.Linq;

namespace RecruitXpress_BE.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;
        public QuestionRepository(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task SetUpLevelOfGeneralTests(int easy, int medium, int hard)
        {
            if (easy < 0 || medium < 0 || hard < 0)
            {
                // Throw an exception indicating that the values cannot be negative.
                throw new ArgumentException("Values for easy, medium, and hard must not be negative.");
            }
            ConstantQuestion.easy = easy;
            ConstantQuestion.medium = medium;
            ConstantQuestion.hard = hard;

        }
        public async Task<IEnumerable<ExamQuestionDTO>> GenerateATest()
        {
            List<ExamQuestionDTO> generatedTest = new List<ExamQuestionDTO>();

            // Define the number of questions for each difficulty level
            int easyQuestions = ConstantQuestion.easy;
            int mediumQuestions = ConstantQuestion.medium;
            int hardQuestions = ConstantQuestion.hard;

            // Retrieve questions from the database for each difficulty level
            var easyQuestionsList = await GetRandomQuestionsByDifficulty("Dễ", easyQuestions);
            var mediumQuestionsList = await GetRandomQuestionsByDifficulty("Trung bình", mediumQuestions);
            var hardQuestionsList = await GetRandomQuestionsByDifficulty("Khó", hardQuestions);

            // Combine the selected questions into the generated test
            generatedTest.AddRange(easyQuestionsList);
            generatedTest.AddRange(mediumQuestionsList);
            generatedTest.AddRange(hardQuestionsList);

            return generatedTest;
        }
        public async Task<ApiResponse<QuestionDTO>> GetAllQuestions(QuestionRequest request)
        {
            var query = _context.Questions
                .Include(q => q.Options)
                .Include(q => q.CreatedByNavigation)
                .Where(q => q.QuestionId == q.QuestionId);

            if (request.Name != null)
            {
                query = query.Where(s => s.Question1.Contains(request.Name));

            }
            if (request.Type != null)
            {
                query = query.Where(s => s.Type == request.Type);
            }
            if (request.CreatedBy != null)
            {
                query = query.Where(s => s.CreatedBy == request.CreatedBy);
            }
            if (request.Status != null)
            {
                query = query.Where(s => s.Status == request.Status);
            }


            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "name":
                        query = request.OrderByAscending
                            ? query.OrderBy(j => j.Question1)
                            : query.OrderByDescending(j => j.Question1);
                        break;

                    case "questionId":
                        query = request.OrderByAscending
                            ? query.OrderBy(j => j.QuestionId)
                            : query.OrderByDescending(j => j.QuestionId);
                        break;

                    default:
                        query = request.OrderByAscending
                               ? query.OrderBy(j => j.QuestionId)
                               : query.OrderByDescending(j => j.QuestionId);
                        break;
                }
            }

            var totalCount = await query.CountAsync();

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;
            var questions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var questionDTOs = _mapper.Map<List<QuestionDTO>>(questions);

            var response = new ApiResponse<QuestionDTO>
            {
                Items = questionDTOs,
                TotalCount = totalCount,
            };
            return response;
        }






        public async Task CreateQuestion(Question questiondto)
        {
            /*var questions = _mapper.Map<Question>(questiondto);*/
            _context.Questions.Add(questiondto);
            await _context.SaveChangesAsync();

        }

        public async Task<QuestionDTO> GetQuestionById(int questionId)
        {
            var question = await _context.Questions
                     .Include(q => q.Options)
                     .FirstOrDefaultAsync(q => q.QuestionId == questionId);
            return _mapper.Map<QuestionDTO>(question);
        }

        public async Task<Question> UpdateQuestion(Question question)
        {
            // Update the Question entity
            _context.Entry(question).State = EntityState.Modified;

            // Load the existing options
            var existingOptions = await _context.Options.Where(o => o.QuestionId == question.QuestionId).ToListAsync();

            // Update existing options
            foreach (var updatedOption in question.Options)
            {
                var existingOption = existingOptions.FirstOrDefault(o => o.OptionId == updatedOption.OptionId);
                if (existingOption != null)
                {
                    // Update existing option properties
                    existingOption.OptionText = updatedOption.OptionText;
                    existingOption.IsCorrect = updatedOption.IsCorrect;
                }
                else
                {
                    _context.Options.Add(updatedOption);
                }    
            }

            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteQuestion(int questionId)
        {

            var options = _context.Options.Where(o => o.QuestionId == questionId);

            // Delete options first
            _context.Options.RemoveRange(options);

            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
            {
                return false;
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOption(int optionId)
        {
            var option = _context.Options.Where(o => o.OptionId == optionId).FirstOrDefault();
            if (option == null)
            {
                return false;
            }
            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<List<ExamQuestionDTO>> GetRandomQuestionsByDifficulty(string difficulty, int count)
        {


            var query = await _context.Questions
                .Include(q => q.Options)
                .Include(q => q.CreatedByNavigation)
                .Where(q => q.Type == difficulty) // Adjust the status condition as needed
                .OrderBy(q => Guid.NewGuid()) // Randomly order the questions
                .Take(count).ToListAsync();

            var generalTestDTOs = _mapper.Map<List<ExamQuestionDTO>>(query);

            return generalTestDTOs;

        }

    }
}
