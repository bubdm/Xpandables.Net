
using System;
using System.Threading.Tasks;

namespace Xpandables.Net5.ValidatorRules
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed record ValidatorRuleBuilder<TArgument>(Func<TArgument, Task> validator)
        : IValidatorRule<TArgument>
        where TArgument : class
    {

        public async Task ValidateAsync(TArgument argument)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            await validator(argument).ConfigureAwait(false);
        }
    }
}
