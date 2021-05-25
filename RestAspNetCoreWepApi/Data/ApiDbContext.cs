using Microsoft.EntityFrameworkCore;
using RestAspNetCoreWepApi.Models;

namespace RestAspNetCoreWepApi.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Fornecedor> Fornecedores { get; set; }
    }
}
