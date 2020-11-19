# Xpandables.Net5.0
Provides with useful interfaces contracts in **.Net 5.0** and some implementations mostly following the spirit of SOLID principles, such as [IAsyncCommandHandler](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Commands/IAsyncCommandHandler.cs), [IDispatcher](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Dispatchers/IDispatcher.cs), [IAsyncQueryHandler](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Queries/IAsyncQueryHandler.cs), [IHttpRestClientHandler](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/HttpRestClient/IHttpRestClientHandler.cs) and so on.

Feel free to fork this project, make your own changes and create a pull request.

## Available types
- [Entity](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Entity.cs) that contains basic properties for all entities.
- [IAddable{T}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/IAddable.cs) is a useful interface when implementing a serializable custom collection with JSON.
- [ICanHandle{T}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/ICanHandle.cs) Provides a method that determines whether or not an argument can be handled.
- [ValueRange{T}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/ValueRange.cs) Defines a pair of values, representing a segment.
- [IInstanceCreator](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/IInstanceCreator.cs) Provides with methods to create instance of specific type at runtime using delegate and cache.
- [IStringCryptography](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/IStringCryptography.cs) Provides with methods to encrypt and decrypt string values.
- [IStringGenerator](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/IStringGenerator.cs) Provides with methods to generate strings.
- [ValueEncrypted](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/ValueEncrypted.cs) Defines a representation of an encrypted value, its key and its salt used with [IStringCryptography](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/IStringCryptography.cs).
- [IAsyncCommand](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/CQRS/IAsyncCommand.cs) is used as marker for commands when using the command pattern.
- [IAsyncCommandHandler{TCommand}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/CQRS/IAsyncCommandHandler.cs) defines a handler for a specific type command.
- [IAsyncCorrelationContext](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Correlation/IAsyncCorrelationContext.cs) defines two events that can be raised after a control flow with "PostEvent" and on exception during the control flow with "RollbackEvent".
- [AsyncCommandCorrelationDecorator{TCommand}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Correlation/AsyncCommandCorrelationDecorator.cs) decorates the target command handler with an implementation of "IAsyncCorrelationContext" that  adds an event (post event) to be raised after the main one in the same control flow only if there is no exception, and an event (roll back event) to be raised when exception.
- [AsyncQueryCorrelationDecorator{TQuery, TResult}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Correlation/AsyncQueryCorrelationDecorator.cs) decorates the target query handler with an implementation of "IAsyncCorrelationContext" that  adds an event (post event) to be raised after the main one in the same control flow only if there is no exception, and an event (roll back event) to be raised when exception.
- [CorrelationCollection{TKey, TValue}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Correlation/CorrelationCollection.cs) Provides a collection of objects that need to be shared across asynchronous control flows. This collection implements "IAsyncEnumerable{KeyValuePair{TKey, TValue}}".
- [IDispatcher](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/CQRS/IDispatcher.cs) Defines a set of methods to automatically handle  "IAsyncCommand" and "IAsyncQuery{TResult}  using the matching implementation of "IAsyncQueryHandler{TQuery, TResult}" or/and "IAsyncCommandHandler{TCommand}".
- [IDispatcherHandlerProvider](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/CQRS/IDispatcherHandlerProvider.cs) Defines set of methods to retrieve handlers of specific type.
- [IDataContext](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/EntityFramework/IDataContext.cs) Allows an application author to manage domain objects using EntityFrameworkCore.
- [IQueryExpression{T,R}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Expressions/IQueryExpression.cs) Defines a methods that returns an "Expression{TDelegate}" that can be used to query the "TSource" instance.
- [IHttpRestClientHandler](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Http/IHttpRestClientHandler.cs)  Provides with methods to handle HTTP Rest client queries and commands using a typed client HTTP Client.
- [IInterceptor](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Interception/IInterceptor.cs) Base interface for types and instances for interception.
- [IAsyncQuery{R}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/CQRS/IAsyncQuery.cs) is used as marker for queries when using the command pattern.
- [IAsyncQueryHandler{T,R}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/CQRS/IAsyncQueryHandler.cs) defines a generic method that a class implements to handle a type-specific query and returns a type-specific result.
- [IValidation{T}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Validations/IValidation.cs) defines a method contract used to validate an argument.
- [IVisitor{T}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Visitors/IVisitor.cs) allows you to add new behaviors to an existing object without changing the object structure.
- [IVisitable{T}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Visitors/IVisitable.cs)  Defines an Accept operation that takes a visitor as an argument.

# Some uses

## [IHttpRestClientHandler](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Http/IHttpRestClientHandler.cs)
Provides with methods to handle HTTP Rest client queries and commands using a typed client HTTP Client. The queries and commands should implement one of the following interfaces : "IStringRequest", "IStreamRequest", "IByteArrayRequest", "IFormUrlEncodedRequest","IMultipartRequest", "IQueryStringRequest"... and decorated with the [HtppRestClientAttribute](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Http/HttpRestClientAttribute.cs) or implement the [IHttpRestClientAttributeProvider](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/Http/IHttpRestClientAttributeProvider.cs).

```C#
// The api signature
[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    public LoginController(IDispatcher dispatcher) => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    [HttpPost, AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    public async Task<IActionResult> PostAsync([FromBody] LoginRequest login, CancellationToken cancellationToken = default)
        => Ok(await _dispatcher.InvokeAsync(login, cancellationToken).ConfigureAwait(false));
}

// startup class ...
public class Startup
{
    ....
    services.AddScoped<ITokenService, TokenService>();
    services.AddDbContext<UserContext>(...);
    services.AddXDataContext(provider => provider.GetRequiredService<UserContext>());
    
    var assemblies = new[] { typeof(LoginRequestHander).Assembly };

    services.AddXDispatcher();
    services.AddXCommandQueriesHandlers(assemblies, options =>
    {
        options.UseValidatorDecorator(); // Queries and commands validation decorator
    });    
    
    ...
}

[HttpRestClient(Path = "api/login", Method = "Post", IsSecured = false)]
public record LoginRequest([Required] string Name, [Required] string Password) : QueryExpression<User>, IAsyncQuery<LoginResponse>, IValidationDecorator
{
    public override Expression<Func<User, bool>> GetExpression() => user => user.Name == Name;
}

public record LoginResponse(string Token);

// The request validator will be called before the LoginRequestHandler
public sealed class LoginRequestValidator : IValidation<LoginRequest>
{
    public Task ValidateAsync(LoginRequest argument)
    {
        // code validation
        ...
    }
}

// The dbContext

public sealed class UserContext : DataContext // DataContext implements the IDataContext interface
{
    ...
}

// The LoginRequest handler...
public sealed class LoginRequestHandler : IAsyncQueryHandler<LoginRequest, LoginResponse>
{
    private readonly IDataContext<User> _dataContext;
    private readonly ITokenService _tokenService;
    
    public LoginRequestHandler(IDataContext<User> dataContext, ITokenService tokenService) => (_dataContext, _tokenService) = (dataContext, tokenService);
    
    public async Task<LoginResponse> HandleAsync(LoginRequest query, CancellationToken cancellationToken = default)
    {
        var user = await _dataContext.FindAsync(u=>u.Where(query), cancellationToken).configureAwait(false)
            ?? throw ... or something else
        
        if(!user.Password.IsEqualTo(query.Password))
            throw ... or something else
        
        var token = _tokenService.CreateToken(....);
        return new LoginResponse(token);
    }
}

// The Api Test class ...
public async Task LoginTestMethodAsync()
{
    System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    var factory = new WebApplicationFactory<Api.Program>();
    var client = factory.CreateClient();
    IHttpRestClientHandler httpClient = new HttpRestClientHandler(client);
    
    var request = new LoginRequest("mylogin", "mypassword");
    var response = await httpClient.HandleAsync(request).ConfigureAwait(false);

    Assert.IsNotNull(response.Result.Token);
}

...

```

## [NotifyPropertyChanged{T}](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/NotifyPropertyChanged.cs)
Implementation for "INotifyPropertyChanged".

```C#
public class User : NotifyPropertyChanged<User>
{
    private string _firstName;
    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
    
    private string _lastName;
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    
    [NotifyPropertyChangedFor(nameof(FirstName)]
    [NotifyPropertyChangedFor(nameof(LastName)]
    public string FullName => $"{FirstName} {LastName}";
    ...
}

// 'FullName' : changes on 'FirstName' and 'LastName' are notified to 'FullName'.
```

## [EnumerationType](https://github.com/Francescolis/Xpandables.Net/blob/Net5.0/Xpandables.Net/EnumerationType.cs)
A helper class to implement custom enumeration.

```C#
public abstract class EnumerationType : IEqualityComparer<EnumerationType>, IEquatable<EnumerationType>,
    IComparable<EnumerationType>
{
    protected EnumerationType(string displayName, int value)
    {
        Value = value;
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
    }
    ....
    
    with useful methods :
    
    public static IEnumerable<TEnumeration> GetAll<TEnumeration>()...
    
    public static TEnumeration FromDisplayName<TEnumeration>(string displayName)
        where TEnumeration : Enumeration ...        
}

[TypeConverter(typeof(EnumerationTypeConverter))]
public sealed class Gender : EnumerationType
{
    private Gender(string displayName, int value) : base(displayName, value) { }
    public static Gender Woman => new Gender(nameof(Woman), 0);
    public static Gender Man => new Gender(nameof(Man), 1);
    
    public bool IsWoman() => this == Woman;
    public bool IsMan() => this == Man;
}

// You can use the EnumerationTypeConverter to convert "Enumeration" objects to and from string representations.

```
