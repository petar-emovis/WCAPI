using WC.DataAccess.Bundle;
using WC.DataAccess.SqlServer.Map;
using WC.DataAccess.SqlServer.Models;
using WC.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// register db/data access
//builder.Services.AddWcDataAccess(builder.Configuration);
builder.Services.AddWcDataAccess();

// register services
builder.Services.AddScoped<IWcManagementService, WcManagementService>();
//builder.Services.AddAutoMapper(typeof(WhichCountryContext).Assembly);
builder.Services.AddAutoMapper(cfg => { }, typeof(WhichCountryContext).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
