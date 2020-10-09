using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test1Api.Models;

namespace Test1Api.Utils
{
    public static class WebHostExtensions
    {
        public static IHost SeedData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using ( var context = scope.ServiceProvider.GetService<TodoContext>())
                {
                    //seeder classes
                    new UserSeeder(context).SeedData();
                }
            }
            return host;
        }
    }
}
