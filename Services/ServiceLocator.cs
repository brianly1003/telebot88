namespace W88.TeleBot.Services;

public class ServiceLocator(IServiceScopeFactory serviceScopeFactory)
{
    public IEnumerable<T> GetServices<T>()
    {
        using var scope = serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<T>();
    }
}