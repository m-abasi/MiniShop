using Application.Cart;
using Application.CQRS;
using Domain.Entities;
using Infrastructure.AppDbContext;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;
using MiniShop.Application.Common.Interfaces;
using Application.Common.Interfaces;
using StackExchange.Redis;

// ── Bootstrap logger ─────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ───────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, services, config) =>
        config.ReadFrom.Configuration(ctx.Configuration)
              .ReadFrom.Services(services)
              .Enrich.FromLogContext());

    // ── MediatR ───────────────────────────────────────────────────────────────
    builder.Services.AddMediatR(typeof(ApplicationAssemblyMarker).Assembly);

    // ── AutoMapper ────────────────────────────────────────────────────────────
    builder.Services.AddAutoMapper(typeof(ApplicationAssemblyMarker).Assembly);

    // ── FluentValidation ──────────────────────────────────────────────────────
    builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

    // ── Database ──────────────────────────────────────────────────────────────
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IApplicationDbContext>(
        provider => provider.GetRequiredService<ApplicationDbContext>());

    // ── Session (for cart) ────────────────────────────────────────────────────
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
    builder.Services.AddHttpContextAccessor();

    // ── Redis ─────────────────────────────────────────────────────────────────
    var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(redisConn));
    builder.Services.AddSingleton<IRedisService, RedisService>();

    // ── Cart (SOLID refactored) ───────────────────────────────────────────────
    builder.Services.AddSingleton<ICartSerializer, JsonCartSerializer>();
    builder.Services.AddScoped<ICartStorage, SessionCartStorage>();
    builder.Services.AddScoped<ICartService, SessionCartService>();

    // ── Identity ──────────────────────────────────────────────────────────────
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // ── Seed Roles & Admin ────────────────────────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        const string adminEmail = "admin@example.com";
        const string adminPassword = "Admin123!";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, FullName = "Administrator" };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // ── Middleware Pipeline ───────────────────────────────────────────────────
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
