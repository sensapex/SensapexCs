using Microsoft.VisualStudio.TestTools.UnitTesting;
using SensapexCs;

namespace SensapexCsTests;

[TestClass]
public class UmSdkTests
{
    [TestMethod]
    public void TestLibUmInfo()
    {
        UmSdkInfo InfoObj = UmSdk.LibUmInfo();
        Assert.AreEqual("v1.400", InfoObj.UmsdkVersion);
        Assert.AreEqual("0.9.4", InfoObj.SensapexCsVersion);
    }
}
