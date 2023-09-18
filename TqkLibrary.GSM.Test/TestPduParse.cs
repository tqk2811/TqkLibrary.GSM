using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.GSM.Extensions;
using TqkLibrary.GSM.Helpers.PduPaser;

namespace TqkLibrary.GSM.Test
{
    [TestClass]
    public class TestPduParse
    {
        const string pdu1 = "07914889200009F46412D0D664914A2D3223442B0000226042817282828D050003BC0302CA6D10555E779F41D17AD8051AD6DF63101C9D06C95C30188CFC729FC37916E89C0E83D06137888E2EBF41EE7338EF0251D3653768D90491EBEF3168FC769F41F3701D24AB81DCE7701E44AEE74174791A44B4BA4043741A444F97E920261214CBE140281839E50251EBA031FA9D0645872CD0FC1D06";
        const string pdu2 = "07914889200009F46412D0D664914A2D3223442B0000226042817262828D050003BC030150D1610A347D87DD20AC93063AD7D3A09CCC860391CB20373AEC06C5602E180C4606D9C36F103D9C06ADD1EFB01BB46C82EC61D0F80D42BFD320BABCEE3E83E8E8FADB7D06CD405469B25805119FCE23688D4E83E8E8701B744E8741747918C47EA741E3701D847EA741F43A88680551D36334889806";
        const string pdu3 = "07914889200009F46412D0D664914A2D3223442B00002260428172038215050003BC0303DC20EA90063AD7D3A0582EE702";
        const string pdu4 = "07914889200026F5400B914883537892F10008226003614542828C05000309020100540069006E0020006E00681EAF006E0020007400691EBF006E00670020007600691EC70074002000540069006E0020006E00681EAF006E0020007400691EBF006E00670020007600691EC700540069006E0020006E00681EAF006E0020007400691EBF006E00670020007600691EC700740020002000540069006E0020006E00681EAF006E";
        const string pdu5 = "07914889200026F5640B914883537892F10008226003614542821E0500030902020020007400691EBF006E00670020007600691EC700740020";
        const string pdu6 = "07914889200026F5240B914883537892F100002270107170058207D4021DE4FEBF01";
        [TestMethod]
        public void TestPdu1()
        {
            byte[] arr = pdu1.HexStringToByteArray();
            var pdu = Pdu.Parse(arr);
            var message = new Message(pdu);
            string text = message.Content;
            string senderNumber = message.SenderNumber;
            int l = text.Length;
        }

        [TestMethod]
        public void TestPdu4()
        {
            byte[] arr = pdu4.HexStringToByteArray();
            var pdu = Pdu.Parse(arr);
            var message = new Message(pdu);
            string text = message.Content;
            string senderNumber = message.SenderNumber;
            int l = text.Length;
        }

        [TestMethod]
        public void TestPdu6()
        {
            byte[] arr = pdu6.HexStringToByteArray();
            var pdu = Pdu.Parse(arr);
            var message = new Message(pdu);
            string text = message.Content;
            string senderNumber = message.SenderNumber;
            int l = text.Length;
        }
    }
}
