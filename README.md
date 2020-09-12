# Xpandables.Net
Provides with useful interfaces contracts in **.Net Standard 2.1** and some implementations mostly following the spirit of SOLID principles, such as [IAsyncCommandHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Commands/IAsyncCommandHandler.cs), [IDispatcher](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Dispatchers/IDispatcher.cs), [IAsyncQueryHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Queries/IAsyncQueryHandler.cs), [IHttpRestClientHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/HttpRestClient/IHttpRestClientHandler.cs) and so on.

Feel free to fork this project, make your own changes and create a pull request.

## Available types
- [Entity](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Entity.cs) that contains basic properties for all entities.
- [ValueObject](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ValueObject.cs) An object that represents a descriptive aspect of the domain with no conceptual identity.
- [IAddable{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/IAddable.cs) is a useful interface when implementing a serializable custom collection with JSON.
- [ICanHandle{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ICanHandle.cs) Provides a method that determines whether or not an argument can be handled.
- [ValueRange{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ValueRange.cs) Defines a pair of values, representing a segment.
- [IInstanceCreator](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Creators/IInstanceCreator.cs) Provides with methods to create instance of specific type at runtime using delegate and cache.
- [IStringCryptography](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Cryptography/IStringCryptography.cs) Provides with methods to encrypt and decrypt string values.
- [IStringGenerator](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Cryptography/IStringGenerator.cs) Provides with methods to generate strings.
- [ValueEncrypted](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Cryptography/ValueEncrypted.cs) Defines a representation of an encrypted value, its key and its salt used with [IStringCryptography](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Cryptography/IStringCryptography.cs).
- [IAsyncCommand](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Commands/IAsyncCommand.cs) and [ICommand](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Commands/ICommand.cs) are used as marker for commands when using the command pattern.
- [IAsyncCommandHandler{TCommand}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Commands/IAsyncCommandHandler.cs) and [ICommandHandler{TCommand}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Commands/ICommandHandler.cs) Define handler for a specific type command.
- [IAsyncCorrelationContext](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Correlation/IAsyncCorrelationContext.cs) and [ICorrelationContext](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Correlation/ICorrelationContext.cs) Define two events that can be raised after a control flow with "PostEvent" and on exception during the control flow with "RollbackEvent".
- [AsyncCommandCorrelationDecorator{TCommand}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Correlation/AsyncCommandCorrelationDecorator.cs) and [CommandCorrelationDecorator{TCommand}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Correlation/CommandCorrelationDecorator.cs) decorate the target command handler with an implementation of "IAsyncCorrelationContext" that  adds an event (post event) to be raised after the main one in the same control flow only if there is no exception, and an event (roll back event) to be raised when exception.
- [AsyncQueryCorrelationDecorator{TQuery, TResult}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Correlation/AsyncQueryCorrelationDecorator.cs) and [QueryCorrelationDecorator{TQuery, TResult}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Correlation/QueryCorrelationDecorator.cs) decorate the target query handler with an implementation of "IAsyncCorrelationContext" that  adds an event (post event) to be raised after the main one in the same control flow only if there is no exception, and an event (roll back event) to be raised when exception.
- [CorrelationCollection{TKey, TValue}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Correlation/CorrelationCollection.cs) Provides a collection of objects that need to be shared across asynchronous control flows. This collection implements "IAsyncEnumerable{KeyValuePair{TKey, TValue}}".
- [IDataBase](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Data/IDataBase.cs) Provides with methods to execute command against a database using a derived form of [DataExecutable{TResult}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Data/Executables/DataExecutable.cs) or [DataExecutableMapper{TResult}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Data/Executables/DataExecutableMapper.cs).
- [IDispatcher](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Dispatchers/IDispatcher.cs) Defines a set of methods to automatically handle "ICommand", "IAsyncCommand", "IAsyncQuery{TResult} and "IQuery{TResult}" using the matching implementation of "ICommandHandler{TCommand}", "IQueryHandler{TQuery, TResult}", "IAsyncQueryHandler{TQuery, TResult}" or/and "IAsyncCommandHandler{TCommand}".
- [IDispatcherHandlerProvider](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Dispatchers/IDispatcherHandlerProvider.cs) Defines set of methods to retrieve handlers of specific type.
- [IDataContext](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/EntityFramework/IDataContext.cs) Allows an application author to manage domain objects using EntityFrameworkCore.
- [IQueryExpression{T,R}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Expressions/IQueryExpression.cs) Defines a methods that returns an "Expression{TDelegate}" that can be used to query the "TSource" instance.
- [IHttpRestClientHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/HttpRestClient/IHttpRestClientHandler.cs)  Provides with methods to handle HTTP Rest client queries and commands using a typed client HTTP Client.
- [IInterceptor](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Interception/IInterceptor.cs) Base interface for types and instances for interception.
- [Optional{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Optionals/Optional.cs) Describes an object that can contain a value or not of a specific type.
- [IAsyncQuery{R}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Queries/IAsyncQuery.cs) and [IQuery{R}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Queries/IQuery.cs) are used as marker for queries when using the command pattern.
- [IAsyncQueryHandler{T,R}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Queries/IAsyncQueryHandler.cs) and [IQueryHandler{T,R}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Queries/IQueryHandler.cs) Defines a generic method that a class implements to handle a type-specific query and returns a type-specific result.
- [IValidatorRule{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ValidatorRules/IValidatorRule.cs) Defines a method contract used to validate an argument.
- [IVisitorRule](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/VisitorRules/IVisitorRule.cs) allows you to add new behaviors to an existing object without changing the object structure.
- [IVisitable](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/VisitorRules/IVisitable.cs)  Defines an Accept operation that takes a visitor as an argument.

# Some uses

## [IHttpRestClientHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/HttpRestClient/IHttpRestClientHandler.cs)
Provides with methods to handle HTTP Rest client queries and commands using a typed client HTTP Client. The queries and commands should implement one of the following interfaces : "IStringRequest", "IStreamRequest", "IByteArrayRequest", "IFormUrlEncodedRequest","IMultipartRequest", "IQueryStringRequest"... and decorated with the [HtppRestClientAttribute](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/HttpRestClient/HttpRestClientAttribute.cs) or implement the [IHttpRestClientAttributeProvider](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/HttpRestClient/IHttpRestClientAttributeProvider.cs).

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
        => Ok((await _dispatcher.SendQueryAsync(login, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false)).FirstOrDefault());
}

// startup class ...
public class Startup
{
    ....
    services.AddScoped<ITokenService, TokenService>();
    services.AddXDataContext<ContextProvider>(); // ContextProvider implements IDataContextProvider
    
    var assemblies = new[] { typeof(LoginRequestHander).Assembly };

    services.AddXDispatcher();
    services.AddXCommandQueriesHandlers(assemblies, options =>
    {
        options.UseValidatorDecorator(); // Queries and commands validation decorator
    });    
    
    ...
}

[HttpRestClient(Path = "api/login", Method = "Post", IsSecured = false)]
public class LoginRequest : QueryExpression<User>, IAsyncQuery<LoginResponse>, IValidationDecorator
{
    public Login(string name, string password) => (Name, Password) = (name, password);
    
    protected override Expression<Func<User, bool>> BuildExpression() => user => user.Name == Name;
    
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;    
}

public sealed class LoginResponse
{
    public LoginResponse() { }
    public LoginResponse(string token) => Token = token;
    public string Token {get; set; }
}

// The request validator will be called before the LoginRequestHandler
public sealed class LoginRequestValidator : IValidatorRule<LoginRequest>
{
    public void Validate(LoginRequest argument)
    {
        // code validation
        ...
    }
}

// The LoginRequest handler...
public sealed class LoginRequestHandler : IAsyncQueryHandler<LoginRequest, LoginResponse>
{
    private readonly IDataContext _dataContext;
    private readonly ITokenService _tokenService;
    public LoginRequestHandler(IDataContext dataContext, ITokenService tokenService) => (_dataContext, _tokenService) = (dataContext, tokenService);
    
    public async IAsyncEnumerable<LoginResponse> HandleAsync(LoginRequest query, CancellationToken cancellationToken = default)
    {
        var user = await _dataContext.SetOf(query).FirstOrDefaultAsync(query, cancellationToken).configureAwait(false)
            ?? throw ...
        
        if(!user.Password.IsEqualTo(query.Password))
            throw ...
        
        var token = _tokenService.CreateToken(....);
        yield return new LoginResponse(token);
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

## [NotifyPropertyChanged{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Notifications/NotifyPropertyChanged.cs)
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

## [EnumerationType](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Enumerations/EnumerationType.cs)
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

## [Optional{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Optionals/Optional.cs)
There is a specific implementation of F# Options you can find in **Optional<T>** with asynchronous behavior.

Without option :

```C#
public User FindUser(string userName, string password)
{
    var foundUser = userRepo.FindUserByName(userName);
    if(foundUser != null)
    {
        var isValidPWD = foundUser.PasswordIsValid(password, passwordService);
        if(isValidPWD)
            return foundUser;
        else ....
    }
         ...
}
```

With option :

```C#
public User FindUser(string userName, string password)
{
    return userRepo.TryFindUserByName(userName)
         .Map(user => user.PasswordIsValid(password, passwordService)) 
         .WhenEmpty(()=> throw ...);
}

// This code 'Map(user => ...)' will be executed only if userRepo contains a value.
// PasswordIsValid returns the current user instance or throws an exception.
// WhenEmpty(()=> throw ...) will be executed only if userRepo is empty.
```
Or

```C#
public Optional<User> TryFindUser(string userName, string password)
   => userRepo.TryFindUserByName(userName)
         .MapOptional(user => user.PasswordIsValid(password, passwordService));
         
// PasswordIsValid returns an Optional<User> with the current user instance if Ok or empty.
```
