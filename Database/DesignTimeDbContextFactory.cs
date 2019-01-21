using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using PigBot.Database;

namespace PigBot.Database
{
    // this is needed for the migrations to work
    public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<PigbotDbContext>
    {
        public PigbotDbContext CreateDbContext(string[] args)
        {
            var container = Container.CreateContainer();
            return container.GetService<PigbotDbContext>();
        }
    }
}