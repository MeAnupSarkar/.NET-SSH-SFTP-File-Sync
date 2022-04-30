using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Core.UnitOfWork.Services;
using MediaFon.FileManager.Domain.Entity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MediaFon.FileManager.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {        
        IUnitOfWork filesService;

        public FilesController(IUnitOfWork unitOfWork)
        {
            this.filesService = unitOfWork;

        }

        // GET: api/<FilesController>
        [HttpGet]
        public IEnumerable<FileMetaData> Get()
        {
            return filesService.Files.GetAllFiles();
        }

        // GET api/<FilesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FilesController>
        [HttpPost]
        public async Task<object> Post( )
        {
            var FileMetaData = new FileMetaData
            {
                Name = "test",
                Extention = "test",
                Location = "test",
                MimeType = "test",
                Size = 111,
                CreatedBy = "Admin"
            };

            filesService.Files.Add(FileMetaData);

            var status = await filesService.Complete();

            return Ok("Insert Status :  " + status);
        }

        // PUT api/<FilesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FilesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
