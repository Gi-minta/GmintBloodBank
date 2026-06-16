using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.CQRS;

/// <summary>
/// Implementación propia del despachador de comandos basada en
/// resolución dinámica de servicios (reemplaza el pipeline de MediatR).
/// </summary>
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));

        var handler = _serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException($"No se encontró HandleAsync para {handlerType.Name}");

        var result = method.Invoke(handler, new object[] { command, cancellationToken })
            ?? throw new InvalidOperationException("El handler retornó null.");

        return (Task<TResponse>)result;
    }
}
