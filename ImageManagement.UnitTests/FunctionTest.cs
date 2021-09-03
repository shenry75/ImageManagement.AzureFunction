using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web;

namespace UnitTestProject1
{
    public abstract class FunctionTest
    {

        string acctParameter = "bdsteststorage12";
        string imgContainerParameter = "testimages";
        string imgAccountKeyParameter = "bg+nNuc2X/l/WsfpoNcMoE9+TacmjPjM93Dw4ex0Hapfwej6qtdre+3R7yNPh20uZPCMfZIuyPjnlEqYBLQ3Cw==";
        //string accountName = System.Environment.GetEnvironmentVariable($"StorageConfig:{acctParameter}");
        //string imageContainer = System.Environment.GetEnvironmentVariable($"StorageConfig:{imgContainerParameter}");
        //string accountKey = System.Environment.GetEnvironmentVariable($"StorageConfig:{imgAccountKeyParameter}");


        public (HttpRequest, TraceWriter) Arrange(object content)
        {
            HttpMethod httpMethod = HttpMethod.Post;
            Uri requestUri = new Uri("http://tempuri.org");
            HttpRequest req = new HttpRequest("MyFile.jpg", "https:bdsteststorage12.blob.core.windows.net/testimages/MyFile.jpg", "");
            var tempPath = Environment.GetEnvironmentVariable("temp");
            HttpRouteCollection routeCollection = new HttpRouteCollection(tempPath);
            TraceWriter log = new VerboseDiagnosticsTraceWriter();
            return (req, log);
        }
    }
}