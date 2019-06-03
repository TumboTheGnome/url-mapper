using System;
using System.Net;

namespace RouteMapping.Core
{
    public struct RouteResult{
        public HttpStatusCode HttpStatusCode{get;}
        public string Body{get;}

        public RouteResult(HttpStatusCode HttpStatusCode, string Body){
            this.HttpStatusCode = HttpStatusCode;
            this.Body = Body;
        }
    }

}