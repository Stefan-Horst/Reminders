using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminders.util;

namespace Reminders.UnitTests
{
    [TestClass]
    public class ConverterFormatterTests
    {
        private ConverterFormatter cv;

        [TestInitialize]
        public void Initialize()
        {
            cv = new ConverterFormatter();
        }

        [TestMethod]
        public void ConvertToMinutes_StandardBehaviour()
        {
            int ie = 2;
            int ie1 = 2*60;
            int ie2 = 2*60*24;
            int ie3 = 2*60*24*7;
            int ie4 = 2*60*24*30;
            int ie5 = 2*60*24*30*12;
            int ia = cv.ConvertToMinutes("2min", 2);
            int ia1 = cv.ConvertToMinutes("2h", 2);
            int ia2 = cv.ConvertToMinutes("2d", 2);
            int ia3 = cv.ConvertToMinutes("2w", 2);
            int ia4 = cv.ConvertToMinutes("2m", 2);
            int ia5 = cv.ConvertToMinutes("2y", 2);
            
            Assert.AreEqual(ie, ia);
            Assert.AreEqual(ie1, ia1);
            Assert.AreEqual(ie2, ia2);
            Assert.AreEqual(ie3, ia3);
            Assert.AreEqual(ie4, ia4);
            Assert.AreEqual(ie5, ia5);
        }

        [TestMethod]
        public void FormatTime_StandardBehaviour()
        {
            string se = "day";
            string se1 = "5 days";
            string se2 = "week";
            string se3 = "3 weeks";
            string se4 = "month";
            string se5 = "3 months";
            string sa = cv.FormatTime(1);
            string sa1 = cv.FormatTime(5);
            string sa2 = cv.FormatTime(7);
            string sa3 = cv.FormatTime(21);
            string sa4 = cv.FormatTime(30);
            string sa5 = cv.FormatTime(90);

            Assert.AreEqual(se, sa);
            Assert.AreEqual(se1, sa1);
            Assert.AreEqual(se2, sa2);
            Assert.AreEqual(se3, sa3);
            Assert.AreEqual(se4, sa4);
            Assert.AreEqual(se5, sa5);
        }

        [TestMethod]
        public void ConvertStringToDate_StandardBehaviour()
        {
            DateTime de = new DateTime(2022, 12, 20);
            DateTime de1 = new DateTime(1999, 1, 1);
            DateTime da = cv.ConvertStringToDate("20122022");
            DateTime da1 = cv.ConvertStringToDate("01011999");

            Assert.AreEqual(de, da);
            Assert.AreEqual(de1, da1);
        }
        
        [TestMethod]
        public void ConvertStringToDateTime_StandardBehaviour()
        {
            DateTime de = new DateTime(2022, 12, 20, 23, 59, 0);
            DateTime de1 = new DateTime(1999, 1, 1, 1, 1, 0);
            DateTime de2 = new DateTime(2022, 12, 20, 0, 0, 0);
            DateTime da = cv.ConvertStringToDateTime("201220222359");
            DateTime da1 = cv.ConvertStringToDateTime("010119990101");
            DateTime da2 = cv.ConvertStringToDateTime("201220220000");

            Assert.AreEqual(de, da);
            Assert.AreEqual(de1, da1);
            Assert.AreEqual(de2, da2);
        }
    }
}