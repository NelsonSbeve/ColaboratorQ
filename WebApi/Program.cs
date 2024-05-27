using Microsoft.EntityFrameworkCore;

using Application.Services;
using DataModel.Repository;
using DataModel.Mapper;
using Domain.Factory;
using Domain.IRepository;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
string queueName = "Q1";
var port = GetPortForQueue(queueName);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AbsanteeContext>(opt =>
    //opt.UseInMemoryDatabase("AbsanteeList")
    //opt.UseSqlite("Data Source=AbsanteeDatabase.sqlite")
    opt.UseSqlite(Host.CreateApplicationBuilder().Configuration.GetConnectionString(queueName))
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
    opt.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Example = new OpenApiString(DateTime.Today.ToString("yyyy-MM-dd"))
    })
);


builder.Services.AddTransient<IColaboratorRepository, ColaboratorRepository>();
builder.Services.AddTransient<IColaboratorFactory, ColaboratorFactory>();
builder.Services.AddTransient<ColaboratorMapper>();
builder.Services.AddTransient<ColaboratorService>();
builder.Services.AddSingleton<IColaboratorConsumer, ColaboratorConsumer>();







var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAllOrigins");


var rabbitMQConsumerService = app.Services.GetRequiredService<IColaboratorConsumer>();
rabbitMQConsumerService.StartColaboratorConsuming(queueName);
 

app.Run($"https://localhost:{port}");

static int GetPortForQueue(string queueName)
{
    // Implement logic to map queue name to a unique port number
    // Example: Assign a unique port number based on the queue name suffix
    int basePort = 5000; // Start from port 5000
    int queueIndex = int.Parse(queueName.Substring(1)); // Extract the numeric part of the queue name (assuming it starts with 'Q')
    return basePort + queueIndex;
}

public partial class Program { }