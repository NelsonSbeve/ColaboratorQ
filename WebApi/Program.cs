using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Application.Services;
using DataModel.Repository;
using DataModel.Mapper;
using Domain.Factory;
using Domain.IRepository;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Microsoft.OpenApi.Any;
<<<<<<< HEAD
 
var builder = WebApplication.CreateBuilder(args);
 
var config = builder.Configuration;
var config2 = builder.Configuration;
 
=======
using RabbitMQ.Client;



var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
>>>>>>> main
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
 
string queueNameArg = Array.Find(args, arg => arg.Contains("--queueName"));
string queueName = "Q1";
 
// if (queueNameArg != null)
//     queueName = queueNameArg.Split('=')[1];
// else
//     queueName = config["queueName"] ?? config.GetConnectionString("queueName");
 
 
var port = GetPortForQueue(queueName);
<<<<<<< HEAD
 
=======

>>>>>>> main
var HostName = "rabbitmq";
var PortMQ = "5672";
var UserName = "guest";
var Password = "guest";
 
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    return new ConnectionFactory()
    {
        HostName = HostName,
        Port = int.Parse(PortMQ),
        UserName = UserName,
        Password = Password
    };
});
<<<<<<< HEAD
 
=======

var DBConnectionString = config.GetConnectionString("DBConnectionString");
 
builder.Services.AddDbContext<AbsanteeContext>(options =>
{
    options.UseNpgsql(DBConnectionString);
});
 
builder.Services.AddDbContextFactory<AbsanteeContext>(options =>
{
    options.UseNpgsql(DBConnectionString);
}, ServiceLifetime.Scoped);

Console.WriteLine("DBConnectionString: " + DBConnectionString);

RabbitMqConfiguration rabbitMqConfig = config.DefineRabbitMqConfiguration();
Console.WriteLine("RabbitMqConfig: " + rabbitMqConfig.Hostname);

// Add services to the container.

>>>>>>> main
builder.Services.AddControllers();
 
var DBConnectionString = config.GetConnectionString("DBConnectionString");
 
builder.Services.AddDbContext<AbsanteeContext>(options =>
{
    options.UseNpgsql(DBConnectionString);
});
 
builder.Services.AddDbContextFactory<AbsanteeContext>(options =>
{
    options.UseNpgsql(DBConnectionString);
}, ServiceLifetime.Scoped);
 
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
<<<<<<< HEAD
builder.Services.AddSingleton<IRabbitMQConsumerController, ColaboratorConsumer>();
 
=======
builder.Services.AddSingleton<IColaboratorConsumer, ColaboratorConsumer>();


>>>>>>> main
var app = builder.Build();
 
// var rabbitMQConsumerServices = app.Services.GetServices<ColaboratorConsumer>();
// foreach (var service in rabbitMQConsumerServices)
// {
//     service.ConfigQueue(queueName);
//     service.StartConsuming();
// }
 
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }
 
app.UseHttpsRedirection();
 
app.UseAuthorization();
 
app.UseCors("AllowAllOrigins");
 
app.MapControllers();
 
var rabbitMQConsumerService = app.Services.GetRequiredService<IRabbitMQConsumerController>();
rabbitMQConsumerService.ConfigQueue(queueName);
rabbitMQConsumerService.StartConsuming();
 
app.Run();
 
<<<<<<< HEAD
=======

app.Run();

>>>>>>> main
static int GetPortForQueue(string queueName)
{
    int basePort = 5010;
    int queueIndex = int.Parse(queueName.Substring(1));
    return basePort + queueIndex;
}
 
public partial class Program { }