using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using NCoreEventServer.SqlStore.MapProfiles;

namespace NCoreEventServer.SqlStore.Tests.MapProfiles
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
