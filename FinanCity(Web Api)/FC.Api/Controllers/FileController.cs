namespace FC.Api.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
   
    [RoutePrefix("File")]
    public class FileController : BaseApiController
    {
        #region Fields

        private DownloadResult _downloadResult;

        #endregion

        #region Ctor

        public FileController(DownloadResult downloadResult)
        {
            _downloadResult = downloadResult;
        }

        #endregion

        #region Methods

        [HttpPost]
        [Route("upload")]
        public IHttpActionResult Upload()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var uploadId = httpRequest.Headers["uploadId"];
                var fileType = int.Parse(httpRequest.Headers["fileType"]);

                var f = httpRequest.Files[0];
                var extension = GetFileExtension(f.FileName);
                var fileByteArray = new byte[f.ContentLength];
                var savepath = Path.Combine(GetDirectoryPath((FileType)fileType), $"{uploadId}.{extension}");
                var existPath = GetFileName((FileType)fileType, uploadId);

                if (f?.ContentLength > 0)
                {
                    f.InputStream.Read(fileByteArray, 0, f.ContentLength);
                    if (File.Exists(existPath))
                    {
                        File.Delete(existPath);
                        f.InputStream.Flush();
                    }
                    f.SaveAs(savepath);
                }

                var info = new FileInfo(savepath);
                var mime = MimeMapping.GetMimeMapping(info.Name);

                return Ok(new
                {
                    Name = $"{((FileType)fileType).ToFolderName()}{extension}",
                    Size = File.ReadAllBytes(savepath).Length,
                    Mime = mime,
                    Extension = extension
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete/{fileType}/{uploadId}")]
        public IHttpActionResult Delete(int fileType, string uploadId)
        {
            try
            {
                var path = GetFileName((FileType)fileType, uploadId);

                if (!File.Exists(path))
                    throw new Exception("File not found.");

                File.Delete(path);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get/{fileType}/{uploadId}")]
        public IHttpActionResult Get(int fileType, string uploadId)
        {
            try
            {
                var path = GetFileName((FileType)fileType, uploadId);

                if (!File.Exists(path))
                    throw new Exception("File not found.");

                var f = new FileInfo(path);
                var bytes = File.ReadAllBytes(path);

                return _downloadResult.Excute(new MemoryStream(bytes),
                    Request, $"{uploadId}{f.Extension}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("info/{fileType}/{uploadId}")]
        public IHttpActionResult Info(int fileType, string uploadId)
        {
            try
            {
                var path = GetFileName((FileType)fileType, uploadId);
                if (!File.Exists(path))
                    throw new Exception("File not found.");

                var f = new FileInfo(path);
                var mime = MimeMapping.GetMimeMapping(f.Name);
                var extension = f.Extension;
              
                return Ok(new
                {
                    Name = $"{((FileType)fileType).ToFolderName()}{extension}",
                    Size = File.ReadAllBytes(path).Length,
                    Mime = mime,
                    Extension = extension
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("url/{fileType}/{uploadId}")]
        public string GetURL(int fileType, string uploadId)
        {
            try
            {
                var path = GetFileName((FileType)fileType, uploadId);
                if (!File.Exists(path))
                    return null;

                var f = new FileInfo(path);
                var mime = MimeMapping.GetMimeMapping(f.Name);
                var bytes = File.ReadAllBytes(path);

                return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("has/{fileType}/{uploadId}")]
        public bool Has(int fileType, string uploadId)
        {
            try
            {
                var path = GetFileName((FileType)fileType, uploadId);
                if (File.Exists(path))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private string GetDirectoryPath(FileType fileType)
        {
            var request = HttpContext.Current.Request;
            var directoryPath = Path.Combine(request.MapPath($"~/App_Data/Uploads/{fileType.ToFolderName()}"));
            var dir = new DirectoryInfo(directoryPath);

            //unless directory exist then create it.
            if (!dir.Exists)
                dir.Create();

            return directoryPath;
        }

        private string GetFileName(FileType fileType, string uploadId)
        {
            var dp = GetDirectoryPath(fileType);
            var dir = new DirectoryInfo(dp);
            var f = dir.GetFiles().Where(x => x.Name.Split('.').First() == uploadId).SingleOrDefault();

            if (f == null)
                return string.Empty;

            return Path.Combine(dp, f.Name);
        }

        private string GetFileExtension(string fileName)
        {
            return fileName.Split('.').Last();
        }

        #endregion
    }   
}
