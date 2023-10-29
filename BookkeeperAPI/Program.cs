using BookkeeperAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Newtonsoft.Json;
using BookkeeperAPI.Middlewares;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Npgsql;
using BookkeeperAPI.Constants;
using BookkeeperAPI.Service.Interface;
using BookkeeperAPI.Service;
using BookkeeperAPI.Repository.Interface;
using BookkeeperAPI.Repository;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

SecretClient client = new SecretClient(new Uri($"https://{configuration["Keyvault"]}.vault.azure.net"), new DefaultAzureCredential());

builder.Configuration.AddAzureKeyVault(client, new KeyVaultSecretManager());

NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration["BookkeeperDB"]);
dataSourceBuilder.MapEnum<ExpenseCategory>();
NpgsqlDataSource dataSource = dataSourceBuilder.Build();

// Add DB context to connect to database
builder.Services.AddDbContext<BookkeeperContext>(
    optionsBuilder => optionsBuilder.UseNpgsql(dataSource)
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((options) =>
{
    options.DescribeAllParametersInCamelCase();
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Bookkeeper API" });
    options.UseInlineDefinitionsForEnums();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddCors((options) =>
{
    options.AddDefaultPolicy((policy) =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseCors();

app.UseMiddleware<ExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
