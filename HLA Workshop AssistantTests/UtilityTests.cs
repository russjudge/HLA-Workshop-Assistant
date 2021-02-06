using Microsoft.VisualStudio.TestTools.UnitTesting;
using HLA_Workshop_Assistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA_Workshop_Assistant.Tests
{
    [TestClass()]
    public class UtilityTests
    {
        [TestMethod()]
        public void GetSteamInstallFolderTest()
        {
            var steamFolder = HLA_Workshop_Assistant.Utility.GetSteamInstallFolder();
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSteamLibraryFoldersTest()
        {
            var folderList = HLA_Workshop_Assistant.Utility.GetSteamLibraryFolders();

            Assert.Fail();
        }
    }
}