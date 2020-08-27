
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Events;

namespace Xpandables.Samples.Infrastructure
{
    public sealed class XpandablesContextDesignTimeFactory : DataContextDesignTimeFactory<XpandablesContext, XpandablesContextProvider>
    {
    }

    public sealed class XpandablesLogContextDesignTimeFactory : DataLogContextDesignTimeFactory<XpandablesLogContext, DefaultLogEntity, XpandablesLogContextProvider>
    { }
}
