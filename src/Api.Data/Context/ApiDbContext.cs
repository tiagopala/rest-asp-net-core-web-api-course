using Api.Business.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Api.Data.Context
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Em caso de esquecermos de mapear alguma coluna no banco de dados, esta coluna não será criada como nvarchar(max) e sim como um varchar(100)
            // dessa forma, otimizando o banco de dados 
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                .Where(p => p.ClrType == typeof(string))))
                property.Relational().ColumnType = "varchar(100)";

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiDbContext).Assembly);

            // Removendo "on delete cascade" das tabelas
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }
    }
}
