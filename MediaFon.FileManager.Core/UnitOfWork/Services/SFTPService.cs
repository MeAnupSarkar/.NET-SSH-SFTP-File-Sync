using MediaFon.FileManager.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Serilog;
using System.Text;

namespace MediaFon.FileManager.Core.UnitOfWork.Services;

public class SFTPService
{
 
    SftpClient client;
    string localContentRootPath;
    string workingDirectory  ;
    string SFTP_SERVER;
    string SFTP_USER;
    string SFTP_PASS;
    int    SFTP_PORT = 22;


 

    public SFTPService(IFilesInfoServiceUnitOfWork unitOfWork, string contentRootPath)
    {
        localContentRootPath = contentRootPath;
        workingDirectory = @"/C:/SFTP";
        SFTP_SERVER = "192.168.0.109";
        SFTP_USER = "anup";
        SFTP_PASS = "Virtual07";
    }

 

    public bool CheckIfConnected() =>  client.IsConnected;

    public bool Disconnect()
    {
        if (client.IsConnected)
            client.Disconnect();

        return client.IsConnected;
    }

    public  List<string>?  ListDirectory()
    {

        try
        {
            using (client = new SftpClient(SFTP_SERVER, SFTP_PORT, SFTP_USER, SFTP_PASS))
            {
                client.KeepAliveInterval = TimeSpan.FromSeconds(60);
                client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
                client.OperationTimeout = TimeSpan.FromMinutes(180);
                client.Connect();
                bool connected = client.IsConnected;

                if (client.IsConnected)
                    Log.Information($"Connected to SFTP Server : {SFTP_SERVER} , User : {SFTP_USER}");

                List<string> directories = new();

                var defaultDirectory = client.WorkingDirectory;


                Log.Information($"Default Directory : {defaultDirectory}");
                Log.Information($"Working Directory : {workingDirectory}");

                SyncFileAndDirectory(workingDirectory,localContentRootPath);

                return directories;
            }

        }
        catch(Exception e)
        {
            Log.Fatal(e,$"Unable to Connect  SFTP Server : {SFTP_SERVER} , User : {SFTP_USER}");
        }


        return null;
    }

    private void SyncFileAndDirectory(  string remotePath, string localPath)
    {
         
        StringBuilder currentLocalPath  = new(localPath);

        foreach (var item in client.ListDirectory(remotePath).OrderByDescending(s=>s.IsRegularFile))
        {
            // if SFTP item is Directory type
            if (item.IsDirectory)
            {
                var localDirectory = $"{currentLocalPath.ToString()}\\{item.Name}";
                if (!Directory.Exists(localDirectory))
                {
                    Log.Information($"SFTP Directory created to local   : {localDirectory} ");
                    Directory.CreateDirectory(localDirectory);

                    AddDirectoryInfoToDatabase(item, localDirectory);
                }

                SyncFileAndDirectory(item.FullName, localDirectory);

            }
            else if (item.IsRegularFile)
            {
                DownloadFile(item.FullName, item.Name, currentLocalPath.ToString());
            }
                
        }
    }

    private void AddDirectoryInfoToDatabase( SftpFile directory, string localPath  )
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

        };

       
    }

    void UploadFile(  string remotePath )
    {
        
        try
        {
            using var s = File.OpenRead(localContentRootPath);
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

    void DownloadFile(string remotePath , string fileName,string currentLocalPath)
    {    
        try
        {            
            using (Stream fileStream = File.OpenWrite($"{currentLocalPath}\\{fileName}"))
            {
                client.DownloadFile(remotePath, fileStream);
                Log.Information($"SFTP File Downloaded and save to location  : {currentLocalPath}\\{fileName}");
            }        
        }
        catch (DirectoryNotFoundException e)
        {
            Log.Error(e,e.Message);
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


}
