using SensapexCs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SensapexCsTest
{
    [TestClass()]
    public class UmxTests
    {
        protected Umx? _UmxObj;

        [TestInitialize]
        public void TestInitialize()
        {
            _UmxObj = new Umx(1);
            Assert.IsTrue(_UmxObj.Open("169.254.255.255", 1000, 0));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (_UmxObj != null)
            {
                _UmxObj.Close();
                _UmxObj = null;
            }
        }

        [TestMethod()]
        public void ListDevicesTest()
        {
            Assert.IsNotNull(_UmxObj);
            List<int> devIds = _UmxObj.QueryDevices(50);
            Assert.IsTrue(devIds.Count > 0);
        }
                
        [TestMethod()]
        public void IsBusyTest()
        {
            Assert.IsNotNull(_UmxObj);
            Assert.IsFalse(_UmxObj.IsBusy());
        }
        
        [TestMethod()]
        public void GetParameterTest()
        {
            Assert.IsNotNull(_UmxObj);
            int value;
            Assert.IsTrue(_UmxObj.GetParameter(3, out value)); // DeviceId
            Assert.AreEqual(_UmxObj.DevId, value);
        }

        [TestMethod()]
        public void SetParameterTest()
        {
            Assert.IsNotNull(_UmxObj);
            Assert.IsTrue(_UmxObj.SetParameter(3, _UmxObj.DevId)); // DeviceId
        }

        [TestMethod()]
        public void PingTest()
        {
            Assert.IsNotNull(_UmxObj);
            Assert.IsTrue(_UmxObj.Ping());
            Assert.IsFalse(_UmxObj.Ping(88)); // Undefined device
        }

        [TestMethod()]
        public void StopTest()
        {
            Assert.IsNotNull(_UmxObj);
            Assert.IsTrue(_UmxObj.Stop());
        }

        [TestMethod()]
        public void GetLastErrorTest()
        {
            Assert.IsNotNull(_UmxObj);
            // Undefined device
            Assert.IsFalse(_UmxObj.Ping(99));
            int result = _UmxObj.GetLastError();
            Assert.AreEqual(-3, result);
        }

        [TestMethod()]
        public void CmdOptionsTest()
        {
            Assert.IsNotNull(_UmxObj);
            // Initial
            int result = _UmxObj.CmdOptions(Smcpv1Constants.SMCP1_OPT_REQ_NOTIFY);
            Assert.AreEqual(result, Smcpv1Constants.SMCP1_OPT_REQ_NOTIFY);
            // Append
            result = _UmxObj.CmdOptions(Smcpv1Constants.SMCP1_OPT_REQ_RESP);
            Assert.AreEqual(Smcpv1Constants.SMCP1_OPT_REQ_NOTIFY | Smcpv1Constants.SMCP1_OPT_REQ_RESP, result);
            // Clear
            result = _UmxObj.CmdOptions(0);
            Assert.AreEqual(0, result);
        }
    }
}