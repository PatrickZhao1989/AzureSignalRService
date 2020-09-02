using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Connections.Client;

namespace SignalRServiceConsoleAppClient
{
    class Program
    {
        private static HubConnection _hubConnection;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (var httpclient = new HttpClient())
            {
                Task.Run(async () =>
                {
                    try
                    {
                        HubConnectionInfo hubConnectionInfo =
                            await httpclient.GetFromJsonAsync<HubConnectionInfo>($"http://localhost:7071/api/negotiate",
                                null);

                        _hubConnection = new HubConnectionBuilder()
                            .WithUrl(hubConnectionInfo.url, options =>
                            {
                                options.AccessTokenProvider = () => Task.FromResult(hubConnectionInfo.accessToken);
                            }).Build();
                        _hubConnection.On("newMessage", (object newMessage) =>
                        {
                            Console.WriteLine(newMessage.ToString());
                        });

                        await _hubConnection.StartAsync();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    
                    // Do any async anything you need here without worry
                }).GetAwaiter().GetResult();

                while (true)
                {
                    Task.Delay(1000).GetAwaiter().GetResult();
                }
            }

        }
    }

    public class HubConnectionInfo
    {
        public string url { get; set; }
        public string accessToken { get; set; }
    }
}
