# Remote SSH/SFTP 
#####  Features

- Every 1 minute service connects to sftp and checks if there are new or modified files.
- Service downloads and syncs all the new files to local path.
- If remote file modified,  same local file also gets updated.
- All file and directory info is stored in Datatabse (postgresql).
- EF core code first migration used.
- Project developed using Service oriented architecture (Repository pattern, Unit of work). 
- Solution  is divided with foud sub projects, such as Web, Core, Domain & Infrasruction.
- For background job , Hangfire is used
- For Logging, Serilog is used.
- SSH server is configurable from AppSettings.json

### Database Migration
For database context and migration , refer to project MediaFon.FileManager.Infrastructure.

` cd  MediaFon.FileManager.Infrastructure`

#### Apply Migration 
` dotnet ef database update`

### App Settings
Change SFTP Server configuration from appsetting.json
```
{
  "SSH_Settings": {
    "SFTP_SERVER": "192.168.0.109",
    "SFTP_USER": "root",
    "SFTP_PASS": "Password",
    "SFTP_PORT": 22,
    "UserDefinedRemoteWorkingDirectory": "/C:/SFTP", 
	// Starting forward slash before 'C:/' is required
    "BackgroundJobInterval": 1,
    "LocalFileStoreLocation": "LocalFileStorage" 
	// Folder should be within the Web Application directory
  }
}
```

### Start Application
MediaFon.FileManager.Web is the start up project to run the application.

### Api Enpoints URL 
#### https://localhost:7193/swagger/index.html

### Background Jobs Realtime Dashboard
#### https://localhost:7193/hangfire

