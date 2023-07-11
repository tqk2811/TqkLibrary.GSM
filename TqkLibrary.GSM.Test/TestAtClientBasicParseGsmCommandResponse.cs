using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TqkLibrary.GSM.AtClient;

namespace TqkLibrary.GSM.Test
{
    [TestClass]
    public class TestAtClientBasicParseGsmCommandResponse
    {
        [TestMethod]
        public void TestCommandResponse()
        {
            var test_str = "+CMGR: \"arg1\",arg2,\"\",\"2022/06/10 17:42:17+28\",\"arg ,with\nlinebreak\"\r\nthis is \r\ndataa";
            GsmCommandResponse gsmCommandResponse = AtClientEventParse.ParseGsmCommandResponse(test_str);
            Assert.IsNotNull(gsmCommandResponse);
            Assert.AreEqual("CMGR", gsmCommandResponse.Command);
            Assert.IsTrue(gsmCommandResponse.Arguments.Count() == 5);
            Assert.IsTrue(gsmCommandResponse.Arguments.Any(x => x.Equals("\"arg ,with\nlinebreak\"")));
            Assert.AreEqual("this is \r\ndataa", gsmCommandResponse.Data);
        }

        [TestMethod]
        public void Test_TestResponse()
        {
            var test_str = "+CMGR: (\"ab c\",\"de f\", ghi),(0-5)";
            GsmCommandResponse gsmCommandResponse = AtClientEventParse.ParseGsmCommandResponse(test_str);
            Assert.IsNotNull(gsmCommandResponse);
            Assert.AreEqual("CMGR", gsmCommandResponse.Command);
            Assert.IsTrue(gsmCommandResponse.Options.Count() == 2);
            Assert.IsTrue(gsmCommandResponse.Options.Any(x => x.Any(y => y.Equals("\"ab c\""))));
        }

        [TestMethod]
        public void TestParseConnectBinary()
        {
            //var test_str = "\r\nCONNECT\r\n\xab\xff\x34\xac\r\n+QFDWL: 20,3\r\n\r\n+QFDWL: 20,3\r\n"; //will bug here, but supper rate
            var test_str = "\r\nCONNECT\r\n\xab\xff\x34\xac\r\n+QFDWL: 20,3\r\n\xff\r\n+QFDWL: 20,3\r\n";
            GsmCommandResponse gsmCommandResponse = AtClientEventParse.ParseGsmCommandResponse(test_str);
            Assert.IsNotNull(gsmCommandResponse);
            Assert.AreEqual("QFDWL", gsmCommandResponse.Command);
            Assert.IsTrue(gsmCommandResponse.Arguments.Count() == 2);
            Assert.IsTrue(gsmCommandResponse.Arguments.Any(x => x.Equals("20")));
            Assert.IsTrue(gsmCommandResponse.BinaryData.SequenceEqual(
                new byte[] { 0xab, 0xff, 0x34, 0xac }.Concat(Encoding.GetEncoding("ISO-8859-1").GetBytes("\r\n+QFDWL: 20,3\r\n\xff"))));
        }


        [TestMethod]
        public void TestParseAT()
        {
            Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");

            byte[] qfdwl_file = File.ReadAllBytes("QFDWL.file");
            string str_ISO_8859_1 = ISO_8859_1.GetString(qfdwl_file);
            string str_ascii = Encoding.ASCII.GetString(qfdwl_file);
            Assert.AreEqual(str_ISO_8859_1.Length, str_ascii.Length);
            Assert.IsTrue(qfdwl_file.SequenceEqual(ISO_8859_1.GetBytes(str_ISO_8859_1)));

            string[] test_arr = new string[] {
                str_ascii,
                str_ISO_8859_1,
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
        public void TestParseDownloadedFile()
        {
            Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");
            byte[] qfdwl_file = File.ReadAllBytes("QFDWL.file");
            string str_ISO_8859_1 = ISO_8859_1.GetString(qfdwl_file);
            Regex regex = new Regex("^(AT.*?\r)(\r\n[\\x00-\\xFF]*?\r\n|)\r\n(OK|ERROR|\\+CM. ERROR:.*?)\r\n$", RegexOptions.None);
            Match match = regex.Match(str_ISO_8859_1);
            Assert.IsTrue(match.Success);

            GsmCommandResponse gsmCommandResponse = AtClientEventParse.ParseGsmCommandResponse(match.Groups[2].Value);
            Assert.IsNotNull(gsmCommandResponse);
            Assert.AreEqual(gsmCommandResponse.Arguments.Count(), 2);
            var binarySize = int.Parse(gsmCommandResponse.Arguments.First());
            Assert.AreEqual(binarySize, gsmCommandResponse.BinaryData.Count());
            byte[] checksum = gsmCommandResponse.Arguments.Last().HexStringToByteArray();
            Assert.AreEqual(checksum.Length, 2);
            byte[] calcCheckSum = gsmCommandResponse.BinaryData.ToArray().CheckSum();
            Assert.IsTrue(calcCheckSum.SequenceEqual(checksum));
        }

        private static readonly Regex regex_response2
            = new Regex("(^\r\n\\+[\\x00-\\xFF]*?\r\n(?=\r\n)|\r\n\\+[\\x00-\\xFF]*?\r\n$|\r\n[\\x00-\\xFF]*?\r\n)", RegexOptions.Multiline);
        [TestMethod]
        public void TestParseEvent()
        {
            string received = "\r\n+CMT: ,33\r\n07914889200026F5240B914883537892F10008323012519243820E0053006D00730020006D1EAB0075\r\n";
            var matches = regex_response2.Matches(received);
            Assert.AreEqual(matches.Count, 1);
            Assert.AreEqual(matches[0].Value, received);


            received = "\r\nRING\r\n";
            matches = regex_response2.Matches(received);
            Assert.AreEqual(matches.Count, 1);
            Assert.AreEqual(matches[0].Value, received);


            received = "\r\nRING\r\n\r\n+CLIP: \"0383599999\",129,\"\",,\"\",0\r\n";
            matches = regex_response2.Matches(received);
            Assert.AreEqual(matches.Count, 2);
            Assert.AreEqual(matches[0].Value, "\r\nRING\r\n");
            Assert.AreEqual(matches[1].Value, "\r\n+CLIP: \"0383599999\",129,\"\",,\"\",0\r\n");

            received = "\r\n+CRING: VOICE\r\n\r\n+CLIP: \"0383599999\",129,\"\",,\"\",0\r\n";
            matches = regex_response2.Matches(received);
            Assert.AreEqual(matches.Count, 2);
            Assert.AreEqual(matches[0].Value, "\r\n+CRING: VOICE\r\n");
            Assert.AreEqual(matches[1].Value, "\r\n+CLIP: \"0383599999\",129,\"\",,\"\",0\r\n");
        }
    }
}