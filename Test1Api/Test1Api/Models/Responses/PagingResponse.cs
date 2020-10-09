using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test1Api.Models.Responses
{
    public class PagingResponse : BaseResponse
    {
        public PagingInfo PagingInfo { get; set; }
    }
}
