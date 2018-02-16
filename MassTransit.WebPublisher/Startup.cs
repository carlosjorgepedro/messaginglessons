using System;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.WebPublisher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<MasssTransitModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }

    public class MasssTransitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<BusSettings>()
                .SingleInstance();

            builder.Register(x =>
                {
                    var busSettings = x.Resolve<BusSettings>();

                    return Bus.Factory.CreateUsingRabbitMq(rabbit =>
                    {
                        rabbit.Host(new Uri(busSettings.HostUrl), settings =>
                        {
                            settings.Password(busSettings.Password);
                            settings.Username(busSettings.UserName);
                        });
                    });
                })
                .As<IBusControl>()
                .SingleInstance();


            builder.Register(x =>
            {
                var busSettings = x.Resolve<BusSettings>();
                var busControl = x.Resolve<IBusControl>();
                var task = busControl.GetSendEndpoint(new Uri($"{busSettings.HostUrl}/{busSettings.Queue}"));

                task.Wait();
                return task.Result;
            });
        }
    }
}
