using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using RouteMapping.Core;
using System.Collections.Generic;
using System.Net;

namespace route_mapping.core.tests
{
    [TestClass]
    public class RouterTests
    {

        public const string Domain = "http://www.test.com";

        [DataTestMethod]
        [DataRow("/api/test/1/2", "/api/test/@domain/@value", "12")]
        [DataRow("/api/test/name","/api/@domain/@value","testname")]
        public void RenderRouteTests(string route, string routeTemplate, string value){

            Router router = new Router();
            router.AddRoute(new Route(HttpRequestMethod.GET, routeTemplate, (Dictionary<string, string> values)=>{
                return new RouteResult(HttpStatusCode.Accepted, values["domain"]+values["value"]);
            }));

            Assert.AreEqual(value, router.Eval(HttpRequestMethod.GET, route).Body);
        }



        [DataTestMethod]
        [DataRow("/api/test/something", "/api/something", false)]
        [DataRow("/api/test/something", "/api/test/something", true)]
        [DataRow("/api/test/something", "/api/test/another", false)]
        [DataRow("/api/test/1","/api/test/#id", false)] //Fix
        public void RouteMatchesCorrectly(string path, string routePath, bool result){
            Assert.AreEqual(result, Router.MatchRoute(HttpRequestMethod.GET, Router.ParseRoutePath(path), new List<Route>(){
                new Route(HttpRequestMethod.GET, routePath, null)
            }, '@') != null);
        }

        [DataTestMethod]
        [DataRow("/api/test/1","/api/test/@id", "id,1")]
        [DataRow("/api/test/1/2","/api/test/@id/@sub", "id,1,sub,2")]
        [DataRow("/api/test/1","/api/test/id", "")]
        public void ExtractRouteValues(string path, string routePath, string values){
            Dictionary<string, string> resultValues = Router.ExtractRouteValues(Router.ParseRoutePath(path), Router.ParseRoutePath(routePath));
            Dictionary<string, string> passedValues = ToDictonary(values);


            Assert.AreEqual(passedValues.Count, resultValues.Count);

            if(passedValues.Count > 0){
                foreach(var entry in passedValues)
                {
                    Assert.AreEqual(resultValues[entry.Key], entry.Value);
                }
            }
        }

        [DataTestMethod]
        [DataRow("t,1,e,3", 2)]
        [DataRow("t,1", 1)]
        [DataRow("", 0)]
        public void ToDictonaryTest(string values, int count){
          Assert.AreEqual(ToDictonary(values).Count, count);
        }

        private Dictionary<string, string> ToDictonary(string values){
            string[] splitVals = new string[]{};
            
            if(values.Trim() != String.Empty)
            {
                splitVals = values.Split(',');
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            for(int i = 0; i < splitVals.Length; i+=2){
            result.Add(splitVals[i], splitVals[i+1]);
            }

            return result;
        }
    }
}
