﻿// <auto-generated />
using System;
using FarmersPlatform.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FarmersPlatform.Migrations
{
    [DbContext(typeof(FarmContext))]
    partial class FarmContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FarmersPlatform.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("FarmersPlatform.Models.Farmer", b =>
                {
                    b.Property<int>("FarmerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FarmerId"));

                    b.Property<string>("AboutMyFarm")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("PlatformJoinDate")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FarmerId");

                    b.ToTable("Farmers");
                });

            modelBuilder.Entity("FarmersPlatform.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<int>("FarmerId")
                        .HasColumnType("int");

                    b.Property<double>("FinalPrice")
                        .HasColumnType("float");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("OrderShipped")
                        .HasColumnType("bit");

                    b.Property<string>("RequestMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ShipDate")
                        .HasColumnType("datetime2");

                    b.HasKey("OrderId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("FarmerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("FarmersPlatform.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FarmerId")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("FarmerId");

                    b.HasIndex("OrderId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("FarmersPlatform.Models.Order", b =>
                {
                    b.HasOne("FarmersPlatform.Models.Customer", null)
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FarmersPlatform.Models.Farmer", null)
                        .WithMany("Orders")
                        .HasForeignKey("FarmerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FarmersPlatform.Models.Product", b =>
                {
                    b.HasOne("FarmersPlatform.Models.Farmer", null)
                        .WithMany("Products")
                        .HasForeignKey("FarmerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FarmersPlatform.Models.Order", null)
                        .WithMany("Products")
                        .HasForeignKey("OrderId");
                });

            modelBuilder.Entity("FarmersPlatform.Models.Customer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("FarmersPlatform.Models.Farmer", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("FarmersPlatform.Models.Order", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
