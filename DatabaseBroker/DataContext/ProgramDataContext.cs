using Entity.Enum;
using Entity.Models;
using Entity.Models.Auth;
using Entity.Models.StaticFiles;
using Entitys.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseBroker.DataContext;

public class ProgramDataContext : DbContext
{
    public ProgramDataContext(DbContextOptions<ProgramDataContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }


    protected void TrackActionsAt()
    {
        foreach (var entity in this.ChangeTracker.Entries()
                     .Where(x => x.State == EntityState.Added && x.Entity is AuditableModelBase<int>))
        {
            var model = (AuditableModelBase<int>)entity.Entity;
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = model.CreatedAt;
        }

        foreach (var entity in this.ChangeTracker.Entries()
                     .Where(x => x.State == EntityState.Modified && x.Entity is AuditableModelBase<int>))
        {
            var model = (AuditableModelBase<int>)entity.Entity;
            model.UpdatedAt = DateTime.Now;
        }
    }

    public override int SaveChanges()
    {
        TrackActionsAt();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        TrackActionsAt();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new CancellationToken())
    {
        TrackActionsAt();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        TrackActionsAt();
        return base.SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //Configuring all MultiLanguage fields over entities
        var multiLanguageFields = modelBuilder
            .Model
            .GetEntityTypes()
            .SelectMany(x => x.ClrType.GetProperties())
            .Where(x => x.PropertyType == typeof(MultiLanguageField));

        foreach (var multiLanguageField in multiLanguageFields)
            modelBuilder
                .Entity(multiLanguageField.DeclaringType!)
                .Property(multiLanguageField.PropertyType, multiLanguageField.Name)
                .HasColumnType("jsonb");

        modelBuilder
            .Entity<SignMethod>()
            .HasOne(x => x.User)
            .WithMany(x => x.SignMethods)
            .HasForeignKey(x => x.UserId);

        modelBuilder
            .Entity<SignMethod>()
            .HasDiscriminator(x => x.Type)
            .HasValue<DefaultSignMethod>(SignMethods.Normal);
    }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Structure> Structures { get; set; }
    public DbSet<StructurePermission> StructurePermissions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TokenModel> Tokens { get; set; }
    public DbSet<SignMethod> UserSignMethods { get; set; }
    public DbSet<StaticFile> StaticFiles { get; set; }
}