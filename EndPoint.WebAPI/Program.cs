using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using MyAPI.Common;
using MyAPI.Common.Helper;
using MyAPI.Data;
using MyAPI.Data.Contracts;
using MyAPI.Data.Repositories;
using MyAPI.Services.Services;
using MyAPI.WebFramework.Configuration;
using MyAPI.WebFramework.Middlewares;
using NLog;
using NLog.Web;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
//logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    
    builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));
    
    builder.Services.AddDbContext<ApplicationDbContext>(option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });
    builder.Services.AddElmah<SqlErrorLog>(option =>
    {
        option.Path = "/elmah-errors";
        option.ConnectionString = builder.Configuration.GetConnectionString("Elmah");
    });
   

    //Register Repository
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddJwtAuthentication();

    //AddAuthorization
    //Option 1
    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });
    //Option 2
    //builder.Services.AddControllers(config =>
    //{
    //    var policy = new AuthorizationPolicyBuilder()
    //                     .RequireAuthenticatedUser()
    //                     .Build();
    //    config.Filters.Add(new AuthorizeFilter(policy));
    //});

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    //other classes that need the logger 
    builder.Services.AddTransient<GenericHelper>();


    var app = builder.Build();
    app.UseCustomExceptionHandler();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseElmah();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
   
    string type = exception.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        //NLog:catch setup errors
        logger.Error(exception, "Stopeed program because of exception");
        throw;
    }

    logger.Fatal(exception, "Unhandled exception");
    
    
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
    
}


