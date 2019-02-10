using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventClient
{
    public class NCoreObjectAttribute : Attribute
    {
        public string ObjectType { get; }

        /// <summary>
        /// The Logical Object Type this Method will Subscribe to
        /// </summary>
        /// <param name="objectType"></param>
        public NCoreObjectAttribute(string objectType)
        {
            ObjectType = objectType;
        }
    }
}
