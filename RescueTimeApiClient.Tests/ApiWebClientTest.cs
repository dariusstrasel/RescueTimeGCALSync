using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using RescueTimeWebApiClient;
using RescueTimeWebApiClient.Endpoints.AnalyticApi;
using RescueTimeWebApiClient.Requests;

namespace RescueTimeApiClient.Tests
{
    [TestFixture]
    public class ApiWebClientTest
    {
        private HttpClient httpClient;
        private Mock<HttpMessageHandler> handlerMock;

        [SetUp]
        public void Setup()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{}"), // Empty JSON object
                })
                .Verifiable();

            httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new System.Uri("https://www.rescuetime.com")
            };
        }

        [Test]
        public void ApiWebClient_GivenFakeGetEventRequest_ReturnsResponseWithoutException()
        {
            // Use the interface IApiRequest<dynamic> to mock the request
            var eventRequest = new Mock<IApiRequest<object>>();
            eventRequest.Setup(r => r.EndPointUri).Returns("http://www.httpbin.org/get");
            eventRequest.Setup(r => r.Method).Returns(HttpMethod.Get);
            
            var subject = new ApiWebClient(httpClient);

            Assert.DoesNotThrowAsync(() => subject.MakeRequest(eventRequest.Object));
        }

        [Test]
        public async Task ApiWebClient_GivenRealGetEventRequest_ReturnsSuccessfulResponse()
        {
            // GetEventRequest needs basic properties set to avoid internal logic errors if any
            var eventRequest = new GetEventRequest(); 
            var subject = new ApiWebClient(httpClient);

            var result = await subject.MakeRequest(eventRequest);
            
            Assert.That(result, Is.Not.Null);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}