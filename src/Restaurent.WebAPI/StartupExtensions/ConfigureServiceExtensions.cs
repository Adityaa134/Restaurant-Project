using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Service;
using Restaurent.Core.ServiceContracts;
using Restaurent.Infrastructure.DBContext;
using Restaurent.Infrastructure.Repositories;

namespace Restaurent.WebAPI.StartupExtensions
{
    public static class ConfigureServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IDishAdderService, DishAdderService>();
            services.AddScoped<IDishGetterService, DishGetterService>();
            services.AddScoped<IDishUpdateService, DishUpdateService>();
            services.AddScoped<IDishDeleteService, DishDeleteService>();
            services.AddScoped<ICategoriesGetterService, CategoriesGetterService>();
            services.AddScoped<IImageAdderService, ImageAdderService>();
            services.AddScoped<IImageDeleteService, ImageDeleteService>();
            services.AddScoped<IImageUpdateService, ImageUpdateService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IAddCartItemsService, AddCartItemsService>();
            services.AddScoped<IGetCartItemsService, GetCartItemsService>();
            services.AddScoped<IRemoveCartItemsService, RemoveCartItemsService>();
            services.AddScoped<IUpdateItemQuantityInCart, UpdateItemQuantityInCart>();
            services.AddScoped<IOrderGetterService, OrderGetterService>();
            services.AddScoped<IOrderCreateService, OrderCreateService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IOrderUpdateService, OrderUpdateService>();
            services.AddScoped<ICategoryAdderService, CategoryAdderService>();
            services.AddScoped<ICategoryUpdateService, CategoryUpdateService>();
            services.Configure<SMTPConfigOptions>(configuration.GetSection("SMTPConfig"));

            services.AddScoped<IDishRepository, DishRepository>();
            services.AddScoped<ICategoryRepository, CategoriesRepository>();
            services.AddScoped<ICartsRepository, CartsRepository>();
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                //Password Complexity
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredUniqueChars = 3; // a password must have 3 diff. characters eg aditya it has 5 unique characters
                options.SignIn.RequireConfirmedEmail = true;
            })

           .AddEntityFrameworkStores<ApplicationDBContext>()

           .AddDefaultTokenProviders()

           .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDBContext, Guid>>()

           .AddRoleStore<RoleStore<ApplicationRole, ApplicationDBContext, Guid>>();
        }
    }
}
