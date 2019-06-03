using System;
using System.Collections.Generic;
using System.Web;

namespace RouteMapping.Core
{
    public delegate RouteResult RouteHandler(Dictionary<string, string> routeParams);

    public enum HttpRequestMethod{
        GET,
        HEAD,
        POST,
        PUT,
        DELETE,
        CONNECT,
        OPTIONS,
        TRACE,
        PATCH
    }

    public class Route{
        public HttpRequestMethod Method {get;}
        public string Path {get;}   

        public List<string> Segments {get;}

        public RouteHandler Handler {get;}
          
        public Route(HttpRequestMethod method, string path, RouteHandler handler){
            this.Method = method;
            this.Path = path;
            this.Handler = handler;
            this.Segments = Router.ParseRoutePath(path);
        }
    }
}