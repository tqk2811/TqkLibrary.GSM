﻿using TqkLibrary.GSM;
using TqkLibrary.GSM.Extensions;
using System;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

Regex regex_Command = new Regex("^\\+([A-z0-9]+):([\\x20-\\x7E]+)(|\\r\\n[\\x01-\\x7E]+)$", RegexOptions.Multiline);
var test_str = "+CMGR: \"REC READ\",\"123\",\"\",\"2022/06/10 17:42:17+28\"";

Match match = regex_Command.Match(test_str);
if (match.Success)
{

}


//var result = Regex.Split("\"test\" csv,\",2,32,\"some ,text\"", ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");


string[] ports = SerialPort.GetPortNames();

using GsmClient gsmClient = new GsmClient("COM6");
gsmClient.LogCallback += GsmClient_LogCallback;

gsmClient.OnCommandResponse += GsmClient_OnCommandReceived;

gsmClient.Open();
using var registerMsg = await gsmClient.RegisterMessage();

var a = await gsmClient.WriteMessageFormat(MessageFormat.TextMode);
var a1 = await gsmClient.ReadMessageFormat();

var b = await gsmClient.WriteNewMessageIndicationsToTerminalEquipment(CNMI_Mode.Class2, CNMI_MT.SmsDeliver);
var b1 = await gsmClient.ReadWriteNewMessageIndicationsToTerminalEquipment();

var c = await gsmClient.WritePreferredMessageStorage(CPMS_MEMR.SM);

var d = await gsmClient.WriteUnstructuredSupplementaryServiceData(CUSD_N.Enable, "*101#");
//var d1 = await gsmClient.WriteUnstructuredSupplementaryServiceData(CUSD_N.Enable, "*102#");
//var d2 = await gsmClient.WriteUnstructuredSupplementaryServiceData(CUSD_N.Enable, "*103#");

var e = await gsmClient.ReadOperatorSelection();


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


void GsmClient_OnCommandReceived(string cmd, string[] args, string data)
{
    switch (cmd)
    {
        case "CMT":

            break;
        default:
            break;
    }
}
void GsmClient_LogCallback(string obj)
{
    Console.WriteLine(obj);
}