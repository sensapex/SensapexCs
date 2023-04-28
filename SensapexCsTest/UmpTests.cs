using Microsoft.VisualStudio.TestTools.UnitTesting;
using SensapexCs;
using static SensapexCs.Ump;

namespace SensapexCsTests
{

    [TestClass()]
    public class UmpTests
    {
        private List<UmpSpeedPos>? _speedPos;
        private Ump? _UmpObj;

        [TestInitialize]
        public void TestInitialize()
        {
            _speedPos = new List<UmpSpeedPos>();
            Assert.IsNotNull(_speedPos);

            UmpSpeedPos sp1 = new()
            {
                xPositionUm = 8000.0f,
                yPositionUm = 7000.0f,
                zPositionUm = 6000.0f,
                xSpeedUms = 1000.0f,
                ySpeedUms = 1500.0f,
                zSpeedUms = 2000.0f
            };

            UmpSpeedPos sp2 = new()
            {
                xPositionUm = 10000.0f,
                yPositionUm = 10000.0f,
                zPositionUm = 10000.0f,
                xSpeedUms = 1000.0f,
                ySpeedUms = 1500.0f,
                zSpeedUms = 2000.0f
            };

            _speedPos.Add(sp1);
            _speedPos.Add(sp2);

            _UmpObj = new Ump(1);
            Assert.IsTrue(_UmpObj.Open("169.254.255.255", 1000, 0));

        }

        [TestCleanup]
        public void TestCleanup()
        {

        }

        [TestMethod()]
        public void GetFirmwareVersionTest()
        {
            Assert.IsNotNull(_UmpObj);

            // Default (4 version elements)
            List<int> versionDefault = new();
            Assert.IsTrue(_UmpObj.GetFirmwareVersion(versionDefault));
            Assert.AreEqual(4, versionDefault.Count);
            Assert.IsTrue(versionDefault.ElementAt(0) <= 2);
            Assert.IsTrue(versionDefault.ElementAt(1) >= 17 && versionDefault.ElementAt(1) <= 40);
            Assert.IsTrue(versionDefault.ElementAt(2) >= 0 && versionDefault.ElementAt(2) <= 12);
            Assert.IsTrue(versionDefault.ElementAt(3) >= 100 && versionDefault.ElementAt(3) <= 799);

            // Variable length version (4 elements)
            List<int> versionElements_4 = new(4);
            Assert.IsTrue(_UmpObj.GetFirmwareVersion(versionElements_4));
            Assert.AreEqual(versionDefault.Count, versionElements_4.Count);
            for (int i = 0; i < versionElements_4.Count; i++)
            {
                Assert.AreEqual(versionDefault.ElementAt(i), versionDefault.ElementAt(i));
            }

            // Variable length version (1 element)
            List<int> versionElements_1 = new(1);
            Assert.IsTrue(_UmpObj.GetFirmwareVersion(versionElements_1));
            Assert.AreEqual(1, versionElements_1.Count);
            Assert.AreEqual(versionDefault.ElementAt(0), versionElements_1.ElementAt(0));
        }

        [TestMethod()]
        public void GetPositionsTest()
        {
            Assert.IsNotNull(_UmpObj);

            List<float> list = new List<float>();
            Assert.IsTrue(_UmpObj.GetPositions(list));
            Assert.AreEqual(4, list.Count);
            Assert.IsTrue(list.ElementAt(0) > 0);
            Assert.IsTrue(list.ElementAt(1) > 0);
            Assert.IsTrue(list.ElementAt(2) > 0);
            Assert.AreEqual(0, list.ElementAt(3));
        }

        [TestMethod()]
        public void GotoPositionTest()
        {
            Assert.IsNotNull(_UmpObj);
            Assert.IsNotNull(_speedPos);

            Assert.IsTrue(_UmpObj.GotoPosition(_speedPos[0]));
            Assert.IsTrue(_UmpObj.GotoPosition(_speedPos[1], false));
        }

        [TestMethod()]
        public void StopTest()
        {
            Assert.IsNotNull(_UmpObj);
            Assert.IsNotNull(_speedPos);

            Assert.IsTrue(_UmpObj.TakeStep(100000f, 0f, 0f, 0f, 100));
            Thread.Sleep(500);
            Assert.IsTrue(_UmpObj.Stop());

            Assert.IsTrue(_UmpObj.GotoPosition(_speedPos[1], false));
        }

        [TestMethod()]
        public void CalibrateLoadTest()
        {
            Assert.IsNotNull(_UmpObj);
            Assert.IsNotNull(_speedPos);

            Assert.IsTrue(_UmpObj.CalibrateLoad());
            Thread.Sleep(2000);
            Assert.IsTrue(_UmpObj.Stop());

            // Goto back to home
            Assert.IsTrue(_UmpObj.GotoPosition(_speedPos[1]));
        }

        [TestMethod()]
        public void CalibrateZeroPositionTest()
        {
            Assert.IsNotNull(_UmpObj);
            Assert.IsNotNull(_speedPos);

            Assert.IsTrue(_UmpObj.CalibrateZeroPosition());
            Thread.Sleep(2000);
            Assert.IsTrue(_UmpObj.Stop());

            // Goto back to home
            Assert.IsTrue(_UmpObj.GotoPosition(_speedPos[1]));
        }

        private void DoRunTakeStepTest(float stepLen_um, int speed_ums, StepMode clsMode)
        {
            Assert.IsNotNull(_UmpObj);

            List<TargetStep> targets = new();

            // Take a long step forward
            TargetStep step = new TargetStep();
            step.AxisId = TargetAxis.AxisX;
            step.Step_um = stepLen_um;
            step.Speed_ums = speed_ums;
            // ... x
            targets.Add(step);
            // ... y
            step.AxisId = TargetAxis.AxisY;
            targets.Add(step);
            // ... z
            step.AxisId = TargetAxis.AxisZ;
            targets.Add(step);

            Assert.IsTrue(_UmpObj.TakeStep(targets, clsMode));
            Thread.Sleep(2500);

            targets.Clear();
            
            // Backwards
            step.Step_um = -step.Step_um;
            // ... x
            step.AxisId = TargetAxis.AxisX;
            targets.Add(step);
            // ... y
            step.AxisId = TargetAxis.AxisY;
            targets.Add(step);
            // ... z
            step.AxisId = TargetAxis.AxisZ;
            targets.Add(step);

            Assert.IsTrue(_UmpObj.TakeStep(targets, clsMode));
            Thread.Sleep(2500);
        }

        [TestMethod()]
        public void TakeStepTest()
        {
            DoRunTakeStepTest(200, 200, StepMode.Automatic);
            DoRunTakeStepTest(50, 50, StepMode.CustomModeLow);
            DoRunTakeStepTest(25, 25, StepMode.CustomModeHigh);
        }
    }
}
