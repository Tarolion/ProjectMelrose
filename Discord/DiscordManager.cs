using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMelrose.Discord
{
    public struct QueueItem
    {
        public string Name;
        public string Avatar_URL;
        public string Message;

        public QueueItem(string n, string a, string m)
        {
            this.Name = n;
            this.Avatar_URL = a;
            this.Message = m;
        }
    }

    public static class DiscordManager
    {
        private static bool Debug = false;

        public static string Name = "Cookie Monster";
        public static string Avatar_URL = @"http://www.good-collective.co.uk/wp-content/themes/we3/assets/images/cookie-monster.png";
        public static readonly string WebhookAddress = @"https://discordapp.com/api/webhooks/433706092741132288/cOPK2vjbn1Q2-OVKVtX1InwLCsb4Au1dUSQtFfkRmk0WPEjJ2LpNV4VIld_cF957YCe3";
        public static string Message;
        public static bool Success = false;
        
        public static void ConstructMessageFromLines(List<string> lines)
        {
            Message = "";

            foreach(String s in lines)
            {
                Message += s + "\n";
            }
        }
        public static void ConstructMessageFromLines(string[] lines)
        {
            Message = "";

            foreach (String s in lines)
            {
                Message += s + "\n";
            }
        }

        public static List<QueueItem> messages = new List<QueueItem>();

        public static async void DiscordSendThread()
        {
            while(messages.Count > 0)
            {
                Success = false;
                QueueItem i = messages.ElementAtOrDefault(0);
                
                while (!Success)
                {
                    Success = await SendDiscord(WebhookAddress, i.Name, i.Avatar_URL, i.Message);
                    if (Debug) Console.WriteLine("Success: " + Success);
                }

                messages.RemoveAt(0);
            }
        }

        public static async Task<bool> SendDiscord(String webhook, String name, String avatar_URL, String message)
        {
            try
            {
                HttpClient client = new HttpClient();

                Dictionary<string, string> Json = new Dictionary<string, string>
                {
                   { "username", name },
                   { "avatar_url", avatar_URL },
                   { "content", message }
                };

                HttpContent content = new FormUrlEncodedContent(Json);

                var response = await client.PostAsync(webhook, content);
                var responseStr = await response.Content.ReadAsStringAsync();

                if (Debug) Console.WriteLine(responseStr);

                return true;
            }
            catch (Exception e)
            {
                if (Debug) Console.WriteLine("ERROR: " + e.StackTrace);
                return false;
            }
        }

    }
}
