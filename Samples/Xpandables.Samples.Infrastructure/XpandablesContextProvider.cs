using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Xpandables.Net.Creators;
using Xpandables.Net.EntityFramework;

namespace Xpandables.Samples.Infrastructure
{
    public sealed class XpandablesContextProvider : DataContextProvider<XpandablesContext, DataContextSettings>
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
}
