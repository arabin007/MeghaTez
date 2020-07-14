﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Application.Errors
{
    class RestException: Exception
    {
        public HttpStatusCode Code { get;}
        public object Errors { get;}

        public RestException(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }
    }
}
