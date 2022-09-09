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
            //int ie = 10;
            //int ie6 = 0;
            bool ba = Validator.IsTimespanValid("10min"/*, out int ia*/);
            bool ba1 = Validator.IsTimespanValid("10h"/*, out int ia1*/);
            bool ba2 = Validator.IsTimespanValid("10d"/*, out int ia2*/);
            bool ba3 = Validator.IsTimespanValid("10w"/*, out int ia3*/);
            bool ba4 = Validator.IsTimespanValid("10m"/*, out int ia4*/);
            bool ba5 = Validator.IsTimespanValid("10y"/*, out int ia5*/);
            bool ba6 = Validator.IsTimespanValid("0d"/*, out int ia6*/);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(be, ba1);
            Assert.AreEqual(be, ba2);
            Assert.AreEqual(be, ba3);
            Assert.AreEqual(be, ba4);
            Assert.AreEqual(be, ba5);
            Assert.AreEqual(be, ba6);
            
            /*Assert.AreEqual(ie, ia);
            Assert.AreEqual(ie, ia1);
            Assert.AreEqual(ie, ia2);
            Assert.AreEqual(ie, ia3);
            Assert.AreEqual(ie, ia4);
            Assert.AreEqual(ie, ia5);
            Assert.AreEqual(ie6, ia6);*/
        }
        
        [TestMethod]
        public void IsTimespanValid_ShortParam_StandardBehaviour()
        {
            bool be = true;
            //int ie = 10;
            //int ie6 = 0;
            bool ba = Validator.IsTimespanValid("10minute"/*, out int ia*/);
            bool ba1 = Validator.IsTimespanValid("10hour"/*, out int ia1*/);
            bool ba2 = Validator.IsTimespanValid("10day"/*, out int ia2*/);
            bool ba3 = Validator.IsTimespanValid("10weeks"/*, out int ia3*/);
            bool ba4 = Validator.IsTimespanValid("10months"/*, out int ia4*/);
            bool ba5 = Validator.IsTimespanValid("10years"/*, out int ia5*/);
            bool ba6 = Validator.IsTimespanValid("0week"/*, out int ia6*/);
            
            Assert.AreEqual(be, ba);
            Assert.AreEqual(be, ba1);
            Assert.AreEqual(be, ba2);
            Assert.AreEqual(be, ba3);
            Assert.AreEqual(be, ba4);
            Assert.AreEqual(be, ba5);
            Assert.AreEqual(be, ba6);
            
            /*Assert.AreEqual(ie, ia);
            Assert.AreEqual(ie, ia1);
            Assert.AreEqual(ie, ia2);
            Assert.AreEqual(ie, ia3);
            Assert.AreEqual(ie, ia4);
            Assert.AreEqual(ie, ia5);
            Assert.AreEqual(ie6, ia6);*/
        }
    }
}