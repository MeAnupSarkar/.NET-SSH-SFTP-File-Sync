using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Core.UnitOfWork.Services;
using MediaFon.FileManager.Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MediaFon.FileManager.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {        
        IFilesInfoServiceUnitOfWork filesService;
        SFTPService sftpService;
        private readonly IWebHostEnvironment _webHostEnvirnoment;
         

        public FilesController(IFilesInfoServiceUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvirnoment = webHostEnvironment;
            this.filesService = unitOfWork;
            this.sftpService = new SFTPService(this.filesService, $"{this._webHostEnvirnoment.ContentRootPath}\\LocalFileStorage");


        }


        [HttpGet]
        public IEnumerable<Domain.Entity.File> Get()
        {
            Log.Information("Fetch All " );

            return filesService.Files.GetAllFiles();
        }

      
 

        [HttpGet()]
        [Route("Disconnect")]
        public object Disconnect()
        {
            try
            {
                var result = sftpService.Disconnect();
                Log.Information("SFTP DisConnected!");
                return Ok(new { status = result , msg = result ? "Disconnected" : "Still Connected!" });  
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { msg = e.Message });

            }

        }

        [HttpGet()]
        [Route("ListDirectory")]
        public List<String>? ListDirectory()
        {
            //try
            //{
                var folders = sftpService.ListDirectory();
                return  folders ;
            //}
            //catch (Exception e)
            //{
            //    Log.Error(e, e.Message);
            //    return null;

            //}

        }


    }
}
