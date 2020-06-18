using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// Validator when no explicit registration exist for a given type.
    /// </summary>
    /// <typeparam name="TArgument">Type of argument to be validated.</typeparam>
    public sealed class NullValidatorRule<TArgument> : ValidatorRule<TArgument>
        where TArgument : class
    {
        /// <summary>
        /// Default implementation.
        /// </summary>
        /// <param name="_">The argument to be validated.</param>
        public async Task ValidateAsync(TArgument _) => await Task.CompletedTask.ConfigureAwait(false);
    }
}