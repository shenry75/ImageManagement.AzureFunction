using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ImageManagement.Function.UnitTests;
using System.Web;
using System.Text;

namespace ImageManagement.Function.UnitTests
{
    [TestClass]
    public class UnitTest1 
    {
        [TestMethod]
        public async Task FunctionInvalidInput()
        {
            // Arrange
            //Mock<IWebRequest> fakeRequest = new Mock<IWebRequest>();
            //var context = new Mock<FunctionContext>();
            //var request = new FakeHttpRequestData(context.Object);
            //await request.Body.WriteAsync(Encoding.ASCII.GetBytes("test"));
            //request.Body.Position = 0;

            //// Act
            //var function = new MyFunction(new NullLogger<MyFunction>());
            //var result = await function.Run(request);
            //result.HttpResponse.Body.Position = 0;

            //// Assert
            //var reader = new StreamReader(result.HttpResponse.Body);
            //var responseBody = await reader.ReadToEndAsync();
            //Assert.IsNotNull(result);
            //Assert.AreEqual(HttpStatusCode.OK, result.HttpResponse.StatusCode);
            //Assert.AreEqual("Hello test", responseBody);
        }
    }

    public interface IWebRequest
    {
        Task<string> DownloadString();
    }

    public class RealWebRequest : IWebRequest
    {
        private readonly string url;

        public RealWebRequest(string url)
        {
            this.url = url;
        }

        public Task<string> DownloadString()
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadStringTaskAsync(new Uri(url));
            }
        }
    }

}
