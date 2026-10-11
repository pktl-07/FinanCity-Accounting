namespace FC.Core
{
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    public class DownloadResult : IHttpActionResult
    {
        #region Fields

        string _fileName;
        Stream _content;
        HttpRequestMessage _httpRequestMessage;
        HttpResponseMessage _httpResponseMessage;

        #endregion

        #region Methods

        public IHttpActionResult Excute(Stream content,
            HttpRequestMessage request,
            string fileName)
        {
            _content = content;
            _httpRequestMessage = request;
            _fileName = Regex.Replace(fileName, @"\s+", "");

            return this;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var mime = MimeMapping.GetMimeMapping(_fileName);

            _httpResponseMessage = _httpRequestMessage.CreateResponse(HttpStatusCode.OK);
        
            _httpResponseMessage.Content = new StreamContent(_content);
            _httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            _httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(mime);
            _httpResponseMessage.Content.Headers.ContentDisposition.FileName = _fileName;
            _httpResponseMessage.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            
            return Task.FromResult(_httpResponseMessage);
        }

        #endregion
    }
}
