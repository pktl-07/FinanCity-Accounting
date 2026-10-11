namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Http;
    using FC.Core;

    [RoutePrefix("Photo")]
    public class PhotoController : BaseApiController
    {
        #region Methods

        [HttpPost]
        [Route("upload")]
        public IHttpActionResult UploadImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var uploadId = httpRequest.Headers["uploadId"];
                var f = httpRequest.Files[0];
                var fileByteArray = new byte[f.ContentLength];
                var path = GetFilePath(uploadId);

                if (f != null && f.ContentLength > 0)
                {
                    f.InputStream.Read(fileByteArray, 0, f.ContentLength);
                    File.Delete(path);
                    f.InputStream.Flush();
                    f.SaveAs(path);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete/{uploadId}")]
        public IHttpActionResult DeleteImage(string uploadId)
        {
            try
            {
                var path = GetFilePath(uploadId);

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
        [Route("get/{uploadId}")]
        public IHttpActionResult GetImage(string uploadId)
        {
            try
            {
                var path = GetFilePath(uploadId);

                if (!File.Exists(path))
                    throw new Exception("Image not found.");

                var bytes = File.ReadAllBytes(path);

                return Ok(bytes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("has/{uploadId}")]
        public bool HasImage(string uploadId)
        {
            try
            {
                if (File.Exists(GetFilePath(uploadId)))
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

        private string GetFilePath(string id)
        {
            var request = HttpContext.Current.Request;
            var directoryPath = Path.Combine(request.MapPath($"~/App_Data/Uploads/{id}"));

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return $"{directoryPath}//origin.png";
        }

        #endregion
    }
}
