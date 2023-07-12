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