using Application.Common.CQRS;

namespace API.Extensions;

/// <summary>
/// Registra el despachador de comandos/consultas propio y escanea
/// los ensamblados de Application para registrar todos los handlers
/// (ICommandHandler / IQueryHandler) automáticamente — sin MediatR.
/// </summary>
public static class CqrsServiceExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        var applicationAssembly = typeof(Application.AssemblyReference).Assembly;

        RegisterHandlers(services, applicationAssembly, typeof(ICommandHandler<,>));
        RegisterHandlers(services, applicationAssembly, typeof(IQueryHandler<,>));

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, System.Reflection.Assembly assembly, Type openGenericInterface)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface)
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
    }
}
