using AutoMapper;
using Microsoft.Extensions.Configuration;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;
using Profile = AutoMapper.Profile;

namespace RecruitXpress_BE.Contracts
{
    public class MappingProfile : Profile
    {


        public MappingProfile()
        {
            CreateMap<JobApplication, JobApplicationDTO>();
               // .ForMember(dest => dest.GeneralTest, opt => opt.MapFrom(src => src.Profile.GeneralTests))
               // .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => src.Profile.Schedules))
               //.ForMember(dest => dest.ScheduleDetail, opt => opt.MapFrom(src => src.ScheduleDetails))
               // .ForMember(dest => dest.Evaluate, opt => opt.MapFrom(src => src.Profile.Evaluates))
               // .ForMember(dest => dest.Exam, opt => opt.MapFrom(src => src.Profile.Account.Exams));
            CreateMap<ComputerProficiency, ComputerProficiencyDTO>();
            CreateMap<MaritalStatus, MaritalStatusDTO>();
            CreateMap<LanguageProficiency, LanguageProficiencyDTO>();
            CreateMap<EducationalBackground, EducationalBackgroundDTO>();
            CreateMap<FamilyInformation, FamilyInformationDTO>();
            CreateMap<training, TrainigDTO>();
            CreateMap<WorkExperience, WorkExperienceDTO>();
            CreateMap<GeneralTest, GeneralTestDTO>()
            .ForMember(dest => dest.GeneralTestDetails, opt => opt.MapFrom(src => src.GeneralTestDetails))
            .ForMember(dest => dest.CreatedByAccount, opt => opt.MapFrom(src => src.CreatedByNavigation));
            CreateMap<JobPosting, JobDTO>();
            CreateMap<Evaluate, EvaluateDTO>();
            CreateMap<EvaluateDTO, Evaluate>();
            CreateMap<CandidateCv, CvtemplateDTO>().ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId));
            CreateMap<Models.Profile, LoggingDTO>();




            // Mapping cua Trang
            CreateMap<Option, OptionDTO>();

            CreateMap<Question, QuestionDTO>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.CreatedByAccount, opt => opt.MapFrom(src => src.CreatedByNavigation))
                .ForMember(dest => dest.Question1, opt => opt.MapFrom(src => src.Question1));

            CreateMap<Account, AccountDTO>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Account1));

            CreateMap<Option, ExamOptionDTO>();

            CreateMap<Question, ExamQuestionDTO>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.Question1, opt => opt.MapFrom(src => src.Question1));

            CreateMap<SpecializedExam, SpecializedExamDTO>()
           .ForMember(dest => dest.CreatedByAccount, opt => opt.MapFrom(src => src.CreatedByNavigation))
           .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title));
            CreateMap<GeneralTestDetail, GeneralTestDetailDTO>();
            CreateMap<Exam, ExamDTO>();

        }
    }
}
