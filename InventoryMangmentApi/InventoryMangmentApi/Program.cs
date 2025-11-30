using Inventory.Bll.Contract;
using Inventory.Bll.services;
using Inventory.Dal;
using Inventory.Dal.DataContext;
using Inventory.Dal.Identity;
using Microsoft.AspNetCore.Identity;

namespace InventoryMangmentApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Changed from ReferenceHandler.Preserve to IgnoreCycles
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDataAccessServices(builder.Configuration);

            builder.Services.AddIdentity<AppUser, IdentityRole>(opts =>
            {
                opts.Password.RequireDigit = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.Name = "InventoryAuth";
                o.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                o.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                o.LoginPath = "/Account/Login";
            });

            builder.Services.AddScoped<IProductServices, ProductServices>();
            builder.Services.AddScoped<IOrderServices, OrderServices>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {
                    policy.WithOrigins("https://localhost:7172")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                await RoleSeeder.SeedAsync(roleMgr, userMgr);
            }

            app.UseCors("AllowMVC");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}