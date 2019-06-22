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
            var london = DateTimeZoneProviders.Tzdb["Europe/Vienna"];
            var instant = SystemClock.Instance.GetCurrentInstant();

            var str3 = instant.ToDateTimeOffset().ToLocalTime().ToString();

            var time = instant.InZone(london);
            var str = time.ToDateTimeOffset().ToString();
            var time2 = DateTimeOffset.Parse(str);
        }
    }
}
