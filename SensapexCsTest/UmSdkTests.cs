using Microsoft.VisualStudio.TestTools.UnitTesting;
using SensapexCs;

namespace SensapexCsTest;

[TestClass]
public class UmSdkTests
{
    [TestMethod]
    public void TestLibUmInfo()
    {
        UmSdkInfo InfoObj = UmSdk.LibUmInfo();
        Assert.AreEqual("v1.400", InfoObj.Version);
    }
}
