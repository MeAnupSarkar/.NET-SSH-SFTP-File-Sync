using Hangfire;
using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Core.Repositories;
using MediaFon.FileManager.Core.Services;
using MediaFon.FileManager.Core.UnitOfWork.Services;
using MediaFon.FileManager.Infrastructure;
using Hangfire.PostgreSql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IFilesRepository, FilesRepository>();
builder.Services.AddTransient<IDirectoryRepository, DirectoryRepository>();
builder.Services.AddTransient<IEventLogsRepository, EventLogsRepository>();
builder.Services.AddTransient<IFilesInfoServiceUnitOfWork, FilesInfoDbService>();

// Initial plan was implementing [In Memory Cache] for better performance.
// But at last quitted the idea because because I got very exhausted by doing continious coding.
//builder.Services.AddTransient<ICacheService, InMemoryCacheService>();


// Adding [MediaFon.FileManager.Infrastructure] project where EF Core related dependencies defined
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHangfire(c => c
                .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("PostgreSqlContext")));

builder.Services.AddHangfireServer();


// Serilog Config
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.EnableAnnotations());

var app = builder.Build();


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



using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var filesInfoDbService = services.GetRequiredService<IFilesInfoServiceUnitOfWork>();

   
    var intervalInMinute = String.IsNullOrEmpty(builder.Configuration["BackgroundJobInterval"]) ? 1 : Convert.ToInt32(builder.Configuration["BackgroundJobInterval"]);

    var ssh = new SSHService(filesInfoDbService, builder.Configuration);


    // Initiating Hangfire Recurring Background Job in every 1 minues interval
    RecurringJob.AddOrUpdate("File Synchronizations from SFTP Server",
                          () => ssh.InitRemoteSFTPSyncWithLocal(),
                          Cron.MinuteInterval(intervalInMinute)
                        );

}

app.Run();
