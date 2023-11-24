using RecruitXpress_BE.DTO;

namespace RecruitXpress_BE.IRepositories;

public interface IGoogleService
{
    string GetAuthUrl(string redirectUrl);
    Task<GoogleTokenResponse> GetTokens(string code);
}