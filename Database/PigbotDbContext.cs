using Microsoft.EntityFrameworkCore;

namespace PigBot.Database
{
    public class PigbotDbContext : DbContext
    {
        public PigbotDbContext(DbContextOptions<PigbotDbContext> options)
            : base(options)
        { } 
        
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<BlackList> BlackLists { get; set; }
        public DbSet<CommandPolicy> CommandPolicies { get; set; }
        public DbSet<CoolDownPolicy> CoolDownPolicies { get; set; }
        public DbSet<FourchanPost> FourchanPosts { get; set; }
    }
}