using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NCoreEventServer.Services.Results
{
    public class DeliveryResult
    {
        public bool SentSuccessfully { get; }

        public string Message { get; }

        public int StatusCode { get; }

        private DeliveryResult(bool success, int statusCode, string message)
        {
            SentSuccessfully = success;
            StatusCode = statusCode;
            Message = message;
        }

        public static DeliveryResult Success(HttpStatusCode StatusCode, string Message)
        {
            return new DeliveryResult(true, (int)StatusCode, Message);
        }

        public static DeliveryResult Failure(HttpStatusCode StatusCode, string Message)
        {
            return new DeliveryResult(false, (int)StatusCode, Message);
        }
    }
}
