using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace SpeedTest.Hubs
{
    public class SpeedTestHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var sessionIdString = httpContext.Request.Query["session"];

                bool isGuid = Guid.TryParse(sessionIdString, out var sessionId);
                if (!isGuid)
                {
                    throw new HubException($"Session id is not a valid GUID : {sessionIdString}");
                }
                else
                {
                    Context.Items.Add("sessionId", sessionId);
                }
            }

            await base.OnConnectedAsync();
        }

        private byte[] GetRandomImage(int size = 0)
        {
            // generate random image
            // create a buffer and fill it with random bytes
            Random rnd = new Random();
            if (size == 0)
            {
                size = rnd.Next(100_000, 500_000);
            }
            byte[] randomBuffer = new byte[size];
            rnd.NextBytes(randomBuffer);
            return randomBuffer;
        }


        public async Task StartSpeedTest()
        {
            var randomBuffer = GetRandomImage(10_000);
            // commenting the following line seems to make the bug disappear or at least very hard to reproduce.
            await Task.Delay(5_000);
            var clock = Stopwatch.StartNew();
            double previousSendTime = 0;
            var count = 10;
            for (var i = 0; i < count; i++)
            {
                await Clients.Caller.SendAsync("Log", randomBuffer);
                var currentTime = clock.Elapsed.TotalMilliseconds;
                Console.WriteLine($"Time for sendAsync {currentTime - previousSendTime}");
                previousSendTime = currentTime;
            }
            var time = clock.Elapsed.TotalMilliseconds;
            Console.WriteLine($"Time total for sendAsync {clock.Elapsed.TotalMilliseconds}");
        }

    }

}
