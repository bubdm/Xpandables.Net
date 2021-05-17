# Xpandables.Net
Provides with useful interfaces contracts in **.Net 5.0** and some implementations mostly following the spirit of SOLID principles, CQRS...
The library is strongly-typed, which means it should be hard to make invalid requests and it also makes it easy to discover available methods and properties though IntelliSense.

Feel free to fork this project, make your own changes and create a pull request.

## Model definition

```cs
// Entity is the domain object base implementation that provides with an identifier and a key generator for derived class.
// You can use AggregateRoot if you're targeting DDD

public sealed class ContactModel : Entity
{
    public ContactModel(string name, string city, string address, string country)
      => Update(name, city, address, country);
      
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

// DataContext is the an abstract context class that inherits from DbContext (EFCore)
// and implements the IDataContext interface.
// IDataContext interface represents a set of commands to manage domain objects using EntityFrameworkCore.

 public sealed class ContactContext : DataContext
 {
     public ContactContext(DbContextOptions<ContactContext> contextOptions) : base(contextOptions) { }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
         modelBuilder.Entity<ContactModel>().HasKey(new string[] { nameof(ContactModel.Id) });
     }
     public DbSet<ContactModel> Contacts { get; set; } = default!;
 }
    
```

## Contracts definition

```cs

// HttpRestClientAttribute describes the parameters for a request used with IHttpRestClientHandler.
// IHttpRestClientHandler provides with methods to handle HTTP Rest client queries and commands 
// using a typed client HTTP Client,  which allows, in a .Net environment, to no longer define client 
// actions because they are already included in the contracts, 
// by implementing interfaces such as IPathStringLocationRequest, IFormUrlEncodedRequest,
// IMultipartRequest...
// QueryExpression{T} allows the derived class to be used for a where clause.

 [HttpRestClient(Path = "api/contacts/{id}", Method = "Get", IsSecured = false, 
    IsNullable = true, In = ParameterLocation.Path)]
 public sealed class Select([Required] string Id) :
    QueryExpression<ContactModel>, IQuery<Contact>, IHttpRestClientRequest<Contact>,
    IPathStringLocationRequest, IInterceptorDecorator
 {
     public Select(string id) => Id = id;
     public string Id {get; init; }
     public override Expression<Func<ContactModel, bool>> GetExpression() 
        => contact => contact.Id == Id && contact.IsActive && !contact.IsDeleted;
        
     public IDictionary<string, string> GetPathStringSource()
        => new Dictionary<string, string> { { nameof(Id), Id } };
 }
 
 ...
 
```

## Contracts Validation

```cs

// IValidator{T} defines method contracts used to validate a type-specific argument using a decorator.
// The validator get called during the control flow before the handler.
// If the validator returns a failed operation result, the execution will be interrupted
// and the result of the validator will be returned.
// We consider as best practice to handle common conditions without throwing exceptions
// and to design classes so that exceptions can be avoided.

public sealed class ContactValidators : OperationResultBase,
   IValidator<Select>, IValidator<Add>, IValidator<Delete>, IValidator<Edit>
{
    private readonly IEntityAccessor<ContactModel> _readEntityAccessor;
    public ContactValidators(IEntityAccessor<ContactModel> readEntityAccessor) 
      => _readEntityAccessor = readEntityAccessor ?? throw new ArgumentNullException(nameof(readEntityAccessor));

    public async Task<IOperationResult> ValidateAsync(
       Select argument, CancellationToken cancellationToken = default)
    {
        if (await _readEntityAccessor
           .TryFindAsync(argument, cancellationToken)
           .ConfigureAwait(false) is null)
            return NotFoundOperation(nameof(argument.Id), "Contact not found");
               
        return OkOperation();
    }
    
    ....
    
}

```

## Contracts Handlers

```cs

public sealed class SelectQueryHandler : QueryHandler<Select, Contact>
{
    private readonly IEntityAccessor<ContactModel> _entityAcessor;
    public ContactHandlers(
        IEntityAccessor<ContactModel> entityAccessor) 
       => _entityAccessor = entityAccessor;

    public async Task<IOperationResult<Contact>> HandleAsync(
       Select query, CancellationToken cancellationToken = default)
        => OkOperation<Contact>(
                await _entityAccessor
                 .TryFindAsync(query,
                  s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country), cancellationToken));
                  
   ...
   
}

```

## Controller

```cs

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class ContactsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public ContactsController(IDispatcher dispatcher) 
       => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    [Route("{id}", Name = "ContactLink")]
    [HttpGet]
    public async Task<IActionResult> SelectAsync(
        [FromRoute] Select select, CancellationToken cancellationToken = default)
        => Ok(await _dispatcher.FetchAsync(select, cancellationToken).ConfigureAwait(false));

    ...
        
    [HttpPatch]
    public async Task<IActionResult> EditAsync(
      [FromRoute] string id, [FromBody] JsonPatchDocument<Edit> editPatch, CancellationToken cancellationToken = default)
    {
        var edit = new Edit { Id = id, ApplyPatch = value => ApplyJsonPatch(value, editPatch) };
        return Ok(await _dispatcher.SendAsync(edit, cancellationToken).ConfigureAwait(false));
    }

    ...
        
}

```

## Test class

