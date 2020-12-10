# Xpandables.Net
Provides with useful interfaces contracts in **.Net 5.0** and some implementations mostly following the spirit of SOLID principles, CQRS...
The library is strongly-typed, which means it should be hard to make invalid requests and it also makes it easy to discover available methods and properties though IntelliSense.

Feel free to fork this project, make your own changes and create a pull request.

See [Samples](https://github.com/Francescolis/Xpandables.Net/tree/Net5.0/Samples/Xpandables.Net.Api) for more details.

## Model definition

```cs
// Entity is the domain object base implementation that provides an identifier and a key generator for derived class.

public sealed class ContactModel : Entity
{
    public ContactModel(string name, string city, string address, string country) => Update(name, city, address, country);
    [MemberNotNull(nameof(Name), nameof(City), nameof(Address), nameof(Country))]
    public void Update(string name, string city, string address, string country)
    {
        UpdateName(name);
        UpdateCity(city);
        UpdateAddress(address);
        UpdateCountry(country);
    }
    public void Edit(string? name, string? city, string? address, string? country)
    {
        if (name is not null) UpdateName(name);
        if (city is not null) UpdateCity(city);
        if (address is not null) UpdateAddress(address);
        if (country is not null) UpdateCountry(country);
    }
    
    ...
    
}
```

## Context definition

```cs

// DataContext is the an abstract context class that inherits from DbContext (EFCore) and implements the IDataContext interface.
// IDataContext interface represents a set of commands to manage domain objects using EntityFrameworkCore.

 public sealed class ContactContext : DataContext
 {
     public ContactContext(DbContextOptions<ContactContext> contextOptions) : base(contextOptions) { }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
         modelBuilder.Entity<ContactModel>().HasKey(new string[] { nameof(ContactModel.Id) });
         modelBuilder.Entity<ContactModel>().HasIndex(new string[] { nameof(ContactModel.Id) }).IsUnique();
     }
     public DbSet<ContactModel> Contacts { get; set; } = default!;
 }
    
```

## Contracts definition

```cs

// HttpRestClientAttribute describes the parameters for a request used with IHttpRestClientHandler.
// IHttpRestClientHandler provides with methods to handle HTTP Rest client queries and commands using a typed client HTTP Client, 
// which allows, in a .Net environment, to no longer define client actions because they are already included in the contracts, 
// by implementing interfaces such as IPathStringLocationRequest, IFormUrlEncodedRequest, IMultipartRequest...
// QueryExpression{T} allows the derived class to be used for a where clause.

 [HttpRestClient(Path = "api/contacts/{id}", Method = "Get", IsSecured = true, IsNullable = true, In = ParameterLocation.Path)]
 public sealed class Select : QueryExpression<ContactModel>, IQuery<Contact>, IPathStringLocationRequest, IInterceptorDecorator
 {
     public override Expression<Func<ContactModel, bool>> GetExpression() 
        => contact => contact.Id == Id && contact.IsActive && !contact.IsDeleted;
     public Select(string id) => Id = id;
     public Select() => Id = null!;
     
     public string Id { get; set; }
     public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), Id } };
 }
 
 ...
 
```

## ContractValidation

```cs

// IValidation{T} defines method contracts used to validate a type-specific argument using a decorator.
The validator get called during the control flow before the handdler.

public sealed class ContactValidators : 
   IValidation<Select>, IValidation<Add>, IValidation<Delete>, IValidation<Edit>
{
    private readonly IDataContext<ContactModel> _dataContext;
    public ContactValidators(IDataContext<ContactModel> dataContext) 
      => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

    public async Task<IOperationResult> ValidateAsync(
       Select argument, CancellationToken cancellationToken = default)
    {
        if (await _dataContext
           .FindAsync(c => c.Where(argument).OrderBy(o => o.Id), cancellationToken)
           .ConfigureAwait(false) is null)
            return new FailedOperationResult(
               System.Net.HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");
               
        return new SuccessOperationResult();
    }
    
    ....
    
    public async Task<IOperationResult> ValidateAsync(
       Edit argument, CancellationToken cancellationToken = default)
    {
        var contact = await _dataContext
           .FindAsync(c => c.Where(argument).OrderBy(o => o.Id), cancellationToken)
           .ConfigureAwait(false);
           
        if (contact is null) return new FailedOperationResult(
            System.Net.HttpStatusCode.NotFound, nameof(argument.Name), "Contact not found");

        argument.Name = contact.Name;
        argument.City = contact.City;
        argument.Address = contact.Address;
        argument.Country = contact.Country;

        return argument.ApplyPatch(argument);
    }
}

```

## ContractHandlers

```cs

public sealed class ContactHandlers :
   IAsyncQueryHandler<SelectAll, Contact>, IQueryHandler<Select, Contact>, ICommandHandler<Add, string>, 
   ICommandHandler<Delete>, ICommandHandler<Edit, Contact>
{
    private readonly IDataContext<ContactModel> _dataContext;
    public ContactHandlers(IDataContext<ContactModel> dataContext) 
       => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

    public async Task<IOperationResult<Contact>> HandleAsync(
       Select query, CancellationToken cancellationToken = default)
        => new SuccessOperationResult<Contact>(
                (await _dataContext
                 .FindAsync(c => c.Where(query).OrderBy(o => o.Id)
                  .Select(s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country)), cancellationToken))!);
                  
   ...
   
   public async Task<IOperationResult<Contact>> HandleAsync(Edit command, CancellationToken cancellationToken = default)
   {
       var toEdit = (await _dataContext.FindAsync(c => c.Where(command).OrderBy(o => o.Id), cancellationToken)
                     .ConfigureAwait(false))!;
       toEdit.Edit(command.Name, command.City, command.Address, command.Country);

       await _dataContext.UpdateEntityAsync(toEdit, cancellationToken).ConfigureAwait(false);
       return new SuccessOperationResult<Contact>(
          new Contact(toEdit.Id, toEdit.Name, toEdit.City, toEdit.Address, toEdit.Country));
   }
}

```
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
