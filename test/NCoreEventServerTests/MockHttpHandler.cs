using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreEventServerTests
{
    public class MockHttpHandler : HttpMessageHandler
    {
        public HttpRequestMessage request { get; private set; }
        private Func<Uri, HttpResponseMessage> responses;

        public void SetResponse(HttpResponseMessage response)
        {
            this.responses = (_) => response;
        }

        public void SetResponses(Func<Uri, HttpResponseMessage> responses)
        {
            this.responses = responses;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            this.request = request;
            var responseTask = new TaskCompletionSource<HttpResponseMessage>();
            responseTask.SetResult(responses(request.RequestUri));
            return responseTask.Task;
        }
    }
}
