# Xpandables.Net
Provides with useful interfaces contracts in **.Net Standard 2.1** and some implementations mostly following the spirit of SOLID principles, such as [IAsyncCommandHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Commands/IAsyncCommandHandler.cs), [IDispatcher](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Dispatchers/IDispatcher.cs), [IAsyncQueryHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Queries/IAsyncQueryHandler.cs), [IHttpRestClientHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/HttpRestClient/IHttpRestClientHandler.cs) and so on.

Feel free to fork this project, make your own changes and create a pull request.

## Available types
- [Entity](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Entity.cs) that contains basic properties for all entities.
- [ValueObject](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ValueObject.cs) An object that represents a descriptive aspect of the domain with no conceptual identity.
- [IAddable{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/IAddable.cs) This interface is useful when implementing a serializable custom collection with JSON.
- [ICanHandle{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ICanHandle.cs) Provides a method that determines whether or not an argument can be handled.
- [ValueRange{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ValueRange.cs) Defines a pair of values, representing a segment.
- [IInstanceCreator](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Creators/IInstanceCreator.cs) Provides with methods to create instance of specific type at runtime using delegate and cache.
- [IStringCryptography](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Cryptography/IStringCryptography.cs) Provides with methods to encrypt and decrypt string values.
- [IStringGenerator](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Cryptography/IStringGenerator.cs) Provides with methods to generate strings.
- [ValueEncrypted](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Cryptography/ValueEncrypted.cs) Defines a representation of an encrypted value, its key and its salt used with [IStringCryptography](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Cryptography/IStringCryptography.cs).


Use of [NotifyPropertyChanged{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Notifications/NotifyPropertyChanged.cs) Implementation for "INotifyPropertyChanged".

```C#
public class User : NotifyPropertyChanged<User>
{
    private string _firstName;
    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
    
    private string _lastName;
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    
    [NotifyPropertyOn(nameof(FirstName)]
    [NotifyPropertyOn(nameof(LastName)]
    public string FullName => $"{FirstName} {LastName}";
    ...
}

// 'FullName' : changes on 'FirstName' and 'LastName' are notified to 'FullName'.
```


Use of [EnumerationType](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Enumerations/EnumerationType.cs), a helper class to implement custom enumeration.

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

Use of [Optional{T}](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Optionals/Optional.cs)

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
