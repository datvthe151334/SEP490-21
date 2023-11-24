using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.Collections;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecruitXpress_BE.Repositories
{
    public class GeneralTestRepository : IGeneralTestRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;
        public GeneralTestRepository(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        public async Task<ApiResponse<GeneralTestDTO>> GetAllGeneralTests(GeneralTestRequest request)
        {
            var query = _context.GeneralTests
                .Include(gt => gt.GeneralTestDetails)
                .Include(gt => gt.CreatedByNavigation)
                .Include(gt => gt.Profile)
                .Where(gt => gt.GeneralTestId == gt.GeneralTestId);

            if (!string.IsNullOrEmpty(request.TestName))
            {
                query = query.Where(gt => gt.TestName.Contains(request.TestName));
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                query = query.Where(gt => gt.Description.Contains(request.Description));
            }

            if (request.CreatedBy.HasValue)
            {
                query = query.Where(gt => gt.CreatedBy == request.CreatedBy);
            }

            if (request.ProfileId.HasValue)
            {
                query = query.Where(gt => gt.ProfileId == request.ProfileId);
            }

            if (request.CreatedAt.HasValue)
            {
                DateTime requestDate = request.CreatedAt.Value.Date; // Get the date portion of the request's DateTime

                query = query.Where(gt => gt.CreatedAt.Value.Date == requestDate);
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy)
                {
                    case "testName":
                        query = request.OrderByAscending
                            ? query.OrderBy(gt => gt.TestName)
                            : query.OrderByDescending(gt => gt.TestName);
                        break;

                    case "generalTestId":
                        query = request.OrderByAscending
                            ? query.OrderBy(gt => gt.GeneralTestId)
                            : query.OrderByDescending(gt => gt.GeneralTestId);
                        break;

                    case "createAt":
                        query = request.OrderByAscending
                            ? query.OrderBy(gt => gt.CreatedAt)
                            : query.OrderByDescending(gt => gt.CreatedAt);
                        break;

                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(gt => gt.GeneralTestId)
                            : query.OrderByDescending(gt => gt.GeneralTestId);
                        break;
                }
            }
            var totalCount = await query.CountAsync();

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;
            var generalTests = await query
               .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var generalTestDTOs = _mapper.Map<List<GeneralTestDTO>>(generalTests);

            var response = new ApiResponse<GeneralTestDTO>
            {
                Items = generalTestDTOs,
                TotalCount = totalCount,
            };
            return response;
        }



        public async Task<GeneralTestDTO> GetGeneralTestById(int generalTestId)
        {
            var generalTest = _context.GeneralTests
        .Include(gt => gt.CreatedByNavigation)
        .Include(gt => gt.Profile)
        .Where(gt => gt.GeneralTestId == generalTestId).SingleOrDefault();

            var generalTestDTO = _mapper.Map<GeneralTestDTO>(generalTest);

            return generalTestDTO;

        }

        public async Task CreateGeneralTest(GeneralTest generalTest)
        {
            generalTest.CreatedAt = DateTime.Now; // Set the CreatedAt property to the current date and time
            _context.GeneralTests.Add(generalTest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGeneralTest(int generalTestId, GeneralTest generalTest)
        {
            if (generalTestId != generalTest.GeneralTestId)
                throw new ArgumentException("GeneralTestId in the URL does not match the provided entity.");

            _context.Entry(generalTest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteGeneralTest(int generalTestId)
        {
            var generalTestDetails = _context.GeneralTestDetails.Where(d => d.GeneralTestId == generalTestId);

            // Delete GeneralTestDetails first
            _context.GeneralTestDetails.RemoveRange(generalTestDetails);

            var generalTest = await _context.GeneralTests.FindAsync(generalTestId);
            if (generalTest == null)
            {
                return false;
            }

            _context.GeneralTests.Remove(generalTest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SubmitGeneralTest(GeneralTest generalTest)
        {
            generalTest.CreatedAt = DateTime.Now; // Set the CreatedAt property to the current date and time
            generalTest.Score = CalculateUserScore(generalTest);
            _context.GeneralTests.Add(generalTest);

            await _context.SaveChangesAsync();
        }
        private int CalculateUserScore(GeneralTest generalTest)
        {
            int userScore = 0;

            var calculatedQuestionIds = new HashSet<int>(); // Store question IDs for which the score has been calculated.

            foreach (var detail in generalTest.GeneralTestDetails)
            {
                if (!calculatedQuestionIds.Contains((int)detail.QuestionId))
                {

                    // Get the correct answers for this question (assuming you have a data structure that stores correct answers)
                    List<int> correctAnswers = GetCorrectAnswers((int)detail.QuestionId);

                    // Check if the user's answers are in the list of correct answers
                    if(detail.Answer==null)
                    {
                        continue;
                    }    
                        List<int> userAnswers = GetUserAnswers(generalTest, (int)detail.QuestionId);
                    if(userAnswers.Count==0)
                        continue;
                    if (userAnswers.Count > correctAnswers.Count)
                    {
                        // If the user selected more answers than there are correct options, no points are awarded.
                        userScore += 0;
                    }
                    else
                    {
                        int correctOptionsCount = correctAnswers.Count;

                        if (correctAnswers.All(ca => userAnswers.Contains(ca)))
                        {
                            // User's answers match all the correct answers.
                            // Increment the user's score based on the number of correct options.
                            userScore += 1;
                        }
                        else
                        {
                            // User's answers don't match all the correct answers.
                            // No points are awarded.
                            userScore += 0;
                        }
                    }

                    // Mark this question as calculated to avoid repetition.
                    calculatedQuestionIds.Add((int)detail.QuestionId);
                }
            }
            var score = (float)userScore / generalTest.GeneralTestDetails.Select(q => q.QuestionId).Distinct().Count();
            var roundedScore = (int)Math.Ceiling(score * 10);
            return roundedScore;
        }

        private List<int> GetCorrectAnswers(int questionId)
        {
            return _context.Options.Where(o => o.QuestionId == questionId && o.IsCorrect == true).Select(o => o.OptionId).ToList();
        }
        private List<int> GetUserAnswers(GeneralTest generalTest, int questionId)
        {
            // Assuming that GeneralTest has a collection of GeneralTestDetail objects
            // and you want to retrieve the user's selected answers for a specific question.
                var userAnswers = generalTest.GeneralTestDetails
                .Where(detail => detail.QuestionId == questionId)
                .Select(detail => (int)detail.Answer)
                .ToList();

            return userAnswers;
        }


    }
}

