using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models.Responses
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
