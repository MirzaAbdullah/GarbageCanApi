using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class databaseIEContext : DbContext
    {
        private readonly IConfiguration configuration;
        public databaseIEContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public databaseIEContext(DbContextOptions<databaseIEContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Assign> Assigns { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(configuration["ConnectionStrings:GarbageCanDBConnectionString"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.UTF-8");

            modelBuilder.Entity<Assign>(entity =>
            {
                entity.HasKey(e => e.IdAssign)
                    .HasName("assign_pkey");

                entity.ToTable("assign");

                entity.Property(e => e.IdAssign).HasColumnName("id_assign");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("created_date");

                entity.Property(e => e.IdRequest).HasColumnName("id_request");

                entity.Property(e => e.IdUser)
                    .HasMaxLength(30)
                    .HasColumnName("id_user");

                entity.HasOne(d => d.IdRequestNavigation)
                    .WithMany(p => p.Assigns)
                    .HasForeignKey(d => d.IdRequest)
                    .HasConstraintName("fk_requests");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Assigns)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_users");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("contacts");

                entity.Property(e => e.ContactId)
                    .HasColumnName("contact_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.ContactName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("contact_name");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasColumnName("phone");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("fk_customer");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("customer_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("customer_name");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasKey(e => e.IdRequest)
                    .HasName("requests_pkey");

                entity.ToTable("requests");

                entity.Property(e => e.IdRequest).HasColumnName("id_request");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("created_date");

                entity.Property(e => e.IdUser)
                    .HasMaxLength(30)
                    .HasColumnName("id_user");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasColumnName("is_active");

                entity.Property(e => e.Latitudes)
                    .HasMaxLength(30)
                    .HasColumnName("latitudes");

                entity.Property(e => e.Longitudes)
                    .HasMaxLength(30)
                    .HasColumnName("longitudes");

                entity.Property(e => e.PickupCost)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("pickup_cost");

                entity.Property(e => e.PickupDate)
                    .HasColumnType("date")
                    .HasColumnName("pickup_date");

                entity.Property(e => e.PickupItem)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("pickup_item");

                entity.Property(e => e.PickupStatus)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("pickup_status");

                entity.Property(e => e.PickupTime)
                    .HasColumnType("time without time zone")
                    .HasColumnName("pickup_time");

                entity.Property(e => e.PickupWeight)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("pickup_weight");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_users");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.IdRole)
                    .HasName("roles_pkey");

                entity.ToTable("roles");

                entity.Property(e => e.IdRole).HasColumnName("id_role");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("users_pkey");

                entity.ToTable("users");

                entity.Property(e => e.IdUser)
                    .HasMaxLength(30)
                    .HasColumnName("id_user");

                entity.Property(e => e.CodeVerification).HasColumnName("code_verification");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("created_date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("first_name");

                entity.Property(e => e.IdRole).HasColumnName("id_role");

                entity.Property(e => e.IsVerified).HasColumnName("is_verified");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("last_name");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(30)
                    .HasColumnName("phone_no");

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdRole)
                    .HasConstraintName("fk_roles");
            });

            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.HasKey(e => e.IdUserDetail)
                    .HasName("user_details_pkey");

                entity.ToTable("user_details");

                entity.Property(e => e.IdUserDetail).HasColumnName("id_user_detail");

                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("address_1");

                entity.Property(e => e.Address2)
                    .HasMaxLength(50)
                    .HasColumnName("address_2");

                entity.Property(e => e.City)
                    .HasMaxLength(30)
                    .HasColumnName("city");

                entity.Property(e => e.Country)
                    .HasMaxLength(30)
                    .HasColumnName("country");

                entity.Property(e => e.IdUser)
                    .HasMaxLength(30)
                    .HasColumnName("id_user");

                entity.Property(e => e.Province)
                    .HasMaxLength(30)
                    .HasColumnName("province");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.UserDetails)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("fk_users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
