﻿using Autofac;
using MahantInv.Infrastructure.Data;
using MahantInv.SharedKernel.Interfaces;
using MediatR;
using MediatR.Pipeline;
using System.Collections.Generic;
using System.Reflection;
using Module = Autofac.Module;

namespace MahantInv.Infrastructure
{
    public class DefaultInfrastructureModule : Module
    {
        private readonly bool _isDevelopment = false;
        private readonly List<Assembly> _assemblies = new();

        public DefaultInfrastructureModule(bool isDevelopment, Assembly callingAssembly = null)
        {
            _isDevelopment = isDevelopment;
            var infrastructureAssembly = Assembly.GetAssembly(typeof(StartupSetup));
            _assemblies.Add(infrastructureAssembly);
            if (callingAssembly != null)
            {
                _assemblies.Add(callingAssembly);
            }
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_isDevelopment)
            {
                RegisterDevelopmentOnlyDependencies(builder);
            }
            else
            {
                RegisterProductionOnlyDependencies(builder);
            }
            RegisterCommonDependencies(builder);
        }

        private void RegisterCommonDependencies(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(DapperRepository<>))
                .As(typeof(IAsyncRepository<>))
                .As(typeof(IReadOnlyRepository<>))
                .InstancePerLifetimeScope();

            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            //builder.Register<ServiceFactory>(context =>
            //{
            //    var c = context.Resolve<IComponentContext>();
            //    return t => c.Resolve(t);
            //});

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IRequestExceptionHandler<,,>),
                typeof(IRequestExceptionAction<,>),
                typeof(INotificationHandler<>),
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                .RegisterAssemblyTypes(_assemblies.ToArray())
                .AsClosedTypesOf(mediatrOpenType)
                .AsImplementedInterfaces();
            }

            //builder.RegisterType<EmailService>().As<IEmailService>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<ProductsRepository>().As<IProductsRepository>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<OrdersRepository>().As<IOrdersRepository>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<ProductInventoryRepository>().As<IProductInventoryRepository>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<PartiesRepository>().As<IPartiesRepository>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<BuyersRepository>().As<IBuyersRepository>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<StorageRepository>().As<IStorageRepository>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<ProductUsageRepository>().As<IProductUsageRepository>()
            //    .InstancePerLifetimeScope();
        }

        private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
        {
            // TODO: Add development only services
        }

        private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
        {
            // TODO: Add production only services
        }

    }
}
