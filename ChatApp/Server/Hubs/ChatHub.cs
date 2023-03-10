using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> Users = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            string username = Context.GetHttpContext().Request.Query["username"];
            if (Users.ContainsValue(username))
                await SendAlert(username);
            else
            {
                Users.Add(Context.ConnectionId, username);
                await AddMessageToChat("", $"{username} joined chat");
                await base.OnConnectedAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Users.FirstOrDefault(x => x.Key == Context.ConnectionId).Value;
            await AddMessageToChat("", $"{username} left chat.");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddMessageToChat(string user, string message)
        {
            await Clients.All.SendAsync("GeneralRoom", user, message);
        }

        public async Task SendAlert(string user)
        {
            await Clients.All.SendAsync("Alert", user);
        }
    }
}
