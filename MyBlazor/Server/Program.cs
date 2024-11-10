using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; 
using Microsoft.IdentityModel.Tokens;
using MyBlazor.Server.Database;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<UsersContext>(
		options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
		sqlOptions => sqlOptions.EnableRetryOnFailure()
	),
	ServiceLifetime.Scoped
);

builder.Services.AddDefaultIdentity<User>(options =>
{
	options.SignIn.RequireConfirmedAccount = true;
	options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<UsersContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	 .AddJwtBearer(options =>
	 {
		 options.TokenValidationParameters = new TokenValidationParameters
		 {
			 ValidateIssuer = true,
			 ValidateAudience = true,
			 ValidateLifetime = true,
			 ValidateIssuerSigningKey = true,
			 ValidIssuer = builder.Configuration["Jwt:Issuer"],
			 ValidAudience = builder.Configuration["Jwt:Audience"],
			 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
		 };
	 });

builder.Services.AddControllers()
	.AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
