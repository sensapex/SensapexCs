namespace SensapexCs.Tests;

[TestClass]
public class UmSdkTests
{
    [TestMethod]
    public void TestLibUmInfo()
    {
        UmSdkInfo InfoObj = UmSdk.LibUmInfo();
        Assert.AreEqual("v1.400", InfoObj.UmsdkVersion);
        Assert.AreEqual("0.9.5", InfoObj.SensapexCsVersion);
    }
}
