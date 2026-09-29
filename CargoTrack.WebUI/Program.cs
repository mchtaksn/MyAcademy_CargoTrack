using CargoTrack.Business;
using CargoTrack.Business.Mappings;
using CargoTrack.Business.Services.Abouts;
using CargoTrack.Business.Services.AuditLogs;
using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.CargoDelayPrediction;
using CargoTrack.Business.Services.CargoPricing;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.Business.Services.Cities;
using CargoTrack.Business.Services.Dashboard;
using CargoTrack.Business.Services.DynamicPricing;
using CargoTrack.Business.Services.Employees;
using CargoTrack.Business.Services.PricingRules;
using CargoTrack.Business.Services.RouteOptimization;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abouts;
using CargoTrack.DataAccess.Repositories.AuditLogs;
using CargoTrack.DataAccess.Repositories.Branches;
using CargoTrack.DataAccess.Repositories.CargoMovements;
using CargoTrack.DataAccess.Repositories.CargoPrices;
using CargoTrack.DataAccess.Repositories.Cargos;
using CargoTrack.DataAccess.Repositories.Cities;
using CargoTrack.DataAccess.Repositories.Deliveries;
using CargoTrack.DataAccess.Repositories.DeliveryExceptions;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DataAccess.Repositories.PricingRules;
using CargoTrack.DataAccess.Repositories.TransferCenters;
using CargoTrack.Entity.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
MapsterConfig.Configure();

// Add services to the container.

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<BusinessAssembly>();

// Repositories
builder.Services.AddScoped<IAboutRepository, AboutRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<ICargoMovementRepository, CargoMovementRepository>();
builder.Services.AddScoped<ITransferCenterRepository, TransferCenterRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ICargoPriceRepository, CargoPriceRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IDeliveryExceptionRepository, DeliveryExceptionRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IPricingRuleRepository, PricingRuleRepository>();


// Services
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<ITransferCenterService, TransferCenterService>();
builder.Services.AddScoped<IPricingRuleService, PricingRuleService>();
builder.Services.AddScoped<ICargoPricingService, CargoPricingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICargoDelayPredictionService, CargoDelayPredictionService>();
builder.Services.AddScoped<IRouteOptimizationService, RouteOptimizationService>();
builder.Services.AddSingleton<IDynamicPricingService, DynamicPricingService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});



builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseExceptionHandler("/Home/Error");

app.UseMiddleware<CargoTrack.WebUI.Middlewares.ExceptionLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
           name: "areas",
           pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
         );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Uygulama başlarken Seed Data ekleme işlemi
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    // Veritabanının var olduğundan emin olun (Migration kullanıyorsanız context.Database.Migrate() yapın)
    context.Database.EnsureCreated();

    // Eğer veritabanında hiç şehir yoksa ekle
    if (!context.Cities.Any())
    {
        var cities = new List<City>
        {
            new City { Id = Guid.NewGuid(), Name = "İstanbul" },
            new City { Id = Guid.NewGuid(), Name = "Ankara" },
            new City { Id = Guid.NewGuid(), Name = "İzmir" },
            new City { Id = Guid.NewGuid(), Name = "Bursa" },
            new City { Id = Guid.NewGuid(), Name = "Antalya" },
            new City { Id = Guid.NewGuid(), Name = "Adana" },
            new City { Id = Guid.NewGuid(), Name = "Konya" },
            new City { Id = Guid.NewGuid(), Name = "Şanlıurfa" },
            new City { Id = Guid.NewGuid(), Name = "Gaziantep" },
            new City { Id = Guid.NewGuid(), Name = "Kocaeli" },
            new City { Id = Guid.NewGuid(), Name = "Mersin" },
            new City { Id = Guid.NewGuid(), Name = "Diyarbakır" },
            new City { Id = Guid.NewGuid(), Name = "Hatay" },
            new City { Id = Guid.NewGuid(), Name = "Kayseri" },
            new City { Id = Guid.NewGuid(), Name = "Samsun" },
            new City { Id = Guid.NewGuid(), Name = "Balıkesir" },
            new City { Id = Guid.NewGuid(), Name = "Kahramanmaraş" },
            new City { Id = Guid.NewGuid(), Name = "Van" },
            new City { Id = Guid.NewGuid(), Name = "Aydın" },
            new City { Id = Guid.NewGuid(), Name = "Tekirdağ" }
        };

        context.Cities.AddRange(cities);
        context.SaveChanges();
    }

    // Roller
    string[] roleNames = { "Admin", "Manager", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new AppRole { Name = roleName });
        }
    }

    // Şubeler (en az 2 tane, farklı şehirlerde — test için gerekli)
    if (!context.Branches.Any())
    {
        var istanbul = context.Cities.First(c => c.Name == "İstanbul");
        var ankara = context.Cities.First(c => c.Name == "Ankara");

        var branches = new List<Branch>
        {
            new Branch { Id = Guid.NewGuid(), Name = "İstanbul Ataşehir Şubesi", CityId = istanbul.Id, Address = "Ataşehir, İstanbul", IsActive = true },
            new Branch { Id = Guid.NewGuid(), Name = "Ankara Çankaya Şubesi", CityId = ankara.Id, Address = "Çankaya, Ankara", IsActive = true }
        };
        context.Branches.AddRange(branches);
        context.SaveChanges();
    }

    // Test kullanıcıları: 1 Manager (Employee kaydıyla birlikte) + 2 müşteri
    if (await userManager.FindByEmailAsync("manager@cargotrack.com") is null)
    {
        var managerUser = new AppUser
        {
            UserName = "manager@cargotrack.com",
            Email = "manager@cargotrack.com",
            FirstName = "Ahmet",
            LastName = "Yönetici",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(managerUser, "Manager123!");
        await userManager.AddToRoleAsync(managerUser, "Manager");

        var istanbulBranch = context.Branches.First(b => b.Name.Contains("İstanbul"));
        context.Employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            AppUserId = managerUser.Id,
            BranchId = istanbulBranch.Id,
            Position = "Şube Müdürü",
            IsActive = true
        });
        context.SaveChanges();
    }

    if (await userManager.FindByEmailAsync("musteri1@cargotrack.com") is null)
    {
        var customer1 = new AppUser
        {
            UserName = "musteri1@cargotrack.com",
            Email = "musteri1@cargotrack.com",
            FirstName = "Murat",
            LastName = "Yücedağ",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(customer1, "Musteri123!");
        await userManager.AddToRoleAsync(customer1, "User");
    }

    if (await userManager.FindByEmailAsync("musteri2@cargotrack.com") is null)
    {
        var customer2 = new AppUser
        {
            UserName = "musteri2@cargotrack.com",
            Email = "musteri2@cargotrack.com",
            FirstName = "Ahmet",
            LastName = "Demir",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(customer2, "Musteri123!");
        await userManager.AddToRoleAsync(customer2, "User");
    }
    if (await userManager.FindByEmailAsync("admin@cargotrack.com") is null)
    {
        var adminUser = new AppUser
        {
            UserName = "admin@cargotrack.com",
            Email = "admin@cargotrack.com",
            FirstName = "Mücahit",
            LastName = "Aksın",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.MapGet("/", () => "Uygulama çalışıyor ve Seed Data kontrol edildi!");

app.Run();