using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Serilog;
using W88.TeleBot.Controllers;
using W88.TeleBot.Extensions;
using W88.TeleBot.Model;
using W88.TeleBot.ServiceBot;
using W88.TeleBot.ServiceBot.Interfaces;
using W88.TeleBot.ServiceBot.Utils;
using W88.TeleBot.Services;
using W88.TeleBot.Services.Queues;
using W88.TeleBot.Services.BackgroundServices;
using W88.TeleBot.Services.Interfaces;

const string allowSpecificOrigin = "AllowSpecificOrigin";

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Setup Bot configuration
var coreConfigSection = configuration.GetSection(CoreConfig.Configuration);
builder.Services.Configure<CoreConfig>(coreConfigSection);
var coreConfig = coreConfigSection.Get<CoreConfig>();

ConfigureService();
var app = builder.Build();
Configure();
app.Run();
return;

void ConfigureService()
{
    // Add logging with Serilog
    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddSingleton<CoreConfig>(coreConfig);

    // Register localization, caching, and utility services
    builder.Services.AddLocalization();
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ServiceLocator>();
    
    // Register shared services
    builder.Services.AddSingleton<UpdateQueue>();
    builder.Services.AddScoped<UpdateHandler>();
    builder.Services.AddScoped<IBaseUpdateHandler, UpdateHandler>();
    builder.Services.AddScoped<IUserStateManager, UserStateManager>();
    builder.Services.AddScoped<ICommandHandler, CommandHandler>();
    builder.Services.AddScoped<ICommandFactory, CommandFactory>();
    builder.Services.AddScoped<ITelegramAuth, TelegramAuth>();
    builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

    // Register named HttpClient
    //builder.Services.AddHttpClient("telegram_bot_client")
    //    .RemoveAllLoggers()
    //    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    //    {
    //        var botConfig = sp.GetRequiredService<IOptions<CoreConfig>>().Value;
    //        TelegramBotClientOptions options = new(botConfig.BotToken);
    //        return new TelegramBotClient(options, httpClient);
    //    });

    // Register TelegramBotClientFactory to dynamically resolve bot clients
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<ITelegramBotClientFactory, TelegramBotClientFactory>();

    builder.Services.AddHostedService<TelegramBotHostedService>();
    builder.Services.AddHostedService<UpdateBackgroundService>();

    // Custom service registrations
    builder.Services.RegisterDomainServices();
    builder.Services.RegisterCommands();

    // CORS policy
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: allowSpecificOrigin,
            policy =>
            {
                policy.WithOrigins(coreConfig.MiniAppUrl)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

    builder.Services.ConfigureTelegramBotMvc();
    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(coreConfig.JwtPublicKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

    // Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

void Configure()
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Add Serilog request logging
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseCors(allowSpecificOrigin);

    app.UseAuthentication();
    app.UseAuthorization();

    var supportedCultures = new[] { "en", "ms" };
    var localizationOptions = new RequestLocalizationOptions()
        .SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);

    app.UseRequestLocalization(localizationOptions);

    foreach (var botConfig in coreConfig.Bots)
    {
        app.MapBotWebhookRoute<BotController>($"/{botConfig.BotName.ToLowerInvariant()}");
    }

    app.MapControllers();

    app.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Welcome to our W88 Telegram Bot!");
    });

    app.UseWebSockets();
}
