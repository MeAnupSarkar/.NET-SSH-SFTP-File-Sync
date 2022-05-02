using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Serilog;
using System.Text;

namespace MediaFon.FileManager.Core.Services;

public class SSHService : ISSHService
{
    private SftpClient client;
    private IFilesInfoServiceUnitOfWork filesInfoService;
    private DateTime jobStartedAt;

    private string SFTP_SERVER                       = String.Empty;
    private string SFTP_USER                         = String.Empty;
    private string SFTP_PASS                         = String.Empty;
    private int    SFTP_PORT                         = 22;

    private string localContentRootPath              = String.Empty;
    private string userDefinedRemoteWorkingDirectory = String.Empty;

    private int totalDirectoryFound                  = 0;
    private int totalFilesFound                      = 0;
    private int totalFilesCreated                    = 0;
    private int totalFilesModified                   = 0;

    public SSHService( IFilesInfoServiceUnitOfWork filesInfoServiceUnitOfWork, IConfiguration config)
    {
        filesInfoService                       = filesInfoServiceUnitOfWork;

        localContentRootPath                   = $"{System.IO.Directory.GetCurrentDirectory()}\\{config["SSH_Settings:LocalFileStoreLocation"]}"; ;

        this.SFTP_SERVER                       = config["SSH_Settings:SFTP_SERVER"].ToString();
        this.SFTP_USER                         = config["SSH_Settings:SFTP_USER"].ToString();
        this.SFTP_PASS                         = config["SSH_Settings:SFTP_PASS"].ToString();
        this.SFTP_PORT                         = String.IsNullOrEmpty(config["SSH_Settings:SFTP_PORT"]) ? 22 : Convert.ToInt32(config["SSH_Settings:SFTP_PORT"]);
        this.userDefinedRemoteWorkingDirectory = config["SSH_Settings:UserDefinedRemoteWorkingDirectory"].ToString();

        jobStartedAt                           = DateTime.UtcNow;
    }

    public async Task InitRemoteSFTPSyncWithLocal()
    {
        Log.Information($"Remote SFTP Background Job  Started.........");

        try
        {
            using (client = new SftpClient(SFTP_SERVER, SFTP_PORT, SFTP_USER, SFTP_PASS))
            {
                client.KeepAliveInterval      = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout       = TimeSpan.FromMinutes(180);
                client.Connect();

                if (client.IsConnected)
                    Log.Information($"Connected to SFTP Host [Server : {SFTP_SERVER} , User : {SFTP_USER}]");

                List<string> directories = new();

                var defaultRemoteDirectory = client.WorkingDirectory;

                if (!client.Exists(userDefinedRemoteWorkingDirectory))
                {
                    Log.Warning($"Current Working Directory : {userDefinedRemoteWorkingDirectory} does not exists in remote server.Please change the path in Appsetting.json.");
                    userDefinedRemoteWorkingDirectory = defaultRemoteDirectory;
                }

                Log.Information($"Default Remote Directory : {defaultRemoteDirectory}");
                Log.Information($"Current Working Directory : {userDefinedRemoteWorkingDirectory}");


                // This  method recursively enter each and every direcory of remote SFTP Server and download every files.
                // Finally it saves all the files by creating corresponding directory
                await SyncFileAndDirectory(userDefinedRemoteWorkingDirectory, localContentRootPath);

                var eventLog = await AddEventLogsToDatabase();

                await filesInfoService.Complete();

                Log.Information($"Background Job [{eventLog.EventName}] completion  result [{eventLog.Result}].");

            }
        }
        catch (Exception e)
        {
            Log.Fatal(e, $"Unable to connect SFTP Host [Server : {SFTP_SERVER} , User : {SFTP_USER}] due to {e.Message}");
        }

        
    }

    private async Task SyncFileAndDirectory(string remotePath, string localPath)
    {
        StringBuilder currentLocalPath = new(localPath);

        foreach (var item in client.ListDirectory(remotePath).OrderByDescending(s => s.IsRegularFile))
        {
            // If SFTP item is Directory type
            if (item.IsDirectory)
            {
                var localDirectory = $"{currentLocalPath}\\{item.Name}";

                // If directory does not exists in local, then create new folder
                if (!System.IO.Directory.Exists(localDirectory))
                {
                    Log.Information($"SFTP Directory created to local   : {localDirectory} ");

                    System.IO.Directory.CreateDirectory(localDirectory);

                    await AddDirectoryInfoToDatabase(item, localDirectory);
                }

                // Recursively method call to download  direcories and files from remote SFTP Server.
                await SyncFileAndDirectory(item.FullName, localDirectory);

                totalDirectoryFound++;
            }
            // If SFTP item is File type
            else if (item.IsRegularFile)
                await DownloadFile(item, currentLocalPath.ToString());
        }
    }

