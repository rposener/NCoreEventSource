using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NCoreEventServer.Services.Results
{
    public class InjestionResult
    {
        public bool SuccessfulInjestion { get; }

        public int StatusCode { get; }

        public string Message { get; }

        private InjestionResult(bool success, int statusCode, string message)
        {
            SuccessfulInjestion = success;
            StatusCode = statusCode;
            Message = message;
        }

        public static InjestionResult Success
        {
            get { return new InjestionResult(true, (int)HttpStatusCode.Accepted, null); }
        }

        public static InjestionResult Failure(HttpStatusCode statusCode, string message)
        {
             return new InjestionResult(false, (int)statusCode, message); 
        }
    }
}
