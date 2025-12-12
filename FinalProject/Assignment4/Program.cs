//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
using Assignment4.Services;
using Assignment4.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(x => x.AddPolicy("permissive", builder =>
			{
				builder.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader();
			}));

builder.Services.AddSingleton<MCPricer>();

builder.Services.AddDbContext<optionContext>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("permissive");

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapControllers();

app.Run();
