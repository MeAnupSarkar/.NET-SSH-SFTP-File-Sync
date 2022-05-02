

using Hangfire;
using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Core.Repositories;
using MediaFon.FileManager.Core.Services;
using MediaFon.FileManager.Core.UnitOfWork.Services;
using MediaFon.FileManager.Infrastructure;
using Hangfire.PostgreSql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IFilesRepository, FilesRepository>();
builder.Services.AddTransient<IDirectoryRepository, DirectoryRepository>();
builder.Services.AddTransient<IEventLogsRepository, EventLogsRepository>();
builder.Services.AddTransient<IFilesInfoServiceUnitOfWork, FilesInfoDbService>();
builder.Services.AddTransient<ICacheService, InMemoryCacheService>();

 

builder.Services.AddHangfire(c => c
                .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("PostgreSqlContext")));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

builder.Services.AddInfrastructure(builder.Configuration);



builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.EnableAnnotations());

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

app.UseHangfireDashboard();


//var filesInfoDbService = app.Services.GetRequiredService<IFilesInfoServiceUnitOfWork>();
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var filesInfoDbService = services.GetRequiredService<IFilesInfoServiceUnitOfWork>();

    var ssh = new SSHService(filesInfoDbService,  builder.Configuration);
    //ssh.LaunchBackgrounJob();

    RecurringJob.AddOrUpdate("File Synchronizations from SFTP Server",
                          () => ssh.InitRemoteSFTPSyncWithLocal(),
                          Cron.MinuteInterval(2)
                        );

}

app.Run();
