﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SancScan.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230702020305_addBook_Borrow_Login")]
    partial class addBook_Borrow_Login
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SancScan.Models.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookId"), 1L, 1);

                    b.Property<string>("AuthorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BookName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("DoesExist")
                        .HasColumnType("bit");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ImageFullPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ImageSize")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsBorrowed")
                        .HasColumnType("bit");

                    b.HasKey("BookId");

                    b.ToTable("Book");
                });

            modelBuilder.Entity("SancScan.Models.Borrow", b =>
                {
                    b.Property<int>("BorrowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BorrowId"), 1L, 1);

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<DateTime>("BorrowDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("BorrowerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("BringBackDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateToBeReturned")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsBroughtBack")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ReceivedBackTime")
                        .HasColumnType("datetime2");

                    b.HasKey("BorrowId");

                    b.HasIndex("BookId");

                    b.ToTable("Borrow");
                });

            modelBuilder.Entity("SancScan.Models.Login", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Login");
                });

            modelBuilder.Entity("SancScan.Models.Borrow", b =>
                {
                    b.HasOne("SancScan.Models.Book", "Book")
                        .WithMany("Borrow")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");
                });

            modelBuilder.Entity("SancScan.Models.Book", b =>
                {
                    b.Navigation("Borrow");
                });
#pragma warning restore 612, 618
        }
    }
}
