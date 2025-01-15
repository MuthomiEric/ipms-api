using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Data;
public class InsuranceContext : IdentityDbContext<SystemUser>
{
    public InsuranceContext(DbContextOptions<InsuranceContext> options) : base(options) { }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
}