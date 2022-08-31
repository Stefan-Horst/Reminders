﻿using System;
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
        public void StandardizeTimespan_StandardBehaviour()
        {
            string se = "10minutes";
            string se1 = "10hours";
            string se2 = "10days";
            string se3 = "10weeks";
            string se4 = "10months";
            string se5 = "10years";
            string se6 = "1minute";
            string se7 = "0";
            int ie = 10;
            int ie1 = 1;
            int ie2 = 0;
            string se0 = "minutes";
            string se01 = "hours";
            string se02 = "days";
            string se03 = "weeks";
            string se04 = "months";
            string se05 = "years";
            string se06 = "minute";
            string se07 = "";
            string sa = cv.StandardizeTimespan("10min", out int ia, out string sa0);
            string sa1 = cv.StandardizeTimespan("10h", out int ia1, out string sa01);
            string sa2 = cv.StandardizeTimespan("10d", out int ia2, out string sa02);
            string sa3 = cv.StandardizeTimespan("10w", out int ia3, out string sa03);
            string sa4 = cv.StandardizeTimespan("10m", out int ia4, out string sa04);
            string sa5 = cv.StandardizeTimespan("10y", out int ia5, out string sa05);
            string sa6 = cv.StandardizeTimespan("10mins", out int ia6, out string sa06);
            string sa7 = cv.StandardizeTimespan("1min", out int ia7, out string sa07);
            string sa8 = cv.StandardizeTimespan("0min", out int ia8, out string sa08);

            Assert.AreEqual(se, sa);
            Assert.AreEqual(se1, sa1);
            Assert.AreEqual(se2, sa2);
            Assert.AreEqual(se3, sa3);
            Assert.AreEqual(se4, sa4);
            Assert.AreEqual(se5, sa5);
            Assert.AreEqual(se, sa6);
            Assert.AreEqual(se6, sa7);
            Assert.AreEqual(se7, sa8);

            Assert.AreEqual(ie, ia);
            Assert.AreEqual(ie, ia1);
            Assert.AreEqual(ie, ia2);
            Assert.AreEqual(ie, ia3);
            Assert.AreEqual(ie, ia4);
            Assert.AreEqual(ie, ia5);
            Assert.AreEqual(ie, ia6);
            Assert.AreEqual(ie1, ia7);
            Assert.AreEqual(ie2, ia8);

            Assert.AreEqual(se0, sa0);
            Assert.AreEqual(se01, sa01);
            Assert.AreEqual(se02, sa02);
            Assert.AreEqual(se03, sa03);
            Assert.AreEqual(se04, sa04);
            Assert.AreEqual(se05, sa05);
            Assert.AreEqual(se0, sa06);
            Assert.AreEqual(se06, sa07);
            Assert.AreEqual(se07, sa08);
        }
        
        [TestMethod]
        public void ConvertToMinutes_StandardBehaviour()
        {
            int ie = 2;
            int ie1 = 2*60;
            int ie2 = 2*60*24;
            int ie3 = 2*60*24*7;
            int ie4 = 2*60*24*30;
            int ie5 = 2*60*24*365;
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