using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Library.API.Entities;
using Library.API.Services;
using Library.API.Helpers;
using Microsoft.AspNetCore.Http;
using Library.API.Model;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Library.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LibraryContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Default"]));

            //bağımlılığımızı ekliyoruz.
            services.AddScoped<ILabraryRepository, LibraryRepository>();
            // Add framework services.
            services.AddMvc(setupAction=>
            {
                //serviste açtığımız method json destekliyorsa xml olarak talep edilen isteklere 406 not acceptable hatası verir
                setupAction.ReturnHttpNotAcceptable = true;
                //xml olarak format belirtlemek istiyorsak nuget ten aşağıdaki referans için paket indirmemiz gerekiyor
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appbuilder=>
                {
                    appbuilder.Run(async context=>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened Try again later");
                    });
                });
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Author, Model.AuthorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                     $"{src.FirstName}{src.LastName}"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                     src.DateOfBirth.GetCurrentAge()));

                cfg.CreateMap<Book, BookDto>();
            });




            libraryContext.EnsureSeedDataForContext();
            app.UseMvc();
        }
    }
}
