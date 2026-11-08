using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;
using MyProject.Services;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não definido");

builder.Services.AddDbContext<MyProjectContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0)))
);
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Aplica as migrations e popula os departamentos iniciais
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MyProjectContext>();
    context.Database.Migrate();

    if (!context.Department.Any())
    {
        context.Department.AddRange(
            new Department { Name = "Computers" },
            new Department { Name = "Electronics" },
            new Department { Name = "Fashion" },
            new Department { Name = "Books" }
        );
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
