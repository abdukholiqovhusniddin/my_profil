using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using AboutMe.Data;
using AboutMe.Models;
using Microsoft.EntityFrameworkCore;

namespace AboutMe.Services
{
    public class TelegramMessageCountService(IServiceProvider serviceProvider, IConfiguration config, ILogger<TelegramMessageCountService> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IConfiguration _config = config;
        private readonly ILogger<TelegramMessageCountService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string? botToken = _config["Telegram:NewToken"];
            string? chatId = _config["Telegram:NewChatId"];

            using var httpClient = new HttpClient();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await httpClient.GetAsync($"https://api.telegram.org/bot{botToken}/getUpdates", stoppingToken);
                    var updates = await response.Content.ReadFromJsonAsync<TelegramUpdateResponse>(cancellationToken: stoppingToken);

                    int messageCount = 0;
                    if (updates?.result != null && updates.result.Count != 0)
                    {
                        messageCount = updates.result.Max(u => u.update_id);
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<MyDb>();
                    var counts = await dbContext.Counts.FirstOrDefaultAsync(c => c.Id == 1, cancellationToken: stoppingToken);

                    if (counts != null)
                    {
                        counts.CommentCount = messageCount - 402277759;
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching Telegram message count.");
                }

                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }

        public class TelegramUpdateResponse
        {
            public List<Result>? result { get; set; }

            public class Result
            {
                public int update_id { get; set; }
            }
        }
    }
}