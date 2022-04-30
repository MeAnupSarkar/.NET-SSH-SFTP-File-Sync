

using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Core.Repositories;
using MediaFon.FileManager.Core.UnitOfWork.Services;
using MediaFon.FileManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IFilesRepository, FilesRepository>();
builder.Services.AddTransient<IUnitOfWork, FilesService>();

builder.Services.AddInfrastructure(builder.Configuration);

 

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
