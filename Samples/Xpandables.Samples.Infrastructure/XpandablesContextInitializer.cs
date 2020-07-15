using System.Collections.Generic;
using System.Linq;

using Xpandables.Net5.Entities;
using Xpandables.Net5.EntityFramework;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Infrastructure
{
    public sealed class XpandablesContextInitializer : IDataContextSeeder<XpandablesContext>
    {
        public XpandablesContext Seed(XpandablesContext dataContext)
        {
            if (!dataContext.Users.Any())
            {
                dataContext.Users.AddRange(new List<User>
                {
                    UserFactory.Create("email@email.com", "first", "last", "motdepasse", Gender.Woman, Picture.Default()),
                    UserFactory.Create("mail@mail.com", "first1", "last1","passsword", Gender.Man, Picture.Default())
                });

                dataContext.Persist();
            }

            return dataContext;
        }
    }
}
