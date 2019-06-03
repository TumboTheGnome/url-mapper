using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RouteMapping.Core{
    public class Router{
        List<Route> _routes = new List<Route>();
        
        public char RouteParamSpecialChar {get;set;} = '@';
        public Func<string> ServerErrorHandler {get;set;} = null;
        public Func<string> NotFoundHandler {get;set;} = null;
        public Func<string> Unauthorized {get;set;} = null;
        public Func<bool> AuthenticationHandler {get;set;} = null;

        public void AddRoute(Route route){
            if(_routes.FirstOrDefault(x => x.Path == route.Path && x.Method == route.Method) != null){
                throw new Exception("Route "+route.Path+" already exists.");
            }

            _routes.Add(route);
        }

        public RouteResult Eval(HttpRequestMethod method, string url){
            
            var routePath = GetRoutePathFromUrl(url);

            Route matchedRoute = MatchRoute(method, routePath, _routes, RouteParamSpecialChar);

            if(matchedRoute != null){
                try{

                   if(AuthenticationHandler != null?AuthenticationHandler():true){
                    return matchedRoute.Handler(ExtractRouteValues(routePath, matchedRoute.Segments));
                   }
                   else{
                       return new RouteResult(HttpStatusCode.Unauthorized, Unauthorized != null? Unauthorized():"");
                   }
                }catch{
                    return new RouteResult(HttpStatusCode.InternalServerError, ServerErrorHandler != null?ServerErrorHandler():"");
                }
            }

            return new RouteResult(HttpStatusCode.NotFound, NotFoundHandler != null?NotFoundHandler():"");
        }

        public static Route MatchRoute(HttpRequestMethod method, List<string> parsedPath, List<Route> routes, char paramSpecialChar){

            return routes.FirstOrDefault(route => {
                if(parsedPath.Count == route.Segments.Count && method == route.Method){
                
                for(int i = 0; i < route.Segments.Count; i++){

                    if(route.Segments[i][0] != paramSpecialChar && route.Segments[i] != parsedPath[i]){
                        return false;
                    }
                }

                return true;
            }
            
            return false;
            });
        }

        public static Dictionary<string, string> ExtractRouteValues(List<string> path, List<string> matchedRoute){

            Dictionary<string, string> routeValues = new Dictionary<string, string>();

            for(int i = 0; i < matchedRoute.Count; i++){

                if(matchedRoute[i][0]== '@'){
                    string label = matchedRoute[i];
                    routeValues.Add(label.Substring(1, label.Length-1).Replace('/', ' ').Trim(), path[i]);
                }
            }

            return routeValues;
        }

        public static List<string> ParseRoutePath(string routePath){

            List<string> results = new List<string>();
            Uri uri = new Uri("http://test.com"+routePath);
            
            foreach(string segment in uri.Segments){
                var formatedSegment = segment.Replace("/", String.Empty).Trim();
                if(formatedSegment.Length > 0){
                    results.Add(formatedSegment);
                }
            }

            return results;
        }

        public List<string> GetRoutePathFromUrl(string url){
            List<string> results = new List<string>();
            Uri uri = new Uri(url);
            
            foreach(var segment in uri.Segments){
                var formatedSegment = segment.Replace("/", String.Empty).Trim();
                if(formatedSegment.Length > 0){
                results.Add(formatedSegment);
                }
            }

            return results;
        }
    }

}