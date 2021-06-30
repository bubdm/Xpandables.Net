# Xpandables.Net
Provides with useful interfaces contracts in **.Net 5.0** and some implementations mostly following the spirit of SOLID principles, CQRS...
The library is strongly-typed, which means it should be hard to make invalid requests and it also makes it easy to discover available methods and properties though IntelliSense.

Feel free to fork this project, make your own changes and create a pull request.

Here are some examples of use :

# Web Api using CQRS and EFCore

Add the following nuget packages :
    Xpandables.Net.AspNetCore
    Xpandables.Net.EntityFramework

## Model Definition

```cs
// Entity is the domain object base implementation that provides with an Id,
// key generator for id and some useful state methods.
// You can use Aggregate{TAggregateId} if you're targeting DDD.

public sealed class PersonEntity : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    
    public static PersonEntity NewPerson(string firstName, string lastName)
    {
        // custom check
        return new(fistName, lastname);
    }
    
    public void ChangeFirstName(string firstName)
        => FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
    
    public void ChangeLastName(string lastName)
        => LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
    
    private PersonEntity(string firstName, string lastName)
        => (FirstName, Lastname) = (firstName, lastName);
}

```

## Contract definition

```cs
// Contract is decorated with HttpRestClientAttribute that describes the parameters for a request 
// used with IHttpRestClientHandler, where IHttpRestClientHandler provides with methods to handle HTTP
// Rest client queries and commands using a typed client HTTP Client. It also allows, in a .Net environment,
// to no longer define client actions because they are already included in the contracts, 
// by implementing interfaces such as IPathStringLocationRequest, IFormUrlEncodedRequest,
// IMultipartRequest, IStringRequest, IStreamRequest...
// Without the use of one of those interfaces, the whole class will be serialized.


[HttpRestClient(Path = "api/person", Method = HttpMethodVerbs.Post, IsSecured = false)]
public sealed class AddPersonRequest : IHttpRestClientRequest<CreatedId>
{    
    [Required]
    public string FirstName { get; init; }
    [Required]
    public string LastName { get; init; } 
    
    public AddPersonRequest(string firstName, string lastName)
        => (FirstName, LastName) = (firstName, lastName);
}

[HttpRestClient(Path = "api/person/{id}", IsNullable = true, In = ParameterLocation.Path, 
    Method = HttpMethodVerbs.Get, IsSecured = false)]
public sealed class GetPersonRequest : IHttpRestClientRequest<Person>,
    IPathStringLocationRequest
{
    public string Id { get; init;}
    
    public GetPersonRequest(string id)
        => Id = id;
    
    public IDictionary<string, string> GetPathStringSource()
        => new Dictionary<string, string> { { nameof(Id), Id } };
}

public sealed record Person(string FirstName, string LastName);
public sealed record CreatedId(string Id);

```

## Command/Query and Handler definitions

```cs

// The command must implement the ICommand interface and others to enable their behaviors.
// Such as IValidatorDecorator to apply validation before processing the command,
// IPersistenceDecorator to add persistence to the control flow
// or IInterceptorDecorator to add interception of the command process...

// You can derive from QueryExpression{TClass} to allow command to behave like an expression
// when querying data, and override the target method.

public sealed class AddPersonCommand : QueryExpression<Person>, ICommand<CreatedId>,
    IValidatorDecorator, IPersistenceDecorator
{
    public string FirstName { get; }
    public string LastName { get; }
    
    public override Expression<Func<Person>, bool>> GetExpression()
        => person => person.FistName == FistName
            && person.LastName == LastName;
}

public sealed class GetPersonQuery : QueryExpression<Person>, IQuery<Person>
{
    public string Id { get; }
    
    public GetPersonQuery(string id) => Id= id;
    
    public override Expression<Func<Person>, bool>> GetExpression()
        => person => person.Id == Id;    
}

// CommandHandler{TCommand}, CommandHandler{TCommand, TResult} and QueryHandler{TResult} are abstract classes
// that implement ICommandHandler{TCommand}, ICommandHandler{TCommand, TResult} and IQueryHandler{TResult}
// and derive from OperationResults : a class that contains some usefull methods
// to return response with HTTP status code.

// IEntityAccessor{TEntity} is a generic interface that provides with methods to access
// data from a storage.

public sealed class AddPersonCommandHandler : CommandHandler<AddPersonCommand, CreatedId>
{
    private readonly IEntityAccessor<Person> _entityAccessor;
    public AddPersonCommandHandler(IEntityAccessor<Person> entityAccessor)
        => _entityAccessor = entityAccessor;
    
    public override async Task<IOperationResult<CreatedId>> HandleAsync(AddPersonCommand command, 
        CancellationToken cancellationToken = default)
    {
        // You can check here for data validation or use a specific class for that
        // (see AddPersonCommandValidationDecorator).
        
        var newPerson = Person.NewPerson(
            command.FirstName,
            command.LastName);
        
        await _entityAccessor.InsertAsync(newPerson, cancellationToken).configureAwait(false);
        
        return OkOperation(new CreatedId(newPerson.Id));
        
        // Note that data will be saved at the end of the control flow
        // if there is no error. The OperationResultFilter will process the output message format.
        // You can add a decorator class to manage the exception.
    }
}

public sealed class GetPersonQueryHandler : QueryHandler<GetPersonQuery, Person>
{
    private readonly IEntityAccessor<Person> _entityAccessor;
    public GetPersonQueryHandler(IEntityAccessor<Person> entityAccessor)
        => _entityAccessor = entityAccessor;    
    
    public override async Task<IOperationResult<Person>> HandleAsync(GetPersonQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _entityAccessor.TryFindAsync(
            query,
            cancellationToken)
            .configureAwait(false);
        
        return result switch
        {
            { } person => OkOperation(new Person(person.FirstName, person.LastName)),
            _ => NotFoundOperation<Person>(nameof(query.Id), "Id not found.")
        };
    }        
}

// When using validation decorator.
// Validator{T} defines a method contract used to validate a type-specific argument using a decorator.
// The validator get called during the control flow before the handler.
// If the validator returns a failed operation result, the execution will be interrupted
// and the result of the validator will be returned.
// We consider as best practice to handle common conditions without throwing exceptions
// and to design classes so that exceptions can be avoided.

public sealed class AddPersonCommandValidationDecorator : Validator<AddPersonCommand>
{
    private ready IEntityAccessor<Person> _entityAccessor;
    public AddPersonCommandValidationDecorator(IEntityAccessor<Person> entityAccessor)
        => _entityAccessor = entityAccessor;
    
    public override async Task<IOperationResult> ValidateAsync(AddPersonCommand argument, 
        CancellationToken cancellationToken)
    {
        return await _entityAccessor.TryFindAsync(argument, cancellationToken).configureAwait(false) switch
        {
            { } => BadOperation(nameof(argument.FirstName), "Already exist"),
            null => OkOperation()
        };
    }
    
    // BadOperation, NotFoundOperation and OkOperation are Http operation results
    // found in the OperationResuls base class.
}

```

## Context definition

```cs

// We are using EFCore

public sealed class PersonEntityTypeConfiguration : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FirstName);
        builder.Property(p => p.LastName);
    }
}

// DataContext is the an abstract context class that inherits from DbContext (EFCore)
// and implements the IDataContext interface.
// IDataContext interface represents a set of commands to manage domain objects using EntityFrameworkCore.

public sealed class PersonContext : DataContext
{
     public PersonContext(DbContextOptions<PersonContext> contextOptions)
        : base(contextOptions) { }
        
     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
         modelBuilder.ApplyConfiguration(new PersonEntityTypeConfiguration());
     }
     
     public DbSet<PersonEntity> People { get; set; } = default!;
}

```

## Controller definition

```cs
// IDispatcher provides with methods to discover registered handlers at runtime.
// We consider as best practice to return ValidationProblemDetails/ProblemDetails in case of errors

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class PersonController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    
    public PersonController(IDispatcher dispatcher) 
       => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreatedId))]
    public async Task<IActionResult> AddPersonAsync(
        [FromBody] AddPersonRequest request, CancellationToken cancellationToken = default)
    {
        var command = new AddPersonCommand(request.FirstName, request.LastName);
        return Ok(await _dispatcher.SendAsync(command, cancellationToken).ConfigureAwait(false));
    }
    
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Person))]
    public async Task<IActonResult> GetPersonAsync(
        [FromRoute] GetPersonRequest request, cancellationToken cancellationToken = default)
    {
        var query = new GetPersonQuery(request.Id);
        return Ok(await _dispatcher.FetchAsync(query, cancellationToken).ConfigureAwait(false));
    }    

    // ...
        
}

// The startup class
// We will register handlers, context, validators and decorators.

public sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // common registrations
        
        services.AddXpandableServices()
            .AddXDataContext<PersonContext>(options => options.UseInMemoryDatabase(nameof(PersonContext))
            .AddXDispatcher()
            .AddXHandlerAccessor()
            .AddXEntityAccessor()
            .AddXOperationResultFilter()
            .AddXHandlers(
                new[] { typeof(AddPersonCommandHandler).Assembly },
                options =>
                {
                    options.UsePersistenceDecorator();
                    options.UseValidatorDecorator();
                })
            .Build();
        
        services
            .AddControllers()
            .AddMvcOptions(options => options.Filters.Add<OperationResultFilter>(int.MinValue));
        
        // AddXpandableServices() will make available methods for registration
        // AddXDataContext{TContext} registers the TContext and make it available for IEntityAccessor as IDataContext
        // AddXDispatcher() registers the dispatcher
        // AddXHandlerAccessor() registers a handlers accessor using the IHandlerAccessor interface
        // AddXEntityAccessor() register the IEntityAccessor{TEntity} interface.
        // AddXOperationResultFilter() register the OperationResultFilter use with MvcOptions Filters.
        // AddXHandlers(assemblies, options) registers all handlers and associated classes (validators, decorators...)
        // according to the options set.
        
        // ...
    }    
}

```

## Wep Api Test class

```cs

[TestMethod]
[DataRow("My FirstName", "My LastName")
public async Task AddPersonTestAsync(string firstName, string lastName)
{
    // Build the api client
    
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    var factory = new WebApplicationFactory<Program>(); // from the api
    var client = factory.CreateClient();
    
    // if you get serialization error (due to System.Text.Json), you can
    // set the serialization options by using an extension method or by globally
    // setting the IHttpRestClientHandler.SerializerOptions property.
    // By default, the options are set to Web definition.
    
    using var httpRestClientHandler = new HttpRestClientHandler(
        new HttpRestClientRequestBuilder(),
        new HttpRestClientResponseBuilder(),
        client);

    var addPersonRequest = new AddPersonRequest(firstName, lastName);
    using var response = await httpRestClientHandler.HandleAsync(addPersonRequest).ConfigureAwait(false);

    if (!response.IsValid())
    {
         Trace.WriteLine($"{response.StatusCode}");
         var errors = response.GetBadOperationResult().Errors;
         
         // GetBadOperationResult() is an extension method for HttpRestClientResponse that returns
         // a failed IOperationResult from the response.
         
         foreach (var error in errors)         
         {
            Trace.WriteLine($"Key : {error.Key}");
            Trace.WriteLine(error.ErrorMessages.StringJoin(";"));
         }         
    }
    else
    {
        var createdId = response.Result;
        Trace.WriteLine($"Added person : {createdId.Id}");
    }
}

```

# Blazor WebAss with Web Api using IHttpRestClientHandler

## Blazor WebAss project

Add the following nuget packages :
    Xpandables.Net.BlazorExtended

In the Program file, replace the default code with this.

```cs

public class Program
{
    public static async Task Main(string[] args)
    {
       var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        
        builder.Services
            .AddOptions()
            .AddXpandableServices()
                .AddXHttpRestClientHandler(httpClient =>
                {
                    httpClient.BaseAddress = new Uri("https://localhost:44396"); // your api url
                    httpClient.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue(ContentType.Json));
                })
            .Build();
        
        // AddXHttpRestClientHandler(httpClient) will add the IHttpRestClientHandler
        // implementation using the HttpClient with your configuration.
        // if you get errors with System.Text.Json, you can use IHttpRestClientHandler.SerializerOptions
        // property to globally set the serializer options or use extension methods.
                
        // custom code
        
        await builder.Build().RunAsync();
    }
}

```

## AddPerson.razor

```html

<EditForm Model="@model" OnValidSubmit="AddSubmitAsync">

    <DataAnnotationsValidatorExtended @ref="@Validator" />

    <div class="form-group">
        <label for="FirstName" class="-form-label">First Name</label>
        <InputTextOnInput @bind-Value="model.FirstName" type="text" class="form-control" />
        <ValidationMessage For="@(() => model.FirstName)" />
    </div>

    <div class="form-group">
        <label for="LastName" class="col-form-label">Last Name</label>
        <InputTextOnInput @bind-Value="model.LastName" type="text" class="form-control" />
        <ValidationMessage For="@(() => model.LastName)" />
    </div>

    <div class="form-group">
        <div class="col-md-12 text-center">
            <button class="col-md-12 btn btn-primary">
                Add
            </button>
        </div>
    </div>

</EditForm>

```

