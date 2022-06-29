namespace TqkLibrary.GSM.Test
{
    [TestClass]
    public class TestGsmCommandResponse
    {
        [TestMethod]
        public void TestMethod1()
        {
            var test_str = "+CMGR: \"arg1\",arg2,\"\",\"2022/06/10 17:42:17+28\",\"arg ,with\nlinebreak\"\r\nthis is \r\ndataa";
            GsmCommandResponse gsmCommandResponse = GsmCommandResponse.Parse(test_str);
            Assert.IsNotNull(gsmCommandResponse);
            Assert.AreEqual("CMGR", gsmCommandResponse.Command);
            Assert.IsTrue(gsmCommandResponse.Arguments.Count() == 5);
            Assert.IsTrue(gsmCommandResponse.Arguments.Any(x => x.Equals("\"arg ,with\nlinebreak\"")));
            Assert.AreEqual("this is \r\ndataa", gsmCommandResponse.Data);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var test_str = "+CMGR: (\"ab c\",\"de f\", ghi),(0-5)";
            GsmCommandResponse gsmCommandResponse = GsmCommandResponse.Parse(test_str);
            Assert.IsNotNull(gsmCommandResponse);
            Assert.AreEqual("CMGR", gsmCommandResponse.Command);
            Assert.IsTrue(gsmCommandResponse.Options.Count() == 2);
            Assert.IsTrue(gsmCommandResponse.Options.Any(x => x.Any(y => y.Equals("\"ab c\""))));
        }
    }
}