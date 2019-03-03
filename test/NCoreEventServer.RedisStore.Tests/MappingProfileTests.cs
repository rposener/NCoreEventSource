using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace NCoreEventServer.RedisStore.Tests
{
    [TestClass]
    public class MappingProfileTests
    {

        [TestMethod]
        public void Assert_MappingProfile_Isvalid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}