**InputTextOnInput** is a component that allows text to be validated on input.
**DataAnnotationsValidatorExtended** is a DataAnnotationsValidator derived class that allows
insertion of external errors to the edit context.

## AddPerson.razor.cs


```cs

public sealed class PersonModel
{
    [Required]
    public string FirstName { get; set; } = default!;
    [Required]
    public string LastName { get; set; } = default!;
}

public partial class AddPerson
{
    protected DataAnnotationsValidatorExtended Validator { get; set; } = default!;
    [Inject]
    protected IHttpRestClientHandler HttpRestClientHandler { get; set; } = default!;
    
    private readonly PersonModel model = new();
    
    protected async Task AddSubmitAsync()
    {
        // You can use the AddPersonRequest from the api or create another class
        // We do not specifiy the action there because the AddPersonRequest definition
        // already hold all the necessary information.
        
        var addRequest = new AddPersonRequest(model.FirstName, model.LastName);
        var addResponse = await HttpRestClientHandler.SendAsync(addRrequest).ConfigureAwait(false);

        if (addResponse.Failed)
        {
            Validator.ValidateModel(addResponse);
        }
        else
        {
            // custom code like displaying the result
            var createdId = addResponse.Result;
        }

        StateHasChanged();
    }    
}

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

Suppose you want to add logging for the AddPersonCommand ...
(There is already a built in logging decorator if you want to use it)

```cs

// The AddPersonCommand decorator for logging

public sealed class AddPersonCommandHandlerLoggingDecorator : 
    ICommandHandler<AddPersonCommand, CreatedPerson>
{
    private readonly ICommandHandler<AddPersonCommand, CreatedId> _ decoratee;
    private readonly ILogger<AddPersonCommandHandler> _logger;
    
    public AddPersonCommandHandlerLoggingDecorator(
        ILogger<AddPersonCommandHandler> logger,
        ICommandHandler<AddPersonCommand, CreatedId> decoratee)
        => (_logger, _ decoratee) = (logger, decoratee);

    public async Task<IOperationResult<CreatedId>> HandleAsync(
        AddPersonCommand command, CancellationToken cancellationToken = default)
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
    .XtryDecorate<AddPersonCommandHandler, AddPersonCommandHandlerLoggingDecorator>()
    .Build();

// or you can define the generic model, for all commands that implement ICommand 
// interface or something else.

public sealed class CommandLoggingDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : class, ICommand<TResult> // you can add more constraints
{
    private readonly ICommandHandler<TCommand, TResult> _ decoratee;
    private readonly ILogger<TCommand> _logger;
    
    public CommandLoggingDecorator(ILogger<TCommand> logger, ICommandHandler<TCommand, TResult> decoratee)
        => (_logger, _ decoratee) = (logger, decoratee);

    public async Task<IOperationResult<TResult>> HandleAsync(
         TCommand command, CancellationToken cancellationToken = default)
    {
        _logger.Information(...);
        
        var response = await _decoratee.HandleAsync(command, cancellationToken).configureAwait(false);
        
        _logger.Information(...)
        
        return response;
    }
}

// and for registration

// The CommandLoggingDecorator will be applied to all command handlers whose commands meet 
// the decorator's constraints : 
// To be a class and implement ICommand{TResult} interface

services
    .AddXpandableServices()
    .XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandLoggingDecorator<,>))
    .Build();

// or You must provide with an implementation of "ICommandQueryLogger" and use the registration as follow

services
    .AddXpandableServices()
    .AddXHandlers(assemblyCollection, options =>
    {
        options.UseCommandQueryLoggerDecorator();
    })
    .Build();

// .AddXHandlers will register all handlers (ICommandhandler{}, IQueryHandler{}, IAsyncQueryHandler{}...)
// adding a decorator for each  handler, and will apply your implementation of ICommandQueryLogger
// for commands/queries implementing ILoggingDecorator
.
```

# Others

Librairies also provide with Aggregate model implementation using event sourcing and out-box pattern.
