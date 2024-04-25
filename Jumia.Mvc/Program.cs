using Jumia.Application.Contract;
using Jumia.Application.Services;
using Jumia.Context;
using Jumia.Infrastructure.Repository;
using Jumia.InfraStructure;
using Jumia.InfraStructure.Repository;
using Jumia.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Jumia.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<JumiaContext>(op =>
            {
                op.UseSqlServer(builder.Configuration.GetConnectionString("Db"));
            });
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.Password.RequireDigit = true)
        .AddEntityFrameworkStores<JumiaContext>()
        .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set session timeout duration
                options.LoginPath = "/Account/Login"; // Define login path
                options.LogoutPath = "/Account/Logout"; // Define logout path
                options.SlidingExpiration = true; // Enable sliding expiration
            });

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IItemServices, ItemServices>();
            builder.Services.AddScoped<IItemReposatory, ItemRepostory>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderReposatory, OrderRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductReposatory, ProductRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICategoryReposatory, CategoryReposatory>();
            builder.Services.AddScoped<IProductImageService, ProductImageService>();
            builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IOrderProuduct, OrderProductReposatory>();
            builder.Services.AddScoped<IPaymentReposatory, PaymentRepository>();
            builder.Services.AddScoped<IPaymentServices, PaymentServices>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddLocalization();
            builder.Services.AddSingleton<IStringLocalizerFactory,jsonStringLocalizerFactory>();
            builder.Services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(jsonStringLocalizerFactory));
                   
                     
                   
                }
                );

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCulture = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: supportedCulture[0]);
                options.SupportedCultures = supportedCulture;
                options.SupportedUICultures = supportedCulture;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            var suppotedCulture = new[] { "en-Us", "ar-Eg" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(suppotedCulture[0])
                .AddSupportedCultures(suppotedCulture)
                .AddSupportedUICultures(suppotedCulture);
            app.UseRequestLocalization(localizationOptions);
            app.UseAuthentication();//to check the cockie
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Admin}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
