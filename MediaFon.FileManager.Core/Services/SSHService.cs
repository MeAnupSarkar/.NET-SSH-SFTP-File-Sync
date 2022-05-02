
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

    SftpClient client;
    IFilesInfoServiceUnitOfWork filesInfoService;


    string SFTP_SERVER = String.Empty;
    string SFTP_USER = String.Empty;
    string SFTP_PASS = String.Empty;
    int SFTP_PORT = 22;

    string localContentRootPath = String.Empty;
    string userDefinedRemoteWorkingDirectory = String.Empty;

    DateTime jobStartedAt;
    int totalDirectoryFound = 0;
    int totalFilesFound = 0;
    int totalFilesCreated = 0;
    int totalFilesModified = 0;


     

    public SSHService(IFilesInfoServiceUnitOfWork filesInfoServiceUnitOfWork, IConfiguration config)
    {
        filesInfoService = filesInfoServiceUnitOfWork;

        localContentRootPath = $"{System.IO.Directory.GetCurrentDirectory()}\\{config["SSH_Settings:LocalFileStoreLocation"]}"; ;

        this.SFTP_SERVER = config["SSH_Settings:SFTP_SERVER"].ToString();
        this.SFTP_USER = config["SSH_Settings:SFTP_USER"].ToString();
        this.SFTP_PASS = config["SSH_Settings:SFTP_PASS"].ToString();
        this.SFTP_PORT = String.IsNullOrEmpty(config["SSH_Settings:SFTP_PORT"]) ? 22 : Convert.ToInt32(config["SSH_Settings:SFTP_PORT"]);
        this.userDefinedRemoteWorkingDirectory = config["SSH_Settings:WorkingDirectory"].ToString();

        jobStartedAt = DateTime.UtcNow;
    }




    public async Task<EventLogs> InitRemoteSFTPSyncWithLocal()
    {
        Log.Information($"Remote SFTP Background Job  Started.........");

        try
        {
            using (client = new SftpClient(SFTP_SERVER, SFTP_PORT, SFTP_USER, SFTP_PASS))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();


                if (client.IsConnected)
                    Log.Information($"Connected to SFTP Host [Server : {SFTP_SERVER} , User : {SFTP_USER}]");

                List<string> directories = new();

                var defaultRemoteDirectory = client.WorkingDirectory;

                if(!client.Exists(userDefinedRemoteWorkingDirectory))
                {
                    Log.Warning($"Current Working Directory : {userDefinedRemoteWorkingDirectory} does not exists in remote server.Please change the path in Appsetting.json.");
                    userDefinedRemoteWorkingDirectory = defaultRemoteDirectory;
                }

                Log.Information($"Default Remote Directory : {defaultRemoteDirectory}");
                Log.Information($"Current Working Directory : {userDefinedRemoteWorkingDirectory}");

                await SyncFileAndDirectory(userDefinedRemoteWorkingDirectory, localContentRootPath);


                

                var eventLog = await AddEventLogsToDatabase();

                await filesInfoService.Complete();

                return eventLog;
            }

        }
        catch (Exception e)
        {
            
            Log.Fatal(e,$"Unable to connect SFTP Host [Server : {SFTP_SERVER} , User : {SFTP_USER}] due to {e.Message}");
        }


        return null;
    }


    private async Task SyncFileAndDirectory(string remotePath, string localPath)
    {

        StringBuilder currentLocalPath = new(localPath);

        foreach (var item in client.ListDirectory(remotePath).OrderByDescending(s => s.IsRegularFile))
        {
            // if SFTP item is Directory type
            if (item.IsDirectory)
            {
                var localDirectory = $"{currentLocalPath}\\{item.Name}";

                if (!System.IO.Directory.Exists(localDirectory))
                {
                    Log.Information($"SFTP Directory created to local   : {localDirectory} ");

                    System.IO.Directory.CreateDirectory(localDirectory);

                    await AddDirectoryInfoToDatabase(item, localDirectory);
                }

                await SyncFileAndDirectory(item.FullName, localDirectory);
                totalDirectoryFound++;

            }
            else if (item.IsRegularFile)
                await DownloadFile(item, currentLocalPath.ToString());

        }

    }


    private async Task DownloadFile(SftpFile file, string currentLocalPath)
    {
        try
        {
            totalFilesFound++;
            var existingModifiedFile = filesInfoService.Files.Find(s => s.RemotePath == file.FullName && s.LastWriteTime != file.LastWriteTime).SingleOrDefault();

            if (existingModifiedFile != null)
            {

                using (Stream fileStream = System.IO.File.OpenWrite($"{currentLocalPath}\\{file.Name}"))
                {
                    client.DownloadFile(file.FullName, fileStream);


                    await UpdateFileInfoToDatabase(file, existingModifiedFile);
                }
            }
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
        finally
        {
            // client.Disconnect();
        }
    }

    void UploadFile(string remotePath)
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
            Name = directory.Name,
            LocalPath = localPath,
            RemotePath = directory.FullName,
            LastAccessTime = directory.LastAccessTime,
            LastWriteTime = directory.LastWriteTime,
            LastAccessTimeUtc = directory.LastAccessTimeUtc,
            LastWriteTimeUtc = directory.LastWriteTimeUtc,
            HasAccessPermission = true,
            Size = directory.Length,
            CreatedBy = "Admin",
            CreatedAt = DateTime.UtcNow
        };

        await filesInfoService.Directories.AddAsync(directoryInfo);



    }


    private async Task AddFileInfoToDatabase(SftpFile file, string localPath)
    {
        var fileInfo = new Domain.Entity.File
        {
            Name = file.Name,
            DirectoryName = localPath,
            Extention = Path.GetExtension(file.FullName),
            LocalPath = localPath,
            RemotePath = file.FullName,
            LastAccessTime = file.LastAccessTime,
            LastWriteTime = file.LastWriteTime,
            LastAccessTimeUtc = file.LastAccessTimeUtc,
            LastWriteTimeUtc = file.LastWriteTimeUtc,
            HasAccessPermission = true,
            Size = file.Length,
            CreatedBy = "Admin"

        };

        await filesInfoService.Files.AddAsync(fileInfo);

        Log.Information($"SFTP File Downloaded and save to location  : {localPath}\\{file.Name}");
        totalFilesCreated++;
    }

    private async Task UpdateFileInfoToDatabase(SftpFile file, Domain.Entity.File existingModifiedFile)
    {

        existingModifiedFile.LastAccessTime = file.LastAccessTime;
        existingModifiedFile.LastWriteTime = file.LastWriteTime;
        existingModifiedFile.LastAccessTimeUtc = file.LastAccessTimeUtc;
        existingModifiedFile.LastWriteTimeUtc = file.LastWriteTimeUtc;
        existingModifiedFile.Size = file.Length;
        existingModifiedFile.ModifiedAt = DateTime.UtcNow;

        await filesInfoService.Files.Update(existingModifiedFile);

        Log.Information($"SFTP File {file.FullName} has modified since last sync. File updated to newer file  location : {existingModifiedFile.LocalPath}");
        totalFilesModified++;

    }

    private async Task<EventLogs> AddEventLogsToDatabase()
    {
        var eventLog = new EventLogs
        {
            EventName = "File Synchronizations from SFTP Server",
            JobName = "SFTP File Sync Job",
            JobType = "Recurring every 1 minute(s) in every day",
            JobIntervalInMinute = 1,
            JobStartedAt = jobStartedAt,
            JobEndedAt = DateTime.UtcNow,
            Result = $"DirectoryFound = {totalDirectoryFound}, FilesFound = {totalFilesFound}, FilesCreated={totalFilesCreated}, FilesModified={totalFilesModified}",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Admin"
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
