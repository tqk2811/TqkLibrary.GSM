using GsmAtWrapper;
using System;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

var result = Regex.Split("\"test\" csv,\",2,32,\"some ,text\"", ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");


string[] ports = SerialPort.GetPortNames();

using GsmClient gsmClient = new GsmClient("COM6");
gsmClient.OnCommandReceived += GsmClient_OnCommandReceived;

gsmClient.Open();
//var a = await gsmClient.SmsTextMode();
//var b = await gsmClient.GetManufacturer();
//var c = await gsmClient.GetModel();
//var d = await gsmClient.GetFirmware();
//var e = await gsmClient.GetIMEI();
//var f = await gsmClient.GetIMSI();
//var g = await gsmClient.GetICCID();
//var h = await gsmClient.GetMSISDN();
//var i = await gsmClient.IsPinSimRequired();

//await gsmClient.Write("COPS", "?");




//var read = await gsmClient.Read("CMGF");
//var write0 = await gsmClient.Write("CMGF", CancellationToken.None, 1);
//var read = await gsmClient.Read("CMGF");
//var write1 = await gsmClient.Write("CNMI", CancellationToken.None, 2, 2);
//var write2 = await gsmClient.WriteGetResult("CPMS", "\"SM\"");
//var read0 = await gsmClient.Read("CPIN");
//var write3 = await gsmClient.WriteGetResult("CUSD", "1,*101#");
//var write4 = await gsmClient.WriteGetResult("CUSD", "1,*102#");
//var write5 = await gsmClient.WriteGetResult("CUSD", "1,*123#");
//var write6 = await gsmClient.WriteGetResult("CUSD", "1,*163#");
gsmClient.TestSend();


void GsmClient_OnCommandReceived(string arg1, string[] arg2)
{
    switch (arg1)
    {
        case "CMT":

            break;
        default:
            break;
    }
}