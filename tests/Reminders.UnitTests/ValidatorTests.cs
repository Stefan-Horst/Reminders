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
            int se = 10;
            int se6 = 0;
            bool ba = v.IsTimespanValid("10min", out int ia);
            bool ba1 = v.IsTimespanValid("10d", out int ia1);
            bool ba2 = v.IsTimespanValid("10w", out int ia2);
            bool ba3 = v.IsTimespanValid("10m", out int ia3);
            bool ba4 = v.IsTimespanValid("10y", out int ia4);
            bool ba5 = v.IsTimespanValid("10w", out int ia5);
            bool ba6 = v.IsTimespanValid("0w", out int ia6);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(be, ba1);
            Assert.AreEqual(be, ba2);
            Assert.AreEqual(be, ba3);
            Assert.AreEqual(be, ba4);
            Assert.AreEqual(be, ba5);
            Assert.AreEqual(be, ba6);
            
            Assert.AreEqual(se, ia);
            Assert.AreEqual(se, ia1);
            Assert.AreEqual(se, ia2);
            Assert.AreEqual(se, ia3);
            Assert.AreEqual(se, ia4);
            Assert.AreEqual(se, ia5);
            Assert.AreEqual(se6, ia6);
        }
    }
}