using AppCore.DataAccess.Configs;
using AppCore.Utils;
using AppCore.Utils.Bases;
using Business.Services;
using Business.Services.Bases;
using DataAccess.EntityFramework.Contexts;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using YourLanguage.Settings;

namespace YourLanguage
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
            services.AddControllersWithViews();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(config =>
                {
                    config.LoginPath = "/Account/Login";
                    config.AccessDeniedPath = "/Account/AccessDenied";
                    config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    config.SlidingExpiration = true;
                });

            services.AddSession(config =>
            {
                config.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            ConnectionConfig.ConnectionString = Configuration.GetConnectionString("LanguageContext");
            services.AddScoped<DbContext, LanguageContext>();
            services.AddScoped<DomainRepositoryBase, DomainRepository>();
            services.AddScoped<RoleRepositoryBase, RoleRepository>();
            services.AddScoped<UserRepositoryBase, UserRepository>();
            services.AddScoped<UserWordRepositoryBase, UserWordRepository>();
            services.AddScoped<WordRepositoryBase, WordRepository>();
            services.AddScoped<QuestionTestRepositoryBase, QuestionTestRepository>();
            services.AddScoped<SpaceTestRepositoryBase, SpaceTestRepository>();
            services.AddScoped<TopicRepositoryBase, TopicRepository>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IDomainService, DomainService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserWordService, UserWordService>();
            services.AddScoped<IWordService, WordService>();
            services.AddScoped<IQuestionTestService, QuestionTestService>();
            services.AddScoped<ISpaceTestService, SpaceTestService>();
            services.AddScoped<ITopicService, TopicService>();

            AppSettingsUtilBase appSettingsUtil = new AppSettingsUtil(Configuration);
            appSettingsUtil.BindAppSettings<AppSettings>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