```cs

...

[TestMethod]
public async Task SelectTest()
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    var factory = new WebApplicationFactory<Program>(); // from the api
    var client = factory.CreateClient();
    using var httpRestClientHandler = new HttpRestClientHandler(new HttpRestClientNewtonsoftRequestBuilder(), new HttpRestClientNewtonsoftResponseBuilder(), client);

    var select = new Select("ADHK12JkDiKJD");
    using var response = await httpRestClientHandler.HandleAsync(selectAll).ConfigureAwait(false);

    if (!response.IsValid())
    {
         Trace.WriteLine($"{response.StatusCode}");
         return;
    }
    else
    {
        var contact = response.Result;
        Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");
    }
}

...

```
## Features
Usually, when registering types, we are forced to reference the libraries concerned and we end up with a very coupled set.
To avoid this, you can register these types by calling an export extension method, which uses **MEF: Managed Extensibility Framework**.

In your api startup class

```cs
// AddXServiceExport(IConfiguration, Action{ExportServiceOptions}) adds and configures registration of services using 
// the IAddServiceExport interface implementation found in the target libraries according to the export options.
// You can use configuration file to set up the libraries to be scanned.

public class Startup
{
    ....
    services
        .AddXpandableServices()
        .AddXServiceExport(Configuration, options => options.SearchPattern = "your-search-pattern-dll")
        .Build();
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
        services
            .AddXpandableServices()
            .AddXDispatcher()
            .AddXTokenEngine<TokenEngine>()
            .Build();
        ....
    }
}

```

## Decorator pattern
You can use the extension methods to apply the decorator pattern to your types.

```cs

// This method and its extensions ensure that the supplied TDecorator" decorator is returned, wrapping the original 
// registered "TService", by injecting that service type into the constructor of the supplied "TDecorator". 
// Multiple decorators may be applied to the same "TService". By default, a new "TDecorator" instance 
// will be returned on each request, 
// independently of the lifestyle of the wrapped service. Multiple decorators can be applied to the same service type. 
// The order in which they are registered is the order they get applied in. This means that the decorator 
// that gets registered first, gets applied first, which means that the next registered decorator, 
// will wrap the first decorator, which wraps the original service type.

 services
    .AddXpandableServices()
    .XTryDecorate<TService, TDecorator>()
    .Build();
   
```

Suppose you want to add logging for the Add command ...

```cs

// The Add command definition

[HttpRestClient(Path = "api/contacts", Method = "Post", IsSecured = false)]
public sealed class Add() : QueryExpression<ContactModel>, ICommand<string>, IValidatorDecorator, IPersistenceDecorator, IInterceptorDecorator
{
    public Add(string name, string city, string address, string country)
    {
        Name = name;
        City = city;
        Address = address;
        Country country;
    }

    public string Name { get; init; }
    public string City { get; init; }
    pubcli string Address { get; init; }
    public string Country { get; init;}

    public override Expression<Func<ContactModel, bool>> GetExpression()
      => contact => contact.Name == Name && contact.City == City && contact.Country == Country;
}

// The Add command handler

public sealed class AddHandler : CommandHandler<Add, string>
{
    public async Task<IOperationResult<string>> HandleAsync(
       Add command, CancellationToken cancellationToken = default)
    {
        .....
        return OkOperation<string>(...);
    }
}

// The Add command decorator for logging

public sealed class AddHandlerLoggingDecorator : ICommandHandler<Add, string>
{
    private readonly ICommandHandler<Add, string> _ decoratee;
    private readonly ILogger<Add> _logger;
    public AddHandlerLoggingDecorator(ILogger<Add> logger, ICommandHandler<Add, string> decoratee)
      =>(_logger, _ decoratee) = (logger, decoratee);

    public async Task<IOperationResult<string>> HandleAsync(
        Add command, CancellationToken cancellationToken = default)
    {
        _logger.Information(...);
        var response = await _decoratee.HandleAsync(command, cancellationToken).configureAwait(false);
        _logger.Information(...)
        return response;
    }
}

// Registration

services
    .AddXpandableServices()
    .XtryDecorate<AddHandler, AddHandlerLoggingDecorator>()
    .Build();

// or you can define the generic model, for all commands that implement ICommand interface or something else.

public sealed class CommandLoggingDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
   where TCommand : class, ICommand<TResult> // you can add more constraints
{
    private readonly ICommandHandler<TCommand, TResult> _ decoratee;
    private readonly ILogger<TCommand> _logger;
    public CommandLoggingDecorator(ILogger<TCommand> logger, ICommandHandler<TCommand, TResult> decoratee)
      =>(_logger, _ decoratee) = (logger, decoratee);

    public async Task<IOperationResult<TResult>> HandleAsync(
         TCommand command, CancellationToken cancellationToken = default)
    {
        _logger.Information(...);
        var response = await _decoratee.HandleAsync(command, cancellationToken).configureAwait(false);
        _logger.Information(...)
        return response;
    }
}

// and register

// The CommandLoggingDecorator will be applied to all command handlers whose commands meet the decorator's constraints : 
// To be a class and implement ICommand{TResult} interface

services
    .AddXpandableServices()
    .XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandLoggingDecorator<,>))
    .Build();

// or You must provide with an implementation of "IOperationResultLogger" and use the registration as follow

services
    .AddXpandableServices()
    .AddXHandlers(assemblyCollection, options=>
    {
        options.UseOperationResultLoggerDecorator();
    })
    .Build();

// .AddXHandlers will register all handlers (ICommandhandler{}, IQueryHandler{}, IAsyncQueryHandler{}) adding a decorator for each
// handler, that will apply your implementation of ILoggingHandler for commands implementing ILoggingDecorator
.
```

