using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Xpandables.Net.Creators;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Events;

namespace Xpandables.Samples.Infrastructure
{
    public sealed class XpandablesContextProvider : DataContextProvider<XpandablesContext>
    {
        public XpandablesContextProvider(IOptions<DataContextSettings> dataContextSettings, IInstanceCreator instanceCreator)
            : base(dataContextSettings, instanceCreator) { }

        public override DbContextOptions<XpandablesContext> GetDataContextOptions()
        {
            return new DbContextOptionsBuilder<XpandablesContext>()
                .UseSqlServer(DataContextSettings.ConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .Options;
        }
    }

    public sealed class XpandablesLogContextProvider : DataLogContextProvider<XpandablesLogContext, DefaultLogEntity>
    {
        public XpandablesLogContextProvider(IOptions<DataLogContextSettings> dataLogContextSettings, IInstanceCreator instanceCreator)
            : base(dataLogContextSettings, instanceCreator) { }

        public override DbContextOptions<XpandablesLogContext> GetDataLogContextOptions()
        {
            return new DbContextOptionsBuilder<XpandablesLogContext>()
                .UseSqlServer(DataLogContextSettings.ConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .Options;
        }
    }
}
