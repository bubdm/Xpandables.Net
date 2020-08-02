
using Xpandables.Net.Interception;
using Xpandables.Samples.Business.Contracts;

namespace Xpandables.Samples.Business.Interceptors
{
    public sealed class SignInRequestInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.Arguments[0].Value is SignInRequest signInRequest)
                signInRequest.Email = "email@email.com";

            invocation.Proceed();
        }
    }
}
