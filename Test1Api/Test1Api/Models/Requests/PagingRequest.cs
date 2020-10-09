using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models.Requests
{
    public class PagingRequest
    {
        public int Page { get; set; } = 20;
        public int Size { get; set; } = 1;
        public string Query { get; set; } = "";
        public string Order { get; set; } = "ASC";
        public string SortBy { get; set; }
    }
}
