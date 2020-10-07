using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Configuration;

namespace WebServer.Entry
{
    public class HttpServerRequest
    {
        public HttpServerRequest(HttpListenerRequest innerRequest)
        {
            _innerRequest = innerRequest;
            Form = new NameValueCollection();
        }

        private readonly HttpListenerRequest _innerRequest;

        //Gets the cookies sent with the request.
        
        public CookieCollection Cookies => _innerRequest.Cookies;
        //Gets the Uri object requested by the client.

        
        public Uri Url => _innerRequest.Url;
        // Gets the HTTP method specified by the client.
        
        public string HttpMethod => _innerRequest.HttpMethod;
        // Gets the client IP address and port number from which the request originated.
        
        public IPEndPoint RemoteEndPoint => _innerRequest.RemoteEndPoint;

        public Stream InputStram => _innerRequest.InputStream;
        
        public NameValueCollection Form { get; private set; }
    }
}