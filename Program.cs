using Microsoft.EntityFrameworkCore;
using USUARIOS.Models;
using System.Text.Json.Serialization;
using USUARIOS.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQL Server
builder.Services.AddDbContext<UsuariosContext>(opcion =>
{
    opcion.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL"));
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ----------------------
// CORS: permite que React haga peticiones
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // <- Origen de tu React
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// ----------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ----------------------
// Activar CORS antes de Authorization
app.UseCors("CorsPolicy");
// ----------------------

app.UseAuthorization();

app.MapControllers();

app.Run();