    private async Task DownloadFile(SftpFile file, string currentLocalPath)
    {
        try
        {
            totalFilesFound++;

            // Checking if the file info is exists in local database or not. If file info exists, further check it is modified or not by checking LastWriteTime.
            var existingModifiedFile = filesInfoService.Files.Find(s => s.RemotePath == file.FullName && s.LastWriteTime != file.LastWriteTime).SingleOrDefault();

            if (existingModifiedFile != null)
            {
                using (Stream fileStream = System.IO.File.OpenWrite($"{currentLocalPath}\\{file.Name}"))
                {
                    client.DownloadFile(file.FullName, fileStream);

                    await UpdateFileInfoToDatabase(file, existingModifiedFile);
                }
            }
            // If file does not exists, create new file in local path and insert file info to database.
            else if (!filesInfoService.Files.Any(s => s.RemotePath == file.FullName))
            {
                using (Stream fileStream = System.IO.File.Create($"{currentLocalPath}\\{file.Name}"))
                {
                    client.DownloadFile(file.FullName, fileStream);

                    await AddFileInfoToDatabase(file, currentLocalPath);
                }
            }
        }
        catch (DirectoryNotFoundException e)
        {
            Log.Error(e, e.Message);
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
        }       
    }

    private void UploadFile(string remotePath)
    {
        try
        {
            using var s = System.IO.File.OpenRead(localContentRootPath);
            client.UploadFile(s, remotePath);
        }
        catch (Exception exception)
        {
            // Log error
            throw;
        }
        finally
        {
            client.Disconnect();
        }
    }

    private async Task AddDirectoryInfoToDatabase(SftpFile directory, string localPath)
    {
        var directoryInfo = new Domain.Entity.Directory
        {
            Name                = directory.Name,
            LocalPath           = localPath,
            RemotePath          = directory.FullName,
            LastAccessTime      = directory.LastAccessTime,
            LastWriteTime       = directory.LastWriteTime,
            LastAccessTimeUtc   = directory.LastAccessTimeUtc,
            LastWriteTimeUtc    = directory.LastWriteTimeUtc,
            HasAccessPermission = true,
            Size                = directory.Length,
            CreatedBy           = "Admin",
            CreatedAt           = DateTime.UtcNow
        };

        await filesInfoService.Directories.AddAsync(directoryInfo);
    }

    private async Task AddFileInfoToDatabase(SftpFile file, string localPath)
    {
        var fileInfo = new Domain.Entity.File
        {
            Name                = file.Name,
            DirectoryName       = localPath,
            Extention           = Path.GetExtension(file.FullName),
            LocalPath           = localPath,
            RemotePath          = file.FullName,
            LastAccessTime      = file.LastAccessTime,
            LastWriteTime       = file.LastWriteTime,
            LastAccessTimeUtc   = file.LastAccessTimeUtc,
            LastWriteTimeUtc    = file.LastWriteTimeUtc,
            HasAccessPermission = true,
            Size                = file.Length,
            CreatedBy           = "Admin"
        };

        await filesInfoService.Files.AddAsync(fileInfo);

        Log.Information($"SFTP File Downloaded and Saved to location  : {localPath}\\{file.Name}");
        totalFilesCreated++;
    }

    private async Task UpdateFileInfoToDatabase(SftpFile file, Domain.Entity.File existingModifiedFile)
    {
        existingModifiedFile.LastAccessTime    = file.LastAccessTime;
        existingModifiedFile.LastWriteTime     = file.LastWriteTime;
        existingModifiedFile.LastAccessTimeUtc = file.LastAccessTimeUtc;
        existingModifiedFile.LastWriteTimeUtc  = file.LastWriteTimeUtc;
        existingModifiedFile.Size              = file.Length;
        existingModifiedFile.ModifiedAt        = DateTime.UtcNow;

        await filesInfoService.Files.Update(existingModifiedFile);

        Log.Information($"SFTP File {file.FullName} has modified since last sync. File updated to newer file  location : {existingModifiedFile.LocalPath}");
        totalFilesModified++;
    }

    private async Task<EventLogs> AddEventLogsToDatabase()
    {
        var eventLog = new EventLogs
        {
            EventName           = "File Synchronizations from SFTP Server",
            JobName             = "SFTP File Sync Job",
            JobType             = "Recurring every 1 minute(s) in every day",
            JobIntervalInMinute = 1,
            JobStartedAt        = jobStartedAt,
            JobEndedAt          = DateTime.UtcNow,
            Result              = $"DirectoryFound = {totalDirectoryFound}, FilesFound = {totalFilesFound}, FilesCreated={totalFilesCreated}, FilesModified={totalFilesModified}",
            CreatedAt           = DateTime.UtcNow,
            CreatedBy           = "Admin"
        };

        await filesInfoService.EventLogs.AddAsync(eventLog);

        return eventLog;
    }

    public bool CheckIfConnected() => client.IsConnected;

    public bool Disconnect()
    {
        if (client.IsConnected)
            client.Disconnect();

        return client.IsConnected;
    }
}