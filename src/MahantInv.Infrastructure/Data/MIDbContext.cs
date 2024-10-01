using Ardalis.EFCore.Extensions;
using MahantInv.Infrastructure.Entities;
using MahantInv.Infrastructure.Identity;
using MahantInv.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MahantInv.Infrastructure.Data
{
    public class MIDbContext : IdentityDbContext<MIIdentityUser>
    {
        private readonly IMediator _mediator;

        public MIDbContext(DbContextOptions<MIDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public virtual DbSet<Buyer> Buyers { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDocument> OrderDocuments { get; set; }

        public virtual DbSet<OrderStatusType> OrderStatusTypes { get; set; }

        public virtual DbSet<OrderTransaction> OrderTransactions { get; set; }

        public virtual DbSet<Party> Parties { get; set; }

        public virtual DbSet<PartyCategory> PartyCategories { get; set; }

        public virtual DbSet<PaymentType> PaymentTypes { get; set; }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductExpiry> ProductExpiries { get; set; }

        public virtual DbSet<ProductInventory> ProductInventories { get; set; }

        public virtual DbSet<ProductInventoryHistory> ProductInventoryHistories { get; set; }

        public virtual DbSet<ProductStorage> ProductStorages { get; set; }

        public virtual DbSet<ProductUsage> ProductUsages { get; set; }

        public virtual DbSet<Storage> Storages { get; set; }

        public virtual DbSet<UnitType> UnitTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();

            // alternately this is built-in to EF Core 2.2
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Order>(entity =>
            {


                entity.HasOne(d => d.LastModifiedBy).WithMany(p => p.Orders);

                entity.HasOne(d => d.Product).WithMany(p => p.Orders);

                entity.HasOne(d => d.Status).WithMany(p => p.Orders);
            });

            modelBuilder.Entity<OrderTransaction>(entity =>
            {
                entity.HasOne(d => d.Order).WithMany(p => p.OrderTransactions);

                entity.HasOne(d => d.Party).WithMany(p => p.OrderTransactions);

                entity.HasOne(d => d.PaymentType).WithMany(p => p.OrderTransactions);
            });

            modelBuilder.Entity<Party>(entity =>
            {


                entity.HasOne(d => d.Category).WithMany(p => p.Parties);

                entity.HasOne(d => d.LastModifiedBy).WithMany(p => p.Parties);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(d => d.LastModifiedBy).WithMany(p => p.Products);
            });

            modelBuilder.Entity<ProductInventory>(entity =>
            {

                entity.HasIndex(d => d.ProductId).IsUnique();
                entity.HasOne(d => d.LastModifiedBy).WithMany(p => p.ProductInventories);

                entity.HasOne(d => d.Product).WithMany(p => p.ProductInventories);
            });

            modelBuilder.Entity<ProductInventoryHistory>(entity =>
            {


                entity.HasOne(d => d.LastModifiedBy).WithMany(p => p.ProductInventoryHistories);

                entity.HasOne(d => d.Product).WithMany(p => p.ProductInventoryHistories);
            });

            modelBuilder.Entity<ProductStorage>(entity =>
            {
                entity.HasOne(d => d.Product).WithMany(p => p.ProductStorages);

                entity.HasOne(d => d.Storage).WithMany();
            });

            modelBuilder.Entity<ProductUsage>(entity =>
            {
                entity.HasOne(d => d.LastModifiedBy).WithMany(p => p.ProductUsages);

                entity.HasOne(d => d.Product).WithMany(p => p.ProductUsages);
            });

            modelBuilder.Entity<Storage>(entity =>
            {

            });
            modelBuilder.Entity<OrderDocument>(entity =>
            {
                entity.HasOne(d => d.Order).WithMany(p => p.OrderDocuments);
            });

            modelBuilder.Entity<ProductExpiry>(entity =>
            {
                entity.HasOne(d => d.Product).WithMany(p => p.ProductExpiries);
                entity.HasOne(d => d.Order).WithMany(p => p.ProductExpiries);
            });

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // ignore events if no dispatcher provided
            if (_mediator == null) return result;

            // dispatch events only if save was successful
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.Events.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    await _mediator.Publish(domainEvent).ConfigureAwait(false);
                }
            }

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}