using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImagesDownloader.Exceptions
{
    public class JobNotFoundException : Exception
    {
        public JobNotFoundException()
        {
        }

        public JobNotFoundException(string message)
            : base(message)
        {
        }

        public JobNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}