using Microsoft.EntityFrameworkCore;
using Bungalov.DataAccess.Baglam;
using FluentValidation;
using FluentValidation.AspNetCore;
using Bungalov.Business.Interfaces;
using Bungalov.Business.Services;
using Bungalov.Core.Interfaces;
using Bungalov.DataAccess.Repositories;
using Bungalov.Business.Validators;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL kullanıyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection Configurations
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IBungalowService, BungalowService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

// FluentValidation Configurations
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<BungalowValidator>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Bungalow}/{action=Index}/{id?}");

app.Run();