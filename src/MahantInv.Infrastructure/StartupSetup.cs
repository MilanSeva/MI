﻿using MahantInv.Infrastructure.Data;
using MahantInv.SharedKernel.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MahantInv.Infrastructure
{
    public static class StartupSetup
    {
        //public static void UseSqlServerUOW(this IServiceCollection services, string connectionString)
        //{
        //    services.AddScoped<UnitOfWork>((serviceProvider) =>
        //    {
        //        return new UnitOfWork(SqlClientFactory.Instance, connectionString);
        //    });

        //    services.AddScoped<IUnitOfWork>((serviceProvider) =>
        //    {
        //        return serviceProvider.GetRequiredService<UnitOfWork>();
        //    });

        //    services.AddScoped<IDapperUnitOfWork>((serviceProvider) =>
        //    {
        //        return serviceProvider.GetRequiredService<UnitOfWork>();
        //    });
        //}
        public static void UseSQLiteUOW(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<UnitOfWork>((serviceProvider) =>
            {
                return new UnitOfWork(SqliteFactory.Instance, connectionString);
            });

            services.AddScoped<IUnitOfWork>((serviceProvider) =>
            {
                return serviceProvider.GetRequiredService<UnitOfWork>();
            });

            services.AddScoped<IDapperUnitOfWork>((serviceProvider) =>
            {
                return serviceProvider.GetRequiredService<UnitOfWork>();
            });
        }
        public static void AddDbContext(this IServiceCollection services, string connectionString) =>
            services.AddDbContext<MIDbContext>(options =>
                options.UseSqlite(connectionString)); // will be created in web project root
    }
}
