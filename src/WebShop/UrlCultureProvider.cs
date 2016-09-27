using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebShop
{
    public class UrlCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
           
            var url = httpContext.Request.Path;
            //Quick and dirty parsing of language from url path, which looks like "/deDE/home/Index"
            var parts = httpContext.Request.Path.Value.Split('/');
            if (parts.Length < 3)
            {
                return Task.FromResult<ProviderCultureResult>(null);
            }
            var hasCulture = Regex.IsMatch(parts[1], @"^[a-z]{2}(-[A-Z]{2})*$");
            if (!hasCulture)
            {
                return Task.FromResult<ProviderCultureResult>(null);
            }
            var culture = parts[1];
            return Task.FromResult(new ProviderCultureResult(culture));
        }
    }
}


