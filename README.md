# Xpandables.Net
Provides with useful interfaces contracts in **.Net Standard 2.1** and some implementations mostly following the spirit of SOLID principles, such as [IAsyncCommandHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Commands/IAsyncCommandHandler.cs), [IDispatcher](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Dispatchers/IDispatcher.cs), [IAsyncQueryHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Queries/IAsyncQueryHandler.cs), [IHttpRestClientHandler](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/HttpRestClient/IHttpRestClientHandler.cs) and so on.

Feel free to fork this project, make your own changes and create a pull request.

## Available types
- [Entity](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/Entity.cs) that contains basic properties for all entities.
- [ValueObject](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ValueObject.cs) An object that represents a descriptive aspect of the domain with no conceptual identity.
- [IAddable](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/IAddable.cs) This interface is useful when implementing a serializable custom collection with JSON.
- [ICanHandle](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ICanHandle.cs) Provides a method that determines whether or not an argument can be handled.
- [ValueRange](https://github.com/Francescolis/Xpandables.Net/blob/master/Xpandables.Net/ValueRange.cs) Defines a pair of values, representing a segment.

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
