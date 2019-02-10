using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NCoreEventServer.Services.Results
{
    public class RegistrationResult
    {
        /// <summary>
        /// Was the Injestion Successful
        /// </summary>
        public bool SuccessfulRegistration { get; }

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
        private RegistrationResult(bool success, int statusCode, string message)
        {
            SuccessfulRegistration = success;
            StatusCode = statusCode;
            Message = message;
        }

        /// <summary>
        /// Standard Success Response
        /// </summary>
        public static RegistrationResult Success { get; } = new RegistrationResult(true, (int)HttpStatusCode.Accepted, null);

        /// <summary>
        /// Constructed Failure Message
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static RegistrationResult Failure(HttpStatusCode statusCode, string message)
        {
             return new RegistrationResult(false, (int)statusCode, message); 
        }
    }
}
