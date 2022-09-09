using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Reminders.ConsoleIO;
using SimultaneousConsoleIO;

namespace Reminders.UnitTests
{
    [TestClass]
    public class CommandExecutorTests
    {
        private const string TEST_DATE = "201220400101";
        
        private CommandExecutor ce;
        private ReminderManager rm;

        [TestInitialize]
        public void Initialize()
        {
            //OutputTextWriter will not be used, but is mandatory arg
            OutputTextWriter otw = new OutputTextWriter(new SimulConsoleIO(new OutputWriter()));
            rm = new ReminderManager(otw);
            ce = new CommandExecutor(otw, rm);
            
            // clear filemanager for clean test environment and create new file for tests
            File.WriteAllText(Path.Combine(rm.FileMgr.DataPath, "dataTest.rmdr"), "", Encoding.Unicode);
            rm.FileMgr.Filename = "dataTest.rmdr";
            rm.Reminders = new List<Reminder>();
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            // remove file created for tests
            File.Delete(Path.Combine(rm.FileMgr.DataPath, "dataTest.rmdr"));
        }

        [TestMethod]
        public void CmdCreate_4Param_StandardBehaviour() // all params
        {
            ce.Execute("create 20.12.2022 23:59 1w text"); // content length 1
            ce.Execute("c 201240 1.00 1d now comes the text"); // content length >1
            
            string se = "20.12.2022 23:59 1week text";
            string se1 = "20.12.2040 01:00 1day now comes the text";
            string sa = rm.Reminders[0].ToString()[3..]; // remove id from beginning
            string sa1 = rm.Reminders[1].ToString()[3..];

            Assert.AreEqual(se, sa);
            Assert.AreEqual(se1, sa1);
        }

        [TestMethod]
        public void CmdCreate_3aParam_StandardBehaviour() // time param
        {
            ce.Execute("create 20.12.2022 23:59 text"); // content length 1
            ce.Execute("c 201240 1.00 now comes the text"); // content length >1
            
            string se = "20.12.2022 23:59 0 text";
            string se1 = "20.12.2040 01:00 0 now comes the text";
            string sa = rm.Reminders[0].ToString()[3..]; // remove id from beginning
            string sa1 = rm.Reminders[1].ToString()[3..];

            Assert.AreEqual(se, sa);
            Assert.AreEqual(se1, sa1);
        }

        [TestMethod]
        public void CmdCreate_3bParam_StandardBehaviour() // repeat param
        {
            ce.Execute("create 20.12.2022 1w text"); // content length 1
            ce.Execute("c 201240 1d now comes the text"); // content length >1
            
            string se = "20.12.2022 00:00 1week text";
            string se1 = "20.12.2040 00:00 1day now comes the text";
            string sa = rm.Reminders[0].ToString()[3..]; // remove id from beginning
            string sa1 = rm.Reminders[1].ToString()[3..];

            Assert.AreEqual(se, sa);
            Assert.AreEqual(se1, sa1);
        }

        [TestMethod]
        public void CmdCreate_2Param_StandardBehaviour()
        {
            ce.Execute("create 20.12.2022 text"); // content length 1
            ce.Execute("c 201240 now comes the text"); // content length >1
            
            string se = "20.12.2022 00:00 0 text";
            string se1 = "20.12.2040 00:00 0 now comes the text";
            string sa = rm.Reminders[0].ToString()[3..]; // remove id from beginning
            string sa1 = rm.Reminders[1].ToString()[3..];

            Assert.AreEqual(se, sa);
            Assert.AreEqual(se1, sa1);
        }

        [TestMethod]
        public void CmdDelete_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("delete " + id);

            int ie = 0;
            int ia = rm.Reminders.Count;
            
            Assert.AreEqual(ie, ia);
        }

        [TestMethod]
        public void CmdUpdate_DateParam_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("update " + id + " 01.01.2022");

            string se = "01.01.2022 01:01 0 reminder 1";
            string sa = rm.Reminders[0].ToString()[3..];
            
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void CmdUpdate_ShortDateParam_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("update " + id + " 010122");

            string se = "01.01.2022 01:01 0 reminder 1";
            string sa = rm.Reminders[0].ToString()[3..];
            
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void CmdUpdate_TimeParam_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("update " + id + " 23:59");

            string se = "20.12.2040 23:59 0 reminder 1";
            string sa = rm.Reminders[0].ToString()[3..];
            
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void CmdUpdate_ShortTimeParam_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("update " + id + " 100");

            string se = "20.12.2040 01:00 0 reminder 1";
            string sa = rm.Reminders[0].ToString()[3..];
            
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void CmdUpdate_RepeatParam_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("update " + id + " 10day");

            string se = "20.12.2040 01:01 10days reminder 1";
            string sa = rm.Reminders[0].ToString()[3..];
            
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void CmdUpdate_ShortRepeatParam_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("update " + id + " 1d");

            string se = "20.12.2040 01:01 1day reminder 1";
            string sa = rm.Reminders[0].ToString()[3..];
            
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void CmdUpdate_SingleContentParam_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("update " + id + " text");

            string se = "20.12.2040 01:01 0 text";
            string sa = rm.Reminders[0].ToString()[3..];
            
            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void CmdUpdate_ContentParam_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE, "0", "reminder 1");
            ce.Execute("update " + id + " here comes the text");

            string se = "20.12.2040 01:01 0 here comes the text";
            string sa = rm.Reminders[0].ToString()[3..];
            
            Assert.AreEqual(se, sa);
        }
    }
}
