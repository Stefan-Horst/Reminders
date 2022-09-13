using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.util;

namespace Reminders.UnitTests
{
    [TestClass]
    public class ValidatorTests
    {
        private Validator v;

        [TestInitialize]
        public void Initialize()
        {
            v = new Validator();
        }

        [TestMethod]
        public void IsDateValid_StandardBehaviour()
        {
            bool be = true;
            string se = "20122022";
            bool ba = v.IsDateValid("20.12.2022", out string sa);

            Assert.AreEqual(be, ba);
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void IsDateValid_ShortAltParam_StandardBehaviour()
        {
            bool be = true;
            string se = "20122022";
            bool ba = v.IsDateValid("20.12.22", out string sa);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void IsDateValid_Alt_StandardBehaviour()
        {
            bool be = true;
            string se = "20122022";
            bool ba = v.IsDateValid("20122022", out string sa);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void IsDateValid_ShortAlt_StandardBehaviour()
        {
            bool be = true;
            string se = "20122022";
            bool ba = v.IsDateValid("201222", out string sa);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void IsTimeValid_StandardBehaviour()
        {
            bool be = true;
            string se = "1050";
            bool ba = v.IsTimeValid("10:50", out string sa);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void IsTimeValid_AltParam_StandardBehaviour()
        {
            bool be = true;
            string se = "1050";
            bool ba = v.IsTimeValid("10.50", out string sa);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void IsTimeValid_ShortAltParam_StandardBehaviour()
        {
            bool be = true;
            string se = "0850";
            bool ba = v.IsTimeValid("8:50", out string sa);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void IsTimespanValid_StandardBehaviour()
        {
            bool be = true;
            bool ba = Validator.IsTimespanValid("10min");
            bool ba1 = Validator.IsTimespanValid("10h");
            bool ba2 = Validator.IsTimespanValid("10d");
            bool ba3 = Validator.IsTimespanValid("10w");
            bool ba4 = Validator.IsTimespanValid("10m");
            bool ba5 = Validator.IsTimespanValid("10y");
            bool ba6 = Validator.IsTimespanValid("0d");
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(be, ba1);
            Assert.AreEqual(be, ba2);
            Assert.AreEqual(be, ba3);
            Assert.AreEqual(be, ba4);
            Assert.AreEqual(be, ba5);
            Assert.AreEqual(be, ba6);
        }
        
        [TestMethod]
        public void IsTimespanValid_ShortParam_StandardBehaviour()
        {
            bool be = true;
            bool ba = Validator.IsTimespanValid("10minute");
            bool ba1 = Validator.IsTimespanValid("10hour");
            bool ba2 = Validator.IsTimespanValid("10day");
            bool ba3 = Validator.IsTimespanValid("10weeks");
            bool ba4 = Validator.IsTimespanValid("10months");
            bool ba5 = Validator.IsTimespanValid("10years");
            bool ba6 = Validator.IsTimespanValid("0week");
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(be, ba1);
            Assert.AreEqual(be, ba2);
            Assert.AreEqual(be, ba3);
            Assert.AreEqual(be, ba4);
            Assert.AreEqual(be, ba5);
            Assert.AreEqual(be, ba6);
        }
    }
}