using Microsoft.VisualStudio.TestTools.UnitTesting;
using SensapexCs;

namespace SensapexCsTest
{
    [TestClass()]
    public class UmvTests
    {
        protected Umv? _UmvObj;

        [TestInitialize]
        public void TestInitialize()
        {
            _UmvObj = new Umv(30);
            Assert.IsTrue(_UmvObj.Open("169.254.255.255", 1000, 0));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (_UmvObj != null)
            {
                _UmvObj.Close();
                _UmvObj = null;
            }
        }

        [TestMethod()]
        public void SetPressureTest()
        {
            Assert.IsNotNull(_UmvObj);
            float targetPressure_kPa = 1.1f;
            Assert.IsTrue(_UmvObj.SetPressure(1, targetPressure_kPa));
            Assert.IsTrue(_UmvObj.SetPressure(8, targetPressure_kPa));
            Assert.IsFalse(_UmvObj.SetPressure(9, targetPressure_kPa)); // Max is 8
        }

        [TestMethod()]
        public void GetPressureTest()
        {
            Assert.IsNotNull(_UmvObj);

            float pressure_kPa;
            Assert.IsTrue(_UmvObj.GetPressure(1, out pressure_kPa));
            Assert.IsFalse(float.IsInfinity(pressure_kPa));
            Assert.IsTrue(_UmvObj.GetPressure(8, out pressure_kPa));
            Assert.IsFalse(float.IsInfinity(pressure_kPa));
            Assert.IsFalse(_UmvObj.GetPressure(9, out pressure_kPa)); // Max is 8
            Assert.IsTrue(float.IsInfinity(pressure_kPa));
        }

        [TestMethod()]
        public void MeasurePressureTest()
        {
            Assert.IsNotNull(_UmvObj);

            float pressure_kPa;
            Assert.IsTrue(_UmvObj.MeasurePressure(1, out pressure_kPa));
            Assert.IsTrue(pressure_kPa < 10);
            Assert.IsTrue(_UmvObj.MeasurePressure(8, out pressure_kPa));
            Assert.IsTrue(pressure_kPa < 10);
            Assert.IsFalse(_UmvObj.MeasurePressure(9, out pressure_kPa)); // Max is 8
            Assert.IsTrue(float.IsInfinity(pressure_kPa));
        }

        [TestMethod()]
        public void SetValveTest()
        {
            Assert.IsNotNull(_UmvObj);
            int targetValue = 1;
            Assert.IsTrue(_UmvObj.SetValve(1, targetValue));
            Assert.IsTrue(_UmvObj.SetValve(8, targetValue));
            Assert.IsFalse(_UmvObj.SetValve(9, targetValue)); // Max is 8
        }

        [TestMethod()]
        public void GetValveTest()
        {
            Assert.IsNotNull(_UmvObj);
            int value;
            Assert.IsTrue(_UmvObj.GetValve(1, out value));
            Assert.IsTrue(value == 0 || value == 1);
            Assert.IsTrue(_UmvObj.GetValve(8, out value));
            Assert.IsTrue(value == 0 || value == 1);
            Assert.IsFalse(_UmvObj.GetValve(9, out value)); // Max is 8
            Assert.IsTrue(value == int.MaxValue);
        }
    }
}