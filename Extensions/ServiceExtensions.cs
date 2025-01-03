using CW88.TeleBot.ServiceBot.Commands;
using CW88.TeleBot.ServiceBot.Interfaces;
using CW88.TeleBot.Services.Domains;
using CW88.TeleBot.Services.Interfaces;
using Microsoft.Extensions.Options;
//using Karamel.LiveChat.Proto;

//using ConversationService = Karamel.LiveChat.Proto.ConversationService;

namespace CW88.TeleBot.Extensions;

public static class ServiceExtensions
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider) where T : class
    {
        var o = serviceProvider.GetService<IOptions<T>>();
        if (o is null)
            throw new ArgumentNullException(nameof(T));

        return o.Value;
    }

    public static ControllerActionEndpointConventionBuilder MapBotWebhookRoute<T>(this IEndpointRouteBuilder endpoints, string route)
    {
        var controllerName = typeof(T).Name.Replace("Controller", "", StringComparison.Ordinal);
        var actionName = typeof(T).GetMethods()[0].Name;

        return endpoints.MapControllerRoute(
            name: "bot_webhook",
            pattern: route,
            defaults: new { controller = controllerName, action = actionName });
    }

    /// <summary>
    /// References:
    /// https://learn.microsoft.com/en-us/aspnet/core/grpc/clientfactory?view=aspnetcore-7.0
    /// https://github.com/grpc/grpc-dotnet/issues/1678
    /// </summary>
    /// <param name="services"></param>
    /// <param name="coreConfig"></param>
    //public static void AddGrpcServicesClient(this IServiceCollection services, CoreConfig coreConfig)
    //{
    //    var igChatServiceUrl = coreConfig?.IGChatServiceUrl;
    //    var uri = new Uri(igChatServiceUrl ?? throw new InvalidOperationException());
    //    var subDirectory = uri.AbsolutePath == "/" ? "" : uri.AbsolutePath;

    //    services.AddGrpcClient<ConversationService.ConversationServiceClient>(o => o.Address = new Uri(uri.GetLeftPart(UriPartial.Authority)))
    //        .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(new HttpClientHandler
    //        {
    //            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    //        }));

    //    services.AddGrpcClient<MessagingService.MessagingServiceClient>(o => o.Address = new Uri(uri.GetLeftPart(UriPartial.Authority)))
    //        .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(new HttpClientHandler
    //        {
    //            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    //        }));
    //}

    public static void RegisterDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IPromotionService, PromotionService>();
        //services.AddScoped<IUserService, UserService>();
        //services.AddScoped<IConversationService, Services.Domains.ConversationService>();
    }

    public static void RegisterCommands(this IServiceCollection services)
    {
        var assembly = typeof(StartCommand).Assembly;

        // Get all types that are assignable to IBotCommand, are not abstract, and are not interfaces
        var commandTypes = assembly.GetTypes()
            .Where(t => typeof(IBotCommand).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
            .ToList();

        foreach (var commandType in commandTypes)
        {
            services.AddScoped(typeof(IBotCommand), commandType);
        }
    }
}