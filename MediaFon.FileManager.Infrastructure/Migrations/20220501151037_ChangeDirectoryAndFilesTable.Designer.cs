﻿// <auto-generated />
using System;
using MediaFon.FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MediaFon.FileManager.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220501151037_ChangeDirectoryAndFilesTable")]
    partial class ChangeDirectoryAndFilesTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MediaFon.FileManager.Domain.Entity.Directory", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool>("HasAccessPermission")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastAccessTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastAccessTimeUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastWriteTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastWriteTimeUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RemotePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasKey("Name");

                    b.ToTable("Directories");
                });

            modelBuilder.Entity("MediaFon.FileManager.Domain.Entity.EventLogs", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("JobEndedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("JobIntervalInMinute")
                        .HasColumnType("integer");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("JobStartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("JobType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EventLogs");
                });

            modelBuilder.Entity("MediaFon.FileManager.Domain.Entity.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("DirectoryName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Extention")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<bool>("HasAccessPermission")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastAccessTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastAccessTimeUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastWriteTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastWriteTimeUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RemotePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
