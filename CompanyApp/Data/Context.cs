using CompanyApp.Models;
using Microsoft.EntityFrameworkCore;
namespace CompanyApp.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        { }
        public DbSet<PeopleFact> PeopleFacts { get; set; }
        public DbSet<PeoplePlan> PeoplePlans { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }
        public DbSet<SubdivisionPlan> SubdivisionPlans { get; set; }
        public DbSet<SubdivisionFact> SubdivisionFacts { get; set; }
        public DbSet<Workpeople> Workpeoples { get; set; }
    }
}
