using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.Utilities
{
    public class AutoLogDelegateHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestBody = request.Content.ReadAsStringAsync().Result;

            return await base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var response = task.Result;

                //Log use log4net
                _LogHandle(request, requestBody, response);

                return response;
            }, cancellationToken);
        }

        protected void _LogHandle(HttpRequestMessage request, string requestBody, HttpResponseMessage response)
        {
            Console.Write(request);
        }
    }
}