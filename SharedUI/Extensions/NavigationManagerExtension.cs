using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

namespace SharedUI.Extensions
{
    public static class NavigationManagerExtension
    {
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key]!;
        }

        public static string PageToHashUri(this NavigationManager navigationManager, string pageAssemblyName, string pageTitle = null!, string iconCSS = null!, bool isActive = true, Dictionary<string, object> pageParams = null!)
        {
            string paramsString = "";
            if (pageParams != null)
            {
                pageParams.AsParallel().ForAll(param => paramsString += param.Key + ":" + JsonSerializer.Serialize(param.Value) + ";");
                paramsString = paramsString.TrimEnd(';');
            }

            string pageString = string.Format(@"{1}{0}{2}{0}{3}{0}{4}{0}{5}", "\n", pageAssemblyName, pageTitle ?? "", iconCSS ?? "", isActive.ToString().ToLower(), paramsString);
            return Crypt(pageString);
        }

        private static string Crypt(string text)
        {
            var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        
    }
    
}