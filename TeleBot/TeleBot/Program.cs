using TeleBot.ServiceBot.Commands;
using TeleBot.ServiceBot;
using TeleBot.ServiceBot.Commands.Deposit;
using TeleBot.ServiceBot.Interfaces;
using Telegram.Bot;
using TeleBot.Services;
using TeleBot.Models;
using TeleBot.Controllers;

namespace TeleBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Setup Bot configuration
            var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
            builder.Services.Configure<BotConfiguration>(botConfigurationSection);

            var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

            // Register named HttpClient to get benefits of IHttpClientFactory
            // and consume it with ITelegramBotClient typed client.
            // More read:
            //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#typed-clients
            //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    var botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

            // Add services to the container.
            builder.Services.AddScoped<UpdateHandlers>();

            //builder.Services.AddTransient<ITelegramBotClient>(x => new TelegramBotClient(builder.Configuration["BotConfiguration:BotToken"]));
            builder.Services.AddHostedService<TelegramBotHostedService>();

            builder.Services.AddSingleton<ServiceLocator>();

            builder.Services.AddSingleton<IUserStateManager, UserStateManager>();
            builder.Services.AddSingleton<ICommandHandler, CommandHandler>();
            builder.Services.AddSingleton<ICommandFactory, CommandFactory>();

            RegisterCommands();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);
            app.MapControllers();

            app.Run();

            void RegisterCommands()
            {
                builder.Services.AddSingleton<IBotCommand, StartCommand>();
                builder.Services.AddSingleton<IBotCommand, PlayCommand>();

                builder.Services.AddSingleton<IBotCommand, AboutUsCommand>();
                builder.Services.AddSingleton<IBotCommand, ShareContactCommand>();
                builder.Services.AddSingleton<IBotCommand, MainCommand>();
                builder.Services.AddSingleton<IBotCommand, GameCommand>();
                builder.Services.AddSingleton<IBotCommand, DepositCommand>();

                builder.Services.AddSingleton<IBotCommand, OnlineBankingCommand>();
                builder.Services.AddSingleton<IBotCommand, PaymentGatewayCommand>();
            }
        }
    }
}