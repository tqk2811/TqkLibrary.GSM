using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        [TestMethod]
        public void TestMethod3()
        {
            //var test_str = "\r\nCONNECT\r\n\xab\xff\x34\xac\r\n+QFDWL: 20,3\r\n\r\n+QFDWL: 20,3\r\n"; //will bug here, but supper rate
            var test_str = "\r\nCONNECT\r\n\xab\xff\x34\xac\r\n+QFDWL: 20,3\r\n\xff\r\n+QFDWL: 20,3\r\n";
            GsmCommandResponse gsmCommandResponse = GsmCommandResponse.Parse(test_str);
            Assert.IsNotNull(gsmCommandResponse);
            Assert.AreEqual("QFDWL", gsmCommandResponse.Command);
            Assert.IsTrue(gsmCommandResponse.Arguments.Count() == 2);
            Assert.IsTrue(gsmCommandResponse.Arguments.Any(x => x.Equals("20")));
            Assert.IsTrue(gsmCommandResponse.BinaryData.SequenceEqual(
                new byte[] { 0xab, 0xff, 0x34, 0xac }.Concat(Encoding.GetEncoding(1252).GetBytes("\r\n+QFDWL: 20,3\r\n\xff"))));
        }


        [TestMethod]
        public void TestMethod4()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding window1252 = Encoding.GetEncoding(1252);

            byte[] qfdwl_file = File.ReadAllBytes("QFDWL.file");
            string str_1252 = window1252.GetString(qfdwl_file);
            string str_ascii = Encoding.ASCII.GetString(qfdwl_file);
            string[] test_arr = new string[] {
                str_ascii,
                str_1252,
                "AT+QFDWL=\"RAM:sound.wav\"\r\r\nCONNECT\r\n\xab\xff\0\0\0\x34\xac\r\n+QFDWL: 20,3\r\n\r\nOK\r\n",
                "AT+COPS?\r\r\n+COPS: 0,0,\"VINAPHONE\"\r\n\r\nOK\r\n",
                "AT+QFDWL=\"RAM:voicea.wav\"\r\r\n+CME ERROR: 4010\r\n",
            };
            Regex regex = new Regex("^(AT.*?\r)(\r\n[\\x00-\\xFF]*?\r\n|)\r\n(OK|ERROR|\\+CM. ERROR:.*?)\r\n$", RegexOptions.None);
            foreach (var test in test_arr)
            {
                Match match = regex.Match(test);
                Assert.IsTrue(match.Success);
            }
        }

        [TestMethod]
        public void TestMethod5()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            byte[] buffer = new byte[] { 0xab, 0xff, 0x34, 0xac };
            string str = Encoding.GetEncoding(1252).GetString(buffer);
            Assert.IsTrue(str.Select(x => (byte)x).SequenceEqual(buffer));
            Assert.IsTrue(Encoding.GetEncoding(1252).GetBytes(str).SequenceEqual(buffer));
        }
    }
}