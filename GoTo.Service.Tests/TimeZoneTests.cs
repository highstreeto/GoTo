using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoTo.Service.Tests {
    [TestClass]
    public class TimeZoneTests {

        [TestMethod]
        public void TestConversion() {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            var instant = SystemClock.Instance.GetCurrentInstant();

            var time = instant.InZone(london);
            var str = time.ToInstant().ToString();
            var time2 = DateTime.Parse(str);
        }
    }
}
