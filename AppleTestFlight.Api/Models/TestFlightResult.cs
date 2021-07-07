﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppleTestFlight.Api.Models
{
    public class TestFlightResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }


        public TestFlightResult(int code, string message, object data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }
    }
}
