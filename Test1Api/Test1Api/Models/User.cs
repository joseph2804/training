using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models
{
    [Table("Users")]
    public class User
    {
        [Column("USE_ID")]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
    }
}
