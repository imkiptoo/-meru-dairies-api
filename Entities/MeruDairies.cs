using System;
using API.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace API.Entities
{
    public partial class MeruDairies : DbContext
    {
        public MeruDairies()
        {
        }

        public MeruDairies(DbContextOptions<MeruDairies> options)
            : base(options)
        {
        }

        public virtual DbSet<Factory> Factories { get; set; }
        public virtual DbSet<MainCompanyTable> MainCompanyTables { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Tools.syncConnection.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Factory>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("factories_pk")
                    .IsClustered(false);

                entity.ToTable("factories");

                entity.HasIndex(e => e.Company, "factories_company_index");

                entity.HasIndex(e => e.Id, "factories_id_index");

                entity.HasIndex(e => e.Id, "factories_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "factories_name_index");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Company)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("company");

                entity.Property(e => e.Hash)
                    .IsUnicode(false)
                    .HasColumnName("hash");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<MainCompanyTable>(entity =>
            {
                entity.HasKey(e => e.TableName)
                    .HasName("tables_pk")
                    .IsClustered(false);

                entity.ToTable("main_company_tables");

                entity.HasIndex(e => e.TableCompany, "tables_table_company_index");

                entity.HasIndex(e => e.TableName, "tables_table_name_index");

                entity.Property(e => e.TableName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("table_name");

                entity.Property(e => e.Hash)
                    .IsUnicode(false)
                    .HasColumnName("hash");

                entity.Property(e => e.TableCompany)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("table_company");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("services");

                entity.Property(e => e.Hash)
                    .IsUnicode(false)
                    .HasColumnName("hash");

                entity.Property(e => e.MainCompanyName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("main_company_name");

                entity.Property(e => e.MainCompanySyncEnabled)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("main_company_sync_enabled");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("users_pk")
                    .IsClustered(false);

                entity.ToTable("users");

                entity.HasIndex(e => e.DateCreated, "client_parameters_date_created_index");

                entity.HasIndex(e => e.DateUpdated, "client_parameters_date_updated_index");

                entity.HasIndex(e => e.FactoryId, "client_parameters_factory_id_index");

                entity.HasIndex(e => e.Passphrase, "client_parameters_passphrase_index");

                entity.HasIndex(e => e.PassphraseType, "client_parameters_passphrase_type_index");

                entity.HasIndex(e => e.UserStatus, "client_parameters_user_status_index");

                entity.HasIndex(e => e.Username, "client_parameters_username_index");

                entity.HasIndex(e => e.Username, "client_parameters_username_uindex")
                    .IsUnique();

                entity.Property(e => e.Username)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_created");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_updated");

                entity.Property(e => e.FactoryId).HasColumnName("factory_id");

                entity.Property(e => e.Hash)
                    .IsUnicode(false)
                    .HasColumnName("hash");

                entity.Property(e => e.Passphrase)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("passphrase");

                entity.Property(e => e.PassphraseType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("passphrase_type");

                entity.Property(e => e.UserStatus)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("user_status");

                entity.Property(e => e.UserType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("user_type");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
