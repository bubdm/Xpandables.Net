using Microsoft.EntityFrameworkCore;

using Xpandables.Net5.Entities;
using Xpandables.Net5.EntityFramework;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Infrastructure
{
    public sealed class XpandablesContext : DataContext, IBehaviorSeed
    {
        public XpandablesContext(DbContextOptions contextOptions) : base(contextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(new string[] { nameof(User.Id) });
            modelBuilder.Entity<User>().HasIndex(new string[] { nameof(User.Id) }).IsUnique();
            modelBuilder.Entity<User>()
                .OwnsOne(r => r.Password, pwd =>
                {
                    pwd.Property(p => p.Key);
                    pwd.Property(p => p.Value);
                    pwd.Property(p => p.Salt);
                })
                .OwnsOne(u => u.Name, n =>
                  {
                      n.Property(p => p.FirstName);
                      n.Property(p => p.LastName);
                  })
                .OwnsOne(r => r.Picture, pic =>
                {
                    pic.Property(p => p.Content);
                    pic.Property(p => p.Extension);
                    pic.Property(p => p.Height);
                    pic.Property(p => p.Title);
                    pic.Property(p => p.Width);
                });

            modelBuilder.UseEnumerationValueConverterForType(EnumerationConverter<Gender>());
        }

        public DbSet<User> Users { get; set; }
    }
}
