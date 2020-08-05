using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;
using Xpandables.Net.EntityFramework;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Infrastructure
{
    public sealed class XpandablesContextInitializer : IDataContextSeeder<XpandablesContext>
    {
        public async Task SeedAsync(XpandablesContext dataContext, CancellationToken cancellationToken = default)
        {
            _ = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

            if (!dataContext.Users.Any())
            {
                dataContext.Users.AddRange(new List<User>
                {
                    UserFactory.Create("email@email.com", "first", "last", "motdepasse", Gender.Woman, Picture.Default()),
                    UserFactory.Create("mail@mail.com", "first1", "last1","passsword", Gender.Man, Picture.Default())
                });

                await dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
