using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace RescueTimeWebApiClient.Requests
{
    public class HttpRequestMessageBuilder
    {
        private HttpMethod HttpMethod;
        private HttpRequestMessage httpRequestMessage;
        private Uri RequestUri;

        public HttpRequestMessageBuilder()
        {
            httpRequestMessage = new HttpRequestMessage();
        }

        public HttpRequestMessageBuilder WithMethod(HttpMethod httpMethod)
        {
            HttpMethod = httpMethod;
            return this;
        }

        public HttpRequestMessageBuilder WithRequestUri(Uri requestUri)
        {
            RequestUri = requestUri;
            return this;
        }

        public HttpRequestMessageBuilder WithPayload(object payload)
        {
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            return this;
        }

        public HttpRequestMessageBuilder WithQueryStringParameters(object payload)
        {
            if (payload == null)
            {
                return this;
            }

            var payloadAsJson = JsonConvert.SerializeObject(payload);
            var payloadAsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(payloadAsJson);

            if (payloadAsDictionary != null && payloadAsDictionary.Count > 0)
            {
                var uriWithQueryParameters = QueryHelpers.AddQueryString(RequestUri.ToString(), payloadAsDictionary);
                RequestUri = new Uri(uriWithQueryParameters);
            }

            return this;
        }
        
        public HttpRequestMessage Build()
        {
            return httpRequestMessage;
        }
    }
}