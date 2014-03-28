using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SquareHook.Membership.Models
{
    public class AllowCrossJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string allow = "*";

            /*if (filterContext.RequestContext.HttpContext.Request.UrlReferrer.OriginalString.Contains("comptia.squarehook.com"))
            {
                allow = "http://saveaspot.comptia.com";
            }*/
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", allow);
            base.OnActionExecuting(filterContext);
        }
    }
}