using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Data;

public class BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options) : DbContext(options)
{
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Livro> Livros => Set<Livro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>(entity =>
        {
            entity.ToTable("Autores");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Nome).IsRequired().HasMaxLength(150);
            entity.Property(a => a.Nacionalidade).IsRequired().HasMaxLength(80);

            // Um autor possui vários livros; não é permitido excluir autor com livros vinculados.
            entity.HasMany(a => a.Livros)
                  .WithOne(l => l.Autor)
                  .HasForeignKey(l => l.AutorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Livro>(entity =>
        {
            entity.ToTable("Livros");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(l => l.Isbn).IsRequired().HasMaxLength(13);
            entity.HasIndex(l => l.Isbn).IsUnique();
            entity.Property(l => l.Genero).IsRequired().HasMaxLength(60);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var dataSeed = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Autor>().HasData(
            new Autor { Id = 1, Nome = "Machado de Assis", Nacionalidade = "Brasileira", DataNascimento = new DateOnly(1839, 6, 21), CriadoEm = dataSeed },
            new Autor { Id = 2, Nome = "Clarice Lispector", Nacionalidade = "Brasileira", DataNascimento = new DateOnly(1920, 12, 10), CriadoEm = dataSeed });

        modelBuilder.Entity<Livro>().HasData(
            new Livro { Id = 1, Titulo = "Dom Casmurro", Isbn = "9788535910663", Genero = "Romance", AnoPublicacao = 1899, QuantidadeEstoque = 5, AutorId = 1, CriadoEm = dataSeed },
            new Livro { Id = 2, Titulo = "Memórias Póstumas de Brás Cubas", Isbn = "9788535911015", Genero = "Romance", AnoPublicacao = 1881, QuantidadeEstoque = 3, AutorId = 1, CriadoEm = dataSeed },
            new Livro { Id = 3, Titulo = "A Hora da Estrela", Isbn = "9788532508126", Genero = "Romance", AnoPublicacao = 1977, QuantidadeEstoque = 4, AutorId = 2, CriadoEm = dataSeed });
    }
}
