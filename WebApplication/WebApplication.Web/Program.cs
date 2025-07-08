using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApplication.Infrastructure;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "MyApp API",
		Version = "v1",
		Description = "MyApp Clean Architecture API"
	});

	// Enable support for controllers inside Areas
	c.CustomOperationIds(apiDesc =>
		apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
});

var app = builder.Build();
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApp API v1");
	c.RoutePrefix = "swagger"; // Available at /swagger
});
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "areas",
	pattern: "{area=Web}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
