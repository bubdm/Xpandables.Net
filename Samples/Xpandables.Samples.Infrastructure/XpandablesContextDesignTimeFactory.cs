
using Xpandables.Net5.EntityFramework;

namespace Xpandables.Samples.Infrastructure
{
    public sealed class XpandablesContextDesignTimeFactory : DataContextDesignTimeFactory<XpandablesContext, DataContextSettings, XpandablesContextProvider>
    {
    }
}
