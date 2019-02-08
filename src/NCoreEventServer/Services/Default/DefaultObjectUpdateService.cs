using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;

namespace NCoreEventServer.Services
{
    public class DefaultObjectUpdateService : IObjectUpdateService
    {
        public Task UpdateObject(string ObjectType, string ObjectId, JsonPatchDocument patchDocument)
        {
            throw new NotImplementedException();
        }
    }
}
