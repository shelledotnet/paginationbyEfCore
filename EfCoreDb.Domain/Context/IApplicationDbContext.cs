using EfCoreDb.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EfCoreDb.Domain.Context
{
    public interface IApplicationDbContext
    {
        DbSet<Student> Students { get; set; }
        DbSet<Customer>  Customers { get; set; }
        Task<int> SaveChanges();
    }
}