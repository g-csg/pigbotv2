using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PigBot.BotCommands;
using PigBot.CommandHandler;
using PigBot.Database;
using PigBot.Etc;
using PigBot.Etc.Images;

namespace PigBot
{
    public class Container
    {
        public static ServiceProvider CreateContainer()
        {
            var configuration = BuildConfig();
            var serviceCollection = new ServiceCollection();
            
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            
            // database
            serviceCollection.AddDbContext<PigbotDbContext>(options => options.UseSqlite(configuration["DataSource"]));
            
            // command handling
            serviceCollection.AddTransient<IBotPolicyGuard, BotPolicyGuard>();
            serviceCollection.AddTransient<IBotCommandHandler, BotCommandHandler>();
            
            // commands
            serviceCollection.AddTransient<IBotCommand, PostCommand>();
            
            // watchers
            serviceCollection.AddTransient<IFourchanWatcher, FourchanWatcher>();
            serviceCollection.AddTransient<IImageDownloadService, ImageDownloadService>();
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        private static IConfiguration BuildConfig()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("config.json");
            var configuration = configurationBuilder.Build();
            return configuration;
        }
    }
}