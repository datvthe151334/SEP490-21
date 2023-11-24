namespace RecruitXpress_BE.Hub;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class JobApplicationStatusHub : Hub
{
    public async Task NotifyStatusChange(int jobApplicationId, int newStatus)
    {
        await Clients.All.SendAsync("StatusChanged", jobApplicationId, newStatus);
    }
}