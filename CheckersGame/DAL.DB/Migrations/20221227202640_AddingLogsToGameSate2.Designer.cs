﻿// <auto-generated />
using System;
using DAL.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.DB.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20221227202640_AddingLogsToGameSate2")]
    partial class AddingLogsToGameSate2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0-rc.2.22472.11");

            modelBuilder.Entity("ProjectDomain.CheckersGame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CheckersOptionId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("GameOverAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("GameWonByPlayer")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Player1Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<int>("Player1Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Player2Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<int>("Player2Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CheckersOptionId");

                    b.ToTable("CheckersGames");
                });

            modelBuilder.Entity("ProjectDomain.CheckersGameState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CheckersGameId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("SerializedGameState")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CheckersGameId");

                    b.ToTable("CheckersGameStates");
                });

            modelBuilder.Entity("ProjectDomain.CheckersOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Height")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RandomMoves")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("WhiteStarts")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Width")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("CheckersOptions");
                });

            modelBuilder.Entity("ProjectDomain.MovementLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CheckersGameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CheckersGameStateId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EatenCheckerX")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EatenCheckerY")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MovementFromX")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MovementFromY")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MovementToX")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MovementToY")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WhoMoved")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CheckersGameId");

                    b.HasIndex("CheckersGameStateId")
                        .IsUnique();

                    b.ToTable("MovementLogs");
                });

            modelBuilder.Entity("ProjectDomain.CheckersGame", b =>
                {
                    b.HasOne("ProjectDomain.CheckersOption", "CheckersOption")
                        .WithMany("CheckersGames")
                        .HasForeignKey("CheckersOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CheckersOption");
                });

            modelBuilder.Entity("ProjectDomain.CheckersGameState", b =>
                {
                    b.HasOne("ProjectDomain.CheckersGame", "CheckersGame")
                        .WithMany("CheckersGameStates")
                        .HasForeignKey("CheckersGameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CheckersGame");
                });

            modelBuilder.Entity("ProjectDomain.MovementLog", b =>
                {
                    b.HasOne("ProjectDomain.CheckersGame", "CheckersGame")
                        .WithMany("MovementLogs")
                        .HasForeignKey("CheckersGameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProjectDomain.CheckersGameState", "CheckersGameState")
                        .WithOne("MovementLog")
                        .HasForeignKey("ProjectDomain.MovementLog", "CheckersGameStateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CheckersGame");

                    b.Navigation("CheckersGameState");
                });

            modelBuilder.Entity("ProjectDomain.CheckersGame", b =>
                {
                    b.Navigation("CheckersGameStates");

                    b.Navigation("MovementLogs");
                });

            modelBuilder.Entity("ProjectDomain.CheckersGameState", b =>
                {
                    b.Navigation("MovementLog");
                });

            modelBuilder.Entity("ProjectDomain.CheckersOption", b =>
                {
                    b.Navigation("CheckersGames");
                });
#pragma warning restore 612, 618
        }
    }
}
