# Xpandables.Net
Provides with useful interfaces contracts in **.Net 5.0** and some implementations mostly following the spirit of SOLID principles, CQRS...
The library is strongly-typed, which means it should be hard to make invalid requests and it also makes it easy to discover available methods and properties though IntelliSense.

Feel free to fork this project, make your own changes and create a pull request.

See [Samples](https://github.com/Francescolis/Xpandables.Net/tree/Net5.0/Samples/Xpandables.Net.Api)

## Type registration
Usually, when registering types, we are forced to reference the libraries concerned and we end up with a very coupled set. To avoid this, you can register these types by calling an export extension method, which uses the principle of **MEF: Managed Extensibility Framework**.

In your api startup class
```cs
public class Startup
{
    ....
    services.AddXServiceExport(Configuration, options => options.SearchPattern = "your-search-pattern-dll");
    ...
}
```
In the library you want types to be registered

```cs
[Export(typeof(IAddServiceExport))]
public sealed class RegisterServiceExport : IAddServiceExport
{
    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddXDispatcher();
        services.AddXTokenEngine<TokenEngine>();
        ....
    }
}
```

* [AddXServiceExport](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net.DependencyInjection/Extensibility/ServiceCollectionExtensions.cs) adds and configures registration of services using the [IAddServiceExport](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net.DependencyInjection/Extensibility/IAddServiceExport.cs) implementations found in the path according to the [ExportServiceOptions](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net.DependencyInjection/Extensibility/ExportServiceOptions.cs) definition.
You can use the configuration file to set up the libraries to be scanned.

## Using decorator pattern
You can use the extension methods to apply the decorator pattern to your types.

```cs
   services.XTryDecorate<TService, TDecorator>();
```

This method and its extensions ensure that the supplied TDecorator" decorator is returned, wrapping the original registered "TService", by injecting that service type into the constructor of the supplied "TDecorator". Multiple decorators may be applied to the same "TService". By default, a new "TDecorator" instance will be returned on each request, independently of the lifestyle of the wrapped service. Multiple decorators can be applied to the same service type. The order in which they are registered is the order they get applied in. This means that the decorator that gets registered first, gets applied first, which means that the next registered decorator, will wrap the first decorator, which wraps the original service type.

Suppose you want to add logging for the SingOnCommand ...

```cs
public sealed record SignOnCommand(string Name, string Password) : ICommand;

public sealed class SignOnCommandHandler : ICommandHandler<SignOnCommand>
{
    public async Task<IOperationResult> HandleAsync(
       SignOnCommand command, CancellationToken cancellationToken = default)
    {
        .....
        return new SuccessOperationResult();
    }
}

// The decorator for logging

public sealed class SignOnCommandLoggingDecorator : ICommandHandler<SignOnCommand>
{
    private readonly ICommandHandler<SignOnCommand> _ decoratee;
    private readonly ILogger<SignOnCommand> _logger;
    public SignOnCommandLoggingDecorator(ILogger<SignOnCommand> logger, ICommandHandler<SignOnCommand> decoratee)
      =>(_logger, _ decoratee) = (logger, decoratee);

    public async Task<IOperationResult> HandleAsync(
        SignOnCommand command, CancellationToken cancellationToken = default)
    {
        _logger.Information(...);
        var response = await _decoratee.HandleAsync(command, cancellationToken).configureAwait(false);
        _logger.Information(...)
        return response;
    }
}

// Registration
services.XtryDecorate<SignOnCommandHandler, SignOnCommandLoggingDecorator>();

// or you can define the generic model
public sealed class CommandLoggingDecorator<TCommand> : ICommandHandler<TCommand>
   where TCommand : class, ICommand // you can add more constraints
{
    private readonly ICommandHandler<TCommand> _ decoratee;
    private readonly ILogger<TCommand> _logger;
    public CommandLoggingDecorator(ILogger<TCommand> logger, ICommandHandler<TCommand> decoratee)
      =>(_logger, _ decoratee) = (logger, decoratee);

    public async Task<IOperationResult> HandleAsync(
         TCommand command, CancellationToken cancellationToken = default)
    {
        _logger.Information(...);
        var response = await _decoratee.HandleAsync(command, cancellationToken).configureAwait(false);
        _logger.Information(...)
        return response;
    }
}

// and register
services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandLoggingDecorator<>));

```
