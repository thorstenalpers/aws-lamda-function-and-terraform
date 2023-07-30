namespace Authenticator.Tests;

using NUnit.Framework;

[TestFixture]
[Category("UnitTests")]
public class UnitTests
{
    [TestCase("https://de.finance.yahoo.com/quote/UNI7083-USD?p=UNI7083-USD&.tsrc=fin-srch", "https://de.finance.yahoo.com/quote/UNI7083-USD/history")]
    [TestCase("https://de.finance.yahoo.com/quote/UNI7083-USD/history", "https://de.finance.yahoo.com/quote/UNI7083-USD/history")]
    [TestCase("https://de.finance.yahoo.com/quote/SAP.DE?p=SAP.DE&.tsrc=fin-srch", "https://de.finance.yahoo.com/quote/SAP.DE/history")]
    [TestCase(null, null)]
    [TestCase("", null)]
    [TestCase(null, null)]
    public void Helper_ToHistoricalCourseUrl(string url, string expectedUrl)
    {
        //var result = Helper.ToHistoricalCourseUrl(url);

        //Assert.AreEqual(expectedUrl, result);
    }
}
