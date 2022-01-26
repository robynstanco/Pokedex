using Microsoft.EntityFrameworkCore;
using Pokedex.Data.Models;

namespace Pokedex.Data
{
    public partial class POKEDEXDBContext : DbContext
    {
        public POKEDEXDBContext()
        {
        }

        public POKEDEXDBContext(DbContextOptions<POKEDEXDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<tblMyPokedex> tblMyPokedex { get; set; }
        public virtual DbSet<tlkpAbility> tlkpAbility { get; set; }
        public virtual DbSet<tlkpCategory> tlkpCategory { get; set; }
        public virtual DbSet<tlkpNationalDex> tlkpNationalDex { get; set; }
        public virtual DbSet<tlkpPokeball> tlkpPokeball { get; set; }
        public virtual DbSet<tlkpType> tlkpType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;initial catalog=POKEDEXDB;integrated security=True;");
            }
        }

        /// <summary>
        /// Build/Configure all models with their desired properties & constraints.
        /// </summary>
        /// <param name="modelBuilder">Model builder to configure</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblMyPokedex>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Location).IsUnicode(false);

                entity.Property(e => e.Nickname).IsUnicode(false);

                entity.HasOne(d => d.Pokeball)
                    .WithMany(p => p.tblMyPokedex)
                    .HasForeignKey(d => d.PokeballId)
                    .HasConstraintName("FK_tblMyPokedex_tlkpPokeball");

                entity.HasOne(d => d.Pokemon)
                    .WithMany(p => p.tblMyPokedex)
                    .HasForeignKey(d => d.PokemonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblMyPokedex_tlkpNationalPokedex");
            });

            modelBuilder.Entity<tlkpAbility>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<tlkpCategory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<tlkpNationalDex>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.ImageURL).IsUnicode(false);

                entity.Property(e => e.JapaneseName).IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.Ability)
                    .WithMany(p => p.tlkpNationalDexAbility)
                    .HasForeignKey(d => d.AbilityId)
                    .HasConstraintName("FK_tlkpNationalDex_tlkpAbility1");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.tlkpNationalDex)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_tlkpNationalDex_tlkpCategory");

                entity.HasOne(d => d.HiddenAbility)
                    .WithMany(p => p.tlkpNationalDexHiddenAbility)
                    .HasForeignKey(d => d.HiddenAbilityId)
                    .HasConstraintName("FK_tlkpNationalDex_tlkpAbility2");

                entity.HasOne(d => d.TypeOne)
                    .WithMany(p => p.tlkpNationalDexTypeOne)
                    .HasForeignKey(d => d.TypeOneId)
                    .HasConstraintName("FK_tlkpNationalDex_tlkpType1");

                entity.HasOne(d => d.TypeTwo)
                    .WithMany(p => p.tlkpNationalDexTypeTwo)
                    .HasForeignKey(d => d.TypeTwoId)
                    .HasConstraintName("FK_tlkpNationalDex_tlkpType2");
            });

            modelBuilder.Entity<tlkpPokeball>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageURL).IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<tlkpType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}