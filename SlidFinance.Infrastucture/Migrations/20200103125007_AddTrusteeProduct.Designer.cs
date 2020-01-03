﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SlidFinance.Infrastructure;

namespace SlidFinance.WebApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200103125007_AddTrusteeProduct")]
    partial class AddTrusteeProduct
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SlidFinance.Domain.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<int>("TrusteeId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("TrusteeId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("SlidFinance.Domain.AuthToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DeviceId")
                        .IsRequired();

                    b.Property<DateTime>("ExpirationDate");

                    b.Property<string>("Token")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AuthTokens");
                });

            modelBuilder.Entity("SlidFinance.Domain.Bank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Banks");
                });

            modelBuilder.Entity("SlidFinance.Domain.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("Balance");

                    b.Property<int?>("BankId");

                    b.Property<string>("Code");

                    b.Property<float>("CreditLimit");

                    b.Property<int?>("ProductId");

                    b.Property<int?>("SelectedTariffId");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SelectedTariffId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("SlidFinance.Domain.Mcc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("Category");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.Property<string>("Description");

                    b.Property<bool>("IsSystem");

                    b.Property<string>("RuDescription");

                    b.Property<string>("RuTitle");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Mcc");
                });

            modelBuilder.Entity("SlidFinance.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Approved");

                    b.Property<int?>("BankId");

                    b.Property<string>("Image");

                    b.Property<bool>("IsPublic");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("SlidFinance.Domain.ProductTariff", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ProductId");

                    b.Property<string>("Title");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductTariff");
                });

            modelBuilder.Entity("SlidFinance.Domain.Rule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<string>("BankCategory");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Description");

                    b.Property<int?>("MccId");

                    b.Property<int>("Order");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MccId");

                    b.ToTable("Rules");
                });

            modelBuilder.Entity("SlidFinance.Domain.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountId");

                    b.Property<float>("Amount");

                    b.Property<bool>("Approved");

                    b.Property<string>("BankCategory")
                        .IsRequired();

                    b.Property<int?>("CategoryId");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<int?>("MccId");

                    b.Property<string>("UserDescription")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("MccId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("SlidFinance.Domain.Trustee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("Trustee");
                });

            modelBuilder.Entity("SlidFinance.Domain.TrusteeAccount", b =>
                {
                    b.Property<int>("AccountId");

                    b.Property<int>("TrusteeId");

                    b.HasKey("AccountId", "TrusteeId");

                    b.HasIndex("TrusteeId");

                    b.ToTable("TrusteeAccounts");
                });

            modelBuilder.Entity("SlidFinance.Domain.TrusteeCategory", b =>
                {
                    b.Property<int>("CategoryId");

                    b.Property<int>("TrusteeId");

                    b.HasKey("CategoryId", "TrusteeId");

                    b.HasIndex("TrusteeId");

                    b.ToTable("TrusteeCategories");
                });

            modelBuilder.Entity("SlidFinance.Domain.TrusteeProduct", b =>
                {
                    b.Property<int>("ProductId");

                    b.Property<int>("TrusteeId");

                    b.HasKey("ProductId", "TrusteeId");

                    b.HasIndex("TrusteeId");

                    b.ToTable("TrusteeProducts");
                });

            modelBuilder.Entity("SlidFinance.Domain.UserCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Order");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SlidFinance.Models.Merchant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedById")
                        .IsRequired();

                    b.Property<string>("DisplayName");

                    b.Property<bool>("IsPublic");

                    b.Property<int>("MccId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("Updated");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("MccId");

                    b.ToTable("Merchants");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SlidFinance.Domain.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SlidFinance.Domain.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlidFinance.Domain.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SlidFinance.Domain.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlidFinance.Domain.ApplicationUser", b =>
                {
                    b.HasOne("SlidFinance.Domain.Trustee", "Trustee")
                        .WithMany()
                        .HasForeignKey("TrusteeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlidFinance.Domain.AuthToken", b =>
                {
                    b.HasOne("SlidFinance.Domain.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlidFinance.Domain.BankAccount", b =>
                {
                    b.HasOne("SlidFinance.Domain.Bank", "Bank")
                        .WithMany()
                        .HasForeignKey("BankId");

                    b.HasOne("SlidFinance.Domain.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("SlidFinance.Domain.ProductTariff", "SelectedTariff")
                        .WithMany()
                        .HasForeignKey("SelectedTariffId");
                });

            modelBuilder.Entity("SlidFinance.Domain.Product", b =>
                {
                    b.HasOne("SlidFinance.Domain.Bank", "Bank")
                        .WithMany()
                        .HasForeignKey("BankId");
                });

            modelBuilder.Entity("SlidFinance.Domain.ProductTariff", b =>
                {
                    b.HasOne("SlidFinance.Domain.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlidFinance.Domain.Rule", b =>
                {
                    b.HasOne("SlidFinance.Domain.BankAccount", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("SlidFinance.Domain.UserCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlidFinance.Domain.Mcc", "Mcc")
                        .WithMany()
                        .HasForeignKey("MccId");
                });

            modelBuilder.Entity("SlidFinance.Domain.Transaction", b =>
                {
                    b.HasOne("SlidFinance.Domain.BankAccount", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlidFinance.Domain.UserCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("SlidFinance.Domain.Mcc", "Mcc")
                        .WithMany()
                        .HasForeignKey("MccId");
                });

            modelBuilder.Entity("SlidFinance.Domain.TrusteeAccount", b =>
                {
                    b.HasOne("SlidFinance.Domain.BankAccount", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlidFinance.Domain.Trustee", "Trustee")
                        .WithMany()
                        .HasForeignKey("TrusteeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlidFinance.Domain.TrusteeCategory", b =>
                {
                    b.HasOne("SlidFinance.Domain.UserCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlidFinance.Domain.Trustee", "Trustee")
                        .WithMany()
                        .HasForeignKey("TrusteeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlidFinance.Domain.TrusteeProduct", b =>
                {
                    b.HasOne("SlidFinance.Domain.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlidFinance.Domain.Trustee", "Trustee")
                        .WithMany()
                        .HasForeignKey("TrusteeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SlidFinance.Models.Merchant", b =>
                {
                    b.HasOne("SlidFinance.Domain.ApplicationUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SlidFinance.Domain.Mcc", "Mcc")
                        .WithMany()
                        .HasForeignKey("MccId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
