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
        const string TIME_FORMAT = @"d\.hh\:mm"; //(fractions of) seconds are not used due to their unreliability
        const string TEST_DATE1 = "201220100101"; //20.12.2040 01:01:00 -> future
        const string TEST_DATE2 = "201220200101"; //20.12.2030 01:01:00 -> future
        const string TEST_DATE3 = "201220300101"; //20.12.2020 01:01:00 -> past
        const string TEST_DATE4 = "201220400101"; //20.12.2010 01:01:00 -> past
        //add test date with DateTime.Now?

        ReminderManager rm;

        [TestInitialize]
        public void Initialize()
        {
            //clear file for clean test environment
            //D:\Stefan\Programmieren\VisualStudioProjects\Reminders\Reminders.UnitTests\bin\Debug\netcoreapp3.1\data.rmdr
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.rmdr"), "", Encoding.Unicode);

            rm = new ReminderManager(new OutputTextWriter(new SimulConsoleIO(new OutputWriter()))); //OutputTextWriter will not be used, but is mandatory arg
        }

        //[TestMethod]
        //public void Test()
        //{
        //    Assert.AreEqual(@"D:\Stefan\Programmieren\VisualStudioProjects\Reminders\Reminders\bin\Debug\netcoreapp3.1\", AppDomain.CurrentDomain.BaseDirectory);
        //}

        //method naming scheme: [referenced method]_[aspect tested]_[expected result]

        [TestMethod]
        public void CreateReminder_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE4, "0", "reminder 1");
            
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void ReadReminder_StandardBehaviour()
        {
            int id = rm.CreateReminder(TEST_DATE4, "0", "reminder 1");
            Reminder expected = new Reminder(TEST_DATE4, "0", "reminder 1");

            Reminder r = rm.ReadReminder(id);
            r.Id = -1; //id is irrelevant for test and would fail it otherwise, because the value can change
            string se = JsonConvert.SerializeObject(expected);
            string sa = JsonConvert.SerializeObject(r);

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetRemainingTime1Param_StandardBehaviour_ReturnDateTime() //maybe just set seconds to 0 instead of tostring formatting
        {
            int id = rm.CreateReminder(TEST_DATE4, "0", "reminder 1");
            TimeSpan expected = new DateTime(2040, 12, 20, 1, 1, 0).Subtract(DateTime.Now);

            TimeSpan ts = rm.GetRemainingTime(id);
            string se = expected.ToString(TIME_FORMAT); //format without (fractions of) seconds which would never be equal
            string sa = ts.ToString(TIME_FORMAT);

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetRemainingTime2Param_StandardBehaviour_ReturnDateTime()
        {
            int id = rm.CreateReminder(TEST_DATE4, "0", "reminder 1");
            DateTime dt = new DateTime(2030, 10, 10, 1, 1, 0);
            string se = "3724.00:00"; //expected difference: 3724 days

            TimeSpan ts = rm.GetRemainingTime(id, dt);
            string sa = ts.ToString(TIME_FORMAT);

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetDueReminders_StandardBehaviour_ReturnReminders()
        {
            rm.CreateReminder(TEST_DATE1, "0", "reminder 1");
            rm.CreateReminder(TEST_DATE2, "0", "reminder 2");
            rm.CreateReminder(TEST_DATE3, "0", "reminder 3");
            rm.CreateReminder(TEST_DATE4, "0", "reminder 4");
            string se = TEST_DATE1+","+TEST_DATE2; //due reminders: id1 and id2
            DateTime dt = new DateTime(2022, 01, 01, 01, 01, 00);

            List <Reminder> l = rm.GetDueReminders(dt);
            string sa = string.Join(",", l.Select(i => i.DateString).ToArray());

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetRemindersDueInTimespan_StandardBehaviour_ReturnReminders()
        {
            rm.CreateReminder(TEST_DATE1, "0", "reminder 1");
            rm.CreateReminder(TEST_DATE2, "0", "reminder 2");
            rm.CreateReminder(TEST_DATE3, "0", "reminder 3");
            rm.CreateReminder(TEST_DATE4, "0", "reminder 4");
            string se = TEST_DATE2 + "," + TEST_DATE3; //reminders in timespan: id2 and id3
            DateTime start = new DateTime(2020, 01, 01, 01, 01, 00);
            DateTime end = new DateTime(2040, 01, 01, 01, 01, 00);

            List<Reminder> l = rm.GetRemindersDueInTimespan(start, end);
            string sa = string.Join(",", l.Select(i => i.DateString).ToArray());

            Assert.AreEqual(se, sa);
        }

        [TestMethod]
        public void GetRemindersDueOnDate_StandardBehaviour_ReturnReminders()
        {
            rm.CreateReminder(TEST_DATE1, "0", "reminder 1");
            rm.CreateReminder(TEST_DATE2, "0", "reminder 2");
            rm.CreateReminder(TEST_DATE3, "0", "reminder 3");
            rm.CreateReminder(TEST_DATE4, "0", "reminder 4");
            string se = TEST_DATE3; //reminder on date: id3
            DateTime dt = new DateTime(2030, 12, 20, 01, 01, 00);

            List<Reminder> l = rm.GetRemindersDueOnDate(dt);
            string sa = string.Join(",", l.Select(i => i.DateString).ToArray());

            Assert.AreEqual(se, sa);
        }
    }
}
