using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api
{
    public class CustomJwtBearerEvents: JwtBearerEvents
    {
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            string errorMessage = "";
            if (context.Exception is SecurityTokenException)
                errorMessage = "Token expired";
            else if (context.Exception is SecurityTokenInvalidIssuerException)
                errorMessage = "Invalid issuer";
            else
                errorMessage = "Invalid token";

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var resp = new { ErrorCode = 401, message = errorMessage };
            var options = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            context.Response.WriteAsync(JsonConvert.SerializeObject(resp, options)).Wait();
            return Task.CompletedTask;
        }

        public override Task Challenge(JwtBearerChallengeContext context)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var resp = new
                {
                    ErrorCode = 401,
                    message = "Missing Token"
                };
                var options = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                context.Response.WriteAsync(JsonConvert.SerializeObject(resp, options)).Wait();
            }
            return Task.CompletedTask;
        }

        public override Task Forbidden(ForbiddenContext context)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var resp = new
            {
                ErrorCode = 403,
                message = "Forbidden"
            };
            var options = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            context.Response.WriteAsync(JsonConvert.SerializeObject(resp, options)).Wait();
            return base.Forbidden(context);
        }

        //private string CreateUnAuthorize()
        //{

        //}
    }
    
}
