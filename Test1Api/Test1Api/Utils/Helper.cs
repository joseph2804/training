using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Test1Api.Models.Responses;

namespace Test1Api.Utils
{
    public static class Helper
    {
        public static readonly string Issuer = "dangduyhiep.com";
        public static readonly string AppKey = "dangduyhiepsadsadasdsasdads";

        public static string GenHash(string input)
        {
            return string.Join("", new SHA1Managed()
                         .ComputeHash(Encoding.UTF8.GetBytes(input))
                         .Select(x => x.ToString("X2")).ToArray());
        }

        public static ClaimData getClaimData(ClaimsPrincipal principal)
        {
            ClaimData data = null;
            if ( principal.Claims.Count() > 0)
            {
                data = new ClaimData
                {
                    Id = Convert.ToInt32(principal.FindFirst(ClaimTypes.Sid).Value),
                    UserName = principal.FindFirst(ClaimTypes.Name).Value,
                    Role = principal.FindFirst(ClaimTypes.Role).Value
                };
            }
            return data;
        }

        public static string toTitleCase(this string str)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(str);
        }

        public static string GetBaseUrl( HttpRequest request)
        {
            return request.Scheme + "://" + request.Host.ToString();
        }
    }
}
