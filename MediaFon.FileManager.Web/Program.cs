

using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Core.Repositories;
using MediaFon.FileManager.Core.UnitOfWork.Services;
using MediaFon.FileManager.Infrastructure;
 
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Host.UseContentRoot(Directory.GetCurrentDirectory());

builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IFilesRepository, FilesRepository>();
builder.Services.AddTransient<IFilesInfoServiceUnitOfWork, FilesInfoDbService>();


builder.Services.AddInfrastructure(builder.Configuration);


//builder.Host.UseSerilog
//Log.Logger = new LoggerConfiguration()
//        .ReadFrom.Configuration(builder.Configuration)
//                 .CreateLogger();

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
