using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

HelperActionOAuthService helper = new HelperActionOAuthService(builder.Configuration);

//ESTA INSTANCIA SOLAMENTE DEBEMOS CREARLA UNA VEZ
builder.Services.AddSingleton<HelperActionOAuthService>(helper);
builder.Services.AddSingleton<HelperCifrado>();
//HABILITAMOS LA SEGURIDAD DENTRO DE PROGRAM
builder.Services.AddAuthentication(helper.GetAuthenticationSchema())
    .AddJwtBearer(helper.GetJWTBearerOptions());

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
string connectionString = builder.Configuration.GetConnectionString("SqlHospital");
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<RepositoryHospital>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
