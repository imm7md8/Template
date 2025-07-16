using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using WebApplication.Infrastructure;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllersWithViews();
//swagger region

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.MapType<DateTime>(() => new OpenApiSchema { Format = "dd/MM/yyyy" });
	c.MapType<DateTime?>(() => new OpenApiSchema { Format = "dd/MM/yyyy" });
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "WinVeston API", Version = "v1" });

	//var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	//var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	//c.IncludeXmlComments(xmlPath);
	c.TagActionsBy(api =>
	{
		if (api.GroupName != null)
		{
			return new[] { api.GroupName };
		}

		var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
		if (controllerActionDescriptor != null)
		{
			return new[] { controllerActionDescriptor.ControllerName };
		}

		throw new InvalidOperationException("Unable to determine tag for endpoint.");
	});
	c.DocInclusionPredicate((name, api) => true);
	c.AddSecurityDefinition("JWT",
		new OpenApiSecurityScheme
		{
			BearerFormat = "JWT",
			In = ParameterLocation.Header,
			Description = "Please insert JWT with Bearer into field",
			Name = "Authorization",
			Type = SecuritySchemeType.ApiKey,
		});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement {
				{
					new OpenApiSecurityScheme {
						Reference = new OpenApiReference {
							Type = ReferenceType.SecurityScheme,
							Id = "JWT"
						}
					},
					new string[] { }
					}
		   });
});
#endregion

//end of swagger region

//jwt region
#region 

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings.GetValue<string>("Key");

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
		ValidAudience = jwtSettings.GetValue<string>("Audience"),
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
	};
});
builder.Services.AddAuthorization(op =>
{
	op.AddPolicy(secretKey, 
		new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

});

#endregion
// End of jwt region
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
	c.RoutePrefix = ""; // Available at /swagger
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
