using Microsoft.Extensions.DependencyInjection;

using System;

using Xpandables.Net.Database;

namespace Xpandables.Net.Api.Database
{
    public class ContactContextFactory : IDataContextFactory
    {
        private readonly IServiceProvider _serviceScopeFactory;
        private readonly int number;
        public ContactContextFactory(IServiceProvider serviceScopeFactory)
        {
            number = new Random().Next(1, 10);
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public IDataContext GetDataContext()
        {
            return number switch
            {
                >= 1 and <= 5 => _serviceScopeFactory.GetRequiredService<ContactContext>(),
                _ => _serviceScopeFactory.GetRequiredService<ContactContextSecond>()
            };
        }
    }
}
