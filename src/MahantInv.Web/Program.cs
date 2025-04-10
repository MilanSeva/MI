﻿using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MahantInv.Infrastructure;
using MahantInv.Infrastructure.Data;
using MahantInv.Infrastructure.Identity;
using MahantInv.Infrastructure.Interfaces;
using MahantInv.Infrastructure.SeedScripts;
using MahantInv.SharedKernel.Filter;
using MahantInv.Web.Service;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ConfigureEndpointDefaults(io => io.Protocols = HttpProtocols.Http1AndHttp2);
//});

var services = builder.Services;
services.AddControllers(options =>
{
    options.Filters.Add<HttpGlobalExceptionFilter>();
});
services.AddControllers(options => { options.Filters.Add<HttpGlobalExceptionFilter>(); });
string connectionString = builder.Configuration.GetConnectionString("MahantInventoryDB");
//Db Connection
builder.Services.AddTransient<DbConnection>(sp =>
{
    var dbProviderFactory = SqliteFactory.Instance;
    var connection = dbProviderFactory.CreateConnection();
    connection.ConnectionString = connectionString;
    return connection;
});
services.UseSQLiteUOW(connectionString);
services.AddDbContext<MIDbContext>(
    options =>
    {
        options.UseSqlite(connectionString,mh=>mh.MigrationsHistoryTable("MigrationHistory"));
        options.EnableDetailedErrors(true);
    });

services.AddControllersWithViews().AddNewtonsoftJson();

services.AddRazorPages();
services.AddIdentity<MIIdentityUser, MIIdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
})
    .AddEntityFrameworkStores<MIDbContext>()
    .AddDefaultTokenProviders();

services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));
services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(600);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
});

services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2 * 1024 * 1024; // 2 MB
});

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
services.Configure<ServiceConfig>(config =>
{
    config.Services = new List<ServiceDescriptor>(services);

    // optional - default path to view services is /listallservices - recommended to choose your own path
    config.Path = "/listservices";
});


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});
builder.Host.UseSerilog(CreateSerilogLogger(builder.Configuration));

builder.Services.AddMediatR(options =>
options.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly())
);

builder.Services.AddTransient<IBuyersRepository, BuyersRepository>();
builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
builder.Services.AddTransient<IPartiesRepository, PartiesRepository>();
builder.Services.AddTransient<IProductInventoryRepository, ProductInventoryRepository>();
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
builder.Services.AddTransient<IProductUsageRepository, ProductUsageRepository>();
builder.Services.AddTransient<IStorageRepository, StorageRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<GoogleCaptchaService>();
builder.Services.AddScoped<IAdHocRepo, AdHocRepo>();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//builder.Host.ConfigureContainer<ContainerBuilder>(cb => cb.Populate(builder.Services));
// Register Autofac modules
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new DefaultInfrastructureModule(false));
});
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(s => s.EnableTryItOutByDefault());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "VIA.Core.API");
        s.EnableTryItOutByDefault();
    }
    );
}

using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider;
    var context = seedService.GetRequiredService<MIDbContext>();
    await context.Database.EnsureCreatedAsync();

    var userManager = seedService.GetRequiredService<UserManager<MIIdentityUser>>();
    var roleManager = seedService.GetRequiredService<RoleManager<MIIdentityRole>>();
    var mediator = seedService.GetRequiredService<IMediator>();
    await SeedData.Initialize(seedService, mediator, userManager, roleManager);
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
//app.MapControllers();

app.UseCors(MyAllowSpecificOrigins);


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();

//Serilog Configurations
Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    return new LoggerConfiguration()
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

