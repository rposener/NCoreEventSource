using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace NCoreEvent
{
    /// <summary>
    /// Inbound Message when an Event is Raised
    /// </summary>
    public class EventMessage : IValidatableObject
    {

        /// <summary>
        /// Any Topic/Event this is Targeting
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Any Data associated with the Event
        /// </summary>
        public string EventJson { get; set; }

        /// <summary>
        /// Any Type of Object that is Updated by this event
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// The Id of Object that is Updated by this event
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Any Update for the Object
        /// </summary>
        public string ObjectUpdate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ObjectUpdate != null && !IsValidJson(ObjectUpdate))
                yield return new ValidationResult("ObjectUpdate is not a valid Json Object", new[] { nameof(ObjectUpdate) });
        }

        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}"))) //For object
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Debug.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Debug.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
