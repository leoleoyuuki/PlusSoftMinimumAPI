using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using ChllengePlusSoft.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleDbConnection");

builder.Services.AddControllers();
builder.Services.AddHttpClient(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Api empresas clintes PlusSoft",
        Version = "v1.1",
        Description = "Uma API para gerenciar empresas.\n obs: Alguns campos tem regras para adicionar no banco de dados, procure na aba opções os valores"
    });
});

builder.Services.AddSingleton<MLModel>(); // Registre o MLModel como um serviço


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }