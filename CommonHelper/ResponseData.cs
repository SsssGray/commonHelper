using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
    public class ResponseData<T>
    {
        public T data { get; set; }

        public String rspCode { get; set; }

        private String rspMsg { get; set; }
    }
}
