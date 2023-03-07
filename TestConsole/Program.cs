using TqkLibrary.GSM;
using TqkLibrary.GSM.Extensions;
using System;
using System.IO.Ports;
using TqkLibrary.GSM.Extensions.Advances;

string[] ports = SerialPort.GetPortNames();

using GsmClient gsmClient = new GsmClient("COM6");

gsmClient.Open();

using var registerMsg = await gsmClient.RegisterMessageAsync();
registerMsg.OnSmsReceived += RegisterMsg_OnSmsReceived;



var a = await gsmClient.CMGF().WriteAsync(MessageFormat.PduMode);
//var a1 = await gsmClient.CMGF().Read();

var b = await gsmClient.CNMI().WriteAsync(CNMI_Mode.Class2, CNMI_MT.SmsDeliver);
//var b1 = await gsmClient.CNMI().Read();

var c = await gsmClient.CPMS().WriteAsync(CPMS_MEMR.SM);

var e = await gsmClient.COPS().ReadAsync();

gsmClient.TestSend();


void RegisterMsg_OnSmsReceived(ISms obj)
{
    Console.WriteLine($"New message from {obj.From} at {obj.ArrivalTime:HH:mm:ss MM-dd-yyyy}: {obj.Message}");
}