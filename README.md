# Xpandables.Net
Provides with useful interfaces contracts in **.Net 5.0** and some implementations mostly following the spirit of SOLID principles, CQRS...
The library is strongly-typed, which means it should be hard to make invalid requests and it also makes it easy to discover available methods and properties though IntelliSense.

Feel free to fork this project, make your own changes and create a pull request.

See [Samples](https://github.com/Francescolis/Xpandables.Net/tree/Net5.0/Samples/Xpandables.Net.Api) for more details.

## Model definition

```cs
// Entity is the domain object base implementation that provides with an identifier and a key generator for derived class.

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
         modelBuilder.Entity<ContactModel>().HasIndex(new string[] { nameof(ContactModel.Id) }).IsUnique();
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
// RecordExpression{T} allows the derived class to be used for a where clause.

 [HttpRestClient(Path = "api/contacts/{id}", Method = "Get", IsSecured = true, 
    IsNullable = true, In = ParameterLocation.Path)]
 public sealed record Select([Required] string Id) :
    RecordExpression<ContactModel>, IQuery<Contact>, IPathStringLocationRequest, IInterceptorDecorator
 {
     public override Expression<Func<ContactModel, bool>> GetExpression() 
        => contact => contact.Id == Id && contact.IsActive && !contact.IsDeleted;
        
     public IDictionary<string, string> GetPathStringSource()
        => new Dictionary<string, string> { { nameof(Id), Id } };
 }
 
 ...
 
```

## Contracts Validation

```cs

// IValidation{T} defines method contracts used to validate a type-specific argument using a decorator.
// The validator get called during the control flow before the handdler.
// If the validator returns a failed operation result, the execution will be interrupted
// and the result of the validator will be returned.

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
           .FindAsync(c => c. AsNoTracking().Where(argument).OrderBy(o => o.Id), cancellationToken)
           .ConfigureAwait(false) is null)
            return new FailedOperationResult(
               System.Net.HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");
               
        return new SuccessOperationResult();
    }
    
    ....
    
    public async Task<IOperationResult> ValidateAsync(
       Edit argument, CancellationToken cancellationToken = default)
    {
        if(await _dataContext
           .FindAsync(c => c.AsNoTracking().Where(argument).OrderBy(o => o.Id), cancellationToken)
           .ConfigureAwait(false) is not { } contact)
           return new FailedOperationResult(
              System.Net.HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");

        argument.Name = contact.Name;
        argument.City = contact.City;
        argument.Address = contact.Address;
        argument.Country = contact.Country;

        return argument.ApplyPatch(argument);
    }
}

```

## Contracts Handlers

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
                 .FindAsync(c => c.AsNoTracking().Where(query).OrderBy(o => o.Id)
                  .Select(s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country)), cancellationToken))!);
                  
   ...
   
   public async Task<IOperationResult<Contact>> HandleAsync(
      Edit command, CancellationToken cancellationToken = default)
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
public async Task SelectAllTest()
{
   var selectAll = new SelectAll();
   using var response = await httpRestClientHandler.HandleAsync(selectAll).ConfigureAwait(false);

   if (!response.IsValid())
   {
      Trace.WriteLine($"{response.StatusCode}");
         return;
   }
    else
       await foreach (var contact in response.Result)
         Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");
}

...

```
## Features
Usually, when registering types, we are forced to reference the libraries concerned and we end up with a very coupled set.
To avoid this, you can register these types by calling an export extension method, which uses **MEF: Managed Extensibility Framework**.

In your api startup class

```cs
// AddXServiceExport(IConfiguration, Action{ExportServiceOptions}) adds and configures registration of services using 
// the IAddServiceExport interface implementation found the target libraries according to the export options.
// You can use configuration file to set up the libraries to be scanned.

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

 services.XTryDecorate<TService, TDecorator>();
   
```

Suppose you want to add logging for the Add command ...

```cs

// The Add command definition

[HttpRestClient(Path = "api/contacts", Method = "Post", IsSecured = false)]
public sealed record Add([Required] string Name, [Required] string City, [Required] string Address, [Required] string Country) :
   RecordExpression<ContactModel>, ICommand<string>, IValidationDecorator, IPersistenceDecorator, IInterceptorDecorator
{
    public override Expression<Func<ContactModel, bool>> GetExpression()
      => contact => contact.Name == Name && contact.City == City && contact.Country == Country;
}

// The Add commnand handler

public sealed class AddHandler : ICommandHandler<Add, string>
{
    public async Task<IOperationResult<string>> HandleAsync(
       Add command, CancellationToken cancellationToken = default)
    {
        .....
        return new SuccessOperationResult<string>(...);
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

services.XtryDecorate<AddHandler, AddHandlerLoggingDecorator>();

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

services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandLoggingDecorator<,>));

```
