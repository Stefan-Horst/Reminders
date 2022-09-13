using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Reminders.ConsoleIO;
using SimultaneousConsoleIO;

namespace Reminders.UnitTests
{
    [TestClass]
    public class ReminderManagerTests
    {
        private const string TimeFormat = @"d\.hh\:mm"; //(fractions of) seconds are not used due to their unreliability
        private const string TestDate1 = "201220100101"; //20.12.2040 01:01:00 -> future
        private const string TestDate2 = "201220200101"; //20.12.2030 01:01:00 -> future
        private const string TestDate3 = "201220300101"; //20.12.2020 01:01:00 -> past
        private const string TestDate4 = "201220400101"; //20.12.2010 01:01:00 -> past
        //add test date with DateTime.Now?

        private ReminderManager rm;

        [TestInitialize]
        public void Initialize()
        {
            rm = new ReminderManager(new OutputTextWriter(new SimulConsoleIO(new OutputWriter()))); //OutputTextWriter will not be used, but is mandatory arg
            
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
        public void CreateReminder_StandardBehaviour()
        {
            int id = rm.CreateReminder(TestDate4, "0", "reminder 1");
            
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void ReadReminder_StandardBehaviour()
        {
            int id = rm.CreateReminder(TestDate4, "0", "reminder 1");
            Reminder expected = new Reminder(TestDate4, "0", "reminder 1");
            expected.Id = id;

            Reminder r = rm.ReadReminder(id);
            string se = JsonConvert.SerializeObject(expected);
            string sa = JsonConvert.SerializeObject(r);

            Assert.AreEqual(se, sa);
        }
        
        [TestMethod]
        public void DeleteReminder_StandardBehaviour()
        {
            int id = rm.CreateReminder(TestDate4, "0", "reminder 1");
            rm.DeleteReminder(id);

            int ie = 0;
            int ia = rm.Reminders.Count;
            
            Assert.AreEqual(ie, ia);
        }
        
        [TestMethod]
        public void UpdateReminder_StandardBehaviour()
        {
            int id = rm.CreateReminder(TestDate4, "0", "reminder 1");
            Reminder expected = new Reminder(TestDate3, "1", "reminder 2");
            expected.Id = id;

            rm.UpdateReminder(id, new Reminder(TestDate3, "1", "reminder 2"));
            Reminder r = rm.ReadReminder(id);
            string se = JsonConvert.SerializeObject(expected);
            string sa = JsonConvert.SerializeObject(r);

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetRemainingTime_1Param_StandardBehaviour()
        {
            int id = rm.CreateReminder(TestDate4, "0", "reminder 1");
            TimeSpan expected = new DateTime(2040, 12, 20, 1, 1, 0).Subtract(DateTime.Now);

            TimeSpan ts = rm.GetRemainingTime(id);
            string se = expected.ToString(TimeFormat); //format without (fractions of) seconds which would never be equal
            string sa = ts.ToString(TimeFormat);

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetRemainingTime_2Param_StandardBehaviour()
        {
            int id = rm.CreateReminder(TestDate4, "0", "reminder 1");
            DateTime dt = new DateTime(2030, 10, 10, 1, 1, 0);
            string se = "3724.00:00"; //expected difference: 3724 days

            TimeSpan ts = rm.GetRemainingTime(id, dt);
            string sa = ts.ToString(TimeFormat);

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetDueReminders_StandardBehaviour()
        {
            rm.CreateReminder(TestDate1, "0", "reminder 1");
            rm.CreateReminder(TestDate2, "0", "reminder 2");
            rm.CreateReminder(TestDate3, "0", "reminder 3");
            rm.CreateReminder(TestDate4, "0", "reminder 4");
            string se = TestDate1+","+TestDate2; //due reminders: id1 and id2
            DateTime dt = new DateTime(2022, 01, 01, 01, 01, 00);

            List <Reminder> l = rm.GetDueReminders(dt);
            string sa = string.Join(",", l.Select(i => i.DateString).ToArray());

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetRemindersDueInTimespan_StandardBehaviour()
        {
            rm.CreateReminder(TestDate1, "0", "reminder 1");
            rm.CreateReminder(TestDate2, "0", "reminder 2");
            rm.CreateReminder(TestDate3, "0", "reminder 3");
            rm.CreateReminder(TestDate4, "0", "reminder 4");
            string se = TestDate2 + "," + TestDate3; //reminders in timespan: id2 and id3
            DateTime start = new DateTime(2020, 01, 01, 01, 01, 00);
            DateTime end = new DateTime(2040, 01, 01, 01, 01, 00);

            List<Reminder> l = rm.GetRemindersDueInTimespan(start, end);
            string sa = string.Join(",", l.Select(i => i.DateString).ToArray());

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetRemindersDueOnDate_StandardBehaviour()
        {
            rm.CreateReminder(TestDate1, "0", "reminder 1");
            rm.CreateReminder(TestDate2, "0", "reminder 2");
            rm.CreateReminder(TestDate3, "0", "reminder 3");
            rm.CreateReminder(TestDate4, "0", "reminder 4");
            string se = TestDate3; //reminder on date: id3
            DateTime dt = new DateTime(2030, 12, 20, 01, 01, 00);

            List<Reminder> l = rm.GetRemindersDueOnDate(dt);
            string sa = string.Join(",", l.Select(i => i.DateString).ToArray());
            
            Assert.AreEqual(se, sa);
        }
    }
}
