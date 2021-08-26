using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace XSpy.Utils
{

    public static class HtmlHelpers
    {

        public static bool HasPermission(this IHtmlHelper html, string role)
        {
            if (html.ViewContext.ViewBag.Roles == null)
                return false;

            var roles = (List<string>)html.ViewContext.ViewBag.Roles;
            return roles.Any(s => s == "IS_ADMIN" || s == role);
        }

        public static string IsSelected(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }
        
        public static bool IsSectionSelected(this IHtmlHelper html, string controller = null, string action = null)
        {
            
            return (string)html.ViewContext.RouteData.Values["controller"] == controller && (string)html.ViewContext.RouteData.Values["action"] != action;
        }

        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

    }
}
