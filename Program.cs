
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Programatica.DummyApp.Console.Data;
using Programatica.DummyApp.Console.Models;
using Programatica.Framework.Core.Adapter;
using Programatica.Framework.Data.Context;
using Programatica.Framework.Data.Repository;
using Programatica.Framework.Services;
using Programatica.Framework.Services.Handlers;
using Programatica.Framework.Services.Injector;

IConfiguration configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddEnvironmentVariables()
  .AddCommandLine(args)
  .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.UseInMemoryDatabase(connectionString);

        // di
        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddTransient<IDbContext, AppDbContext>()
                .AddTransient<IDateTimeAdapter, DateTimeAdapter>()
                .AddTransient<IAuthUserAdapter, AuthUserAdapter>()
                .AddTransient<IJsonSerializerAdapter, JsonSerializerAdapter>()
                .AddTransient(typeof(IRepository<>), typeof(Repository<>))
                .AddTransient(typeof(IInjector<>), typeof(Injector<>))
                .AddTransient(typeof(IService<>), typeof(Service<>))
                .AddTransient<IList<IEventHandler<User>>>(p => p.GetServices<IEventHandler<User>>().ToList());
    })
    .Build();

await Init(host.Services);


/* do something with services */
static async Task Init(IServiceProvider services)
{
    using IServiceScope serviceScope = services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var userService = provider.GetRequiredService<IService<User>>();
    await userService.CreateAsync(new User
    {
        Name = "user1",
        Email = "email@server.com",
        Password = "pass"
    });
    var user = await userService.InspectAsync(id: 1);
    Console.WriteLine(@$"
        Id: {user.Id}, 
        SystemId: {user.SystemId}, 
        Name: {user.Name}, 
        CreatedDate: {user.CreatedDate}, 
        CreatedUser: {user.CreatedUser}");
}