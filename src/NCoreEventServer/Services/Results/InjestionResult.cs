using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NCoreEventServer.Services.Results
{
    public class InjestionResult
    {
        /// <summary>
        /// Was the Injestion Successful
        /// </summary>
        public bool SuccessfulInjestion { get; }

        /// <summary>
        /// Status Code to Return to Caller
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Any Message to be sent in the response
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Private Constructor for InjestionResult
        /// </summary>
        /// <param name="success"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        private InjestionResult(bool success, int statusCode, string message)
        {
            SuccessfulInjestion = success;
            StatusCode = statusCode;
            Message = message;
        }

        /// <summary>
        /// Standard Success Response
        /// </summary>
        public static InjestionResult Success { get; } = new InjestionResult(true, (int)HttpStatusCode.Accepted, null);

        /// <summary>
        /// Constructed Failure Message
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static InjestionResult Failure(HttpStatusCode statusCode, string message)
        {
             return new InjestionResult(false, (int)statusCode, message); 
        }
    }
}
