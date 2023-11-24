using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using System.Text;
using Google.Apis.Auth.OAuth2.Responses;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;

namespace RecruitXpress_BE.Repositories;

public class GoogleService : IGoogleService
{
    private readonly HttpClient _httpClient;

    public GoogleService()
    {
        _httpClient = new HttpClient();
    }

    public string GetAuthUrl(string redirectUri)
    {
        try
        {
            return
                $"{Constant.GOOGLE_SERVICE.SCOPE_URL}" +
                "prompt=consent&include_granted_scopes=true" +
                $"&redirect_uri={GoogleHelper.urlEncodeForGoogle(redirectUri)}" +
                $"&response_type={Constant.GOOGLE_SERVICE.RESPONSE_TYPE}" +
                $"&client_id={Constant.GOOGLE_SERVICE.CLIENT_ID}" +
                $"&scope={Constant.GOOGLE_SERVICE.SCOPE.EMAIL}" +
                $"&access_type={Constant.GOOGLE_SERVICE.ACCESS_TYPE}";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<GoogleTokenResponse> GetTokens(string code)
    {
        var redirectURL = "https://localhost:7113/auth/google-callback";
        var tokenEndpoint = "https://accounts.google.com/o/oauth2/token";
        var content = new StringContent(
            $"code={code}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectURL)}" +
            $"&response_type={Constant.GOOGLE_SERVICE.RESPONSE_TYPE}" +
            $"&client_id={Constant.GOOGLE_SERVICE.CLIENT_ID}" +
            $"&client_secret={Constant.GOOGLE_SERVICE.CLIENT_SERCRET}" +
            $"&scope={Constant.GOOGLE_SERVICE.SCOPE.EMAIL}" +
            $"&access_type={Constant.GOOGLE_SERVICE.ACCESS_TYPE}" +
            $"&grant_type=authorization_code",
            Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.PostAsync(tokenEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTokenResponse>(responseContent);
            return tokenResponse;
        }
        else
        {
            // Handle the error case when authentication fails
            throw new Exception($"Failed to authenticate: {responseContent}");
        }
    }
}

    // public async Task<string> AddToGoogleCalendar(GoogleCalendarRequestDTO googleCalendarReqDTO) {
    //     try {
    //         var token = new TokenResponse {
    //             RefreshToken = googleCalendarReqDTO.refreshToken
    //         };
    //         var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
    //             new GoogleAuthorizationCodeFlow.Initializer {
    //                 ClientSecrets = new ClientSecrets {
    //                     ClientId = Constant.GOOGLE_SERVICE.CLIENT_ID,
    //                     ClientSecret = Constant.GOOGLE_SERVICE.CLIENT_SERCRET
    //                 }
    //
    //             }), "user", token);
    //
    //         var service = new CalendarService(new BaseClientService.Initializer() {
    //             HttpClientInitializer = credentials,
    //         });
    //
    //         Event newEvent = new Event() {
    //             Summary = googleCalendarReqDTO.Summary,
    //             Description = googleCalendarReqDTO.Description,
    //             Start = new EventDateTime() {
    //                 DateTime = googleCalendarReqDTO.StartTime,
    //                 //TimeZone = Method.WindowsToIana();    //user's time zone
    //             },
    //             End = new EventDateTime() {
    //                 DateTime = googleCalendarReqDTO.EndTime,
    //                 //TimeZone = Method.WindowsToIana();    //user's time zone
    //             },
    //             Reminders = new Event.RemindersData() {
    //                 UseDefault = false,
    //                 Overrides = new EventReminder[] {
    //
    //                     new EventReminder() {
    //                         Method = "email", Minutes = 30
    //                     },
    //
    //                     new EventReminder() {
    //                         Method = "popup", Minutes = 15
    //                     },
    //
    //                     new EventReminder() {
    //                         Method = "popup", Minutes = 1
    //                     },
    //                 }
    //             }
    //
    //         };
    //
    //         EventsResource.InsertRequest insertRequest = service.Events.Insert(newEvent, googleCalendarReqDTO.CalendarId);
    //         Event createdEvent = await insertRequest.ExecuteAsync();
    //         return createdEvent.Id;
    //     } catch (Exception e) {
    //         Console.WriteLine(e);
    //         th row;
    //     }
    // }
    //
    //  public async Task<CalendarList> GetGoogleCalendar(GoogleCalendarRequestDTO googleCalendarReqDTO) {
    //     try {
    //         var token = new TokenResponse {
    //             RefreshToken = googleCalendarReqDTO.refreshToken
    //         };
    //         var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
    //             new GoogleAuthorizationCodeFlow.Initializer {
    //                 ClientSecrets = new ClientSecrets {
    //                     ClientId = Constant.GOOGLE_SERVICE.CLIENT_ID,
    //                     ClientSecret = Constant.GOOGLE_SERVICE.CLIENT_SERCRET
    //                 }
    //
    //             }), "user", token);
    //
    //         var service = new CalendarService(new BaseClientService.Initializer() {
    //             HttpClientInitializer = credentials,
    //         });
    //
    //         return service.CalendarList.List();
    //         // Event newEvent = new Event() {
    //         //     Summary = googleCalendarReqDTO.Summary,
    //         //     Description = googleCalendarReqDTO.Description,
    //         //     Start = new EventDateTime() {
    //         //         DateTime = googleCalendarReqDTO.StartTime,
    //         //         //TimeZone = Method.WindowsToIana();    //user's time zone
    //         //     },
    //         //     End = new EventDateTime() {
    //         //         DateTime = googleCalendarReqDTO.EndTime,
    //         //         //TimeZone = Method.WindowsToIana();    //user's time zone
    //         //     },
    //         //     Reminders = new Event.RemindersData() {
    //         //         UseDefault = false,
    //         //         Overrides = new EventReminder[] {
    //         //
    //         //             new EventReminder() {
    //         //                 Method = "email", Minutes = 30
    //         //             },
    //         //
    //         //             new EventReminder() {
    //         //                 Method = "popup", Minutes = 15
    //         //             },
    //         //
    //         //             new EventReminder() {
    //         //                 Method = "popup", Minutes = 1
    //         //             },
    //         //         }
    //         //     }
    //         //
    //         // };
    //         //
    //         // EventsResource.InsertRequest insertRequest = service.Events.Insert(newEvent, googleCalendarReqDTO.CalendarId);
    //         // Event createdEvent = await insertRequest.ExecuteAsync();
    //         // return createdEvent.Id;
    //     } catch (Exception e) {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }
