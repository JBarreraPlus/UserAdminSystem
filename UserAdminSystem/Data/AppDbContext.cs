using Microsoft.EntityFrameworkCore;
using UserAdminSystem.Models;

namespace UserAdminSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserApp> Users { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<RoleModule> RoleModules { get; set; }
    public DbSet<ModulePermission> ModulePermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ? Relation One To Many  between User and person
        builder.Entity<UserApp>()
            .HasMany(u => u.Persons)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        // ? Relation One to One between Persons and Address
        builder.Entity<Person>()
            .HasOne(p => p.Address)
            .WithOne(a => a.Person)
            .HasForeignKey<Address>(a => a.PersonId);

        // ? Relation One to Many between Roles and Users
        builder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId);

        // ? Relation Many to Many between Roles and Modules
        builder.Entity<RoleModule>()
            .HasKey(rm => new { rm.RoleId, rm.ModuleId });

        builder.Entity<RoleModule>()
            .HasOne(rm => rm.Role)
            .WithMany(r => r.RoleModules)
            .HasForeignKey(rm => rm.RoleId);

        builder.Entity<RoleModule>()
            .HasOne(rm => rm.Module)
            .WithMany(m => m.RoleModules)
            .HasForeignKey(rm => rm.ModuleId);

        // ? Relation Many to Many between Modules and Permissions
        builder.Entity<ModulePermission>()
            .HasKey(mp => new { mp.ModuleId, mp.PermissionId });

        builder.Entity<ModulePermission>()
            .HasOne(mp => mp.Module)
            .WithMany(m => m.ModulePermissions)
            .HasForeignKey(mp => mp.ModuleId);

        builder.Entity<ModulePermission>()
            .HasOne(mp => mp.Permission)
            .WithMany(m => m.ModulePermissions)
            .HasForeignKey(mp => mp.PermissionId);

        // ? Unique Values
        builder.Entity<UserApp>().HasIndex(u => u.UserName).IsUnique();
        builder.Entity<Permission>().HasIndex(p => p.Name).IsUnique();
        builder.Entity<Module>().HasIndex(p => p.Name).IsUnique();
        builder.Entity<Role>().HasIndex(p => p.Name).IsUnique();

        // ? Global filters
        builder.Entity<UserApp>().HasQueryFilter(u => u.IsActive);
        builder.Entity<Person>().HasQueryFilter(p => p.IsActive);
        builder.Entity<Address>().HasQueryFilter(a => a.IsActive);
        builder.Entity<Role>().HasQueryFilter(r => r.IsActive);
        builder.Entity<RoleModule>().HasQueryFilter(rm => rm.IsActive);
        builder.Entity<Module>().HasQueryFilter(m => m.IsActive);
        builder.Entity<ModulePermission>().HasQueryFilter(mp => mp.IsActive);
        builder.Entity<Permission>().HasQueryFilter(p => p.IsActive);
    }
}