using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using MyBlazor.Server.Database;
using System.Text;

var builder = WebApplication.CreateSlimBuilder(args);

//builder.Services.AddRazorComponents()
//				.AddInteractiveServerComponents();

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

//builder.Services.ConfigureHttpJsonOptions(options =>
//{
//	options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
//});

builder.WebHost.UseKestrelHttpsConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToFile("index.html");
app.MapControllers();

//var sampleTodos = new Todo[] {
//	new(1, "Walk the dog"),
//	new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
//	new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
//	new(4, "Clean the bathroom"),
//	new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
//};

//var todosApi = app.MapGroup("/todos");
//todosApi.MapGet("/", () => sampleTodos);
//todosApi.MapGet("/{id}", (int id) =>
//	sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
//		? Results.Ok(todo)
//		: Results.NotFound());

app.Run();

//public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

//[JsonSerializable(typeof(Todo[]))]
//internal partial class AppJsonSerializerContext : JsonSerializerContext
//{

//}
