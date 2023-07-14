﻿using System;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

using TqkLibrary.GSM;
using TqkLibrary.GSM.AtClient;
using TqkLibrary.GSM.Extended;
using TqkLibrary.GSM.Extended.Advances;
using TqkLibrary.GSM.Extensions;
using static TqkLibrary.GSM.Extended.Advances.CMTMessage;
using static TqkLibrary.GSM.Extended.CommandRequestCMGF;
using static TqkLibrary.GSM.Extended.CommandRequestCNMI;
using static TqkLibrary.GSM.Extended.CommandRequestCPMS;

string[] ports = SerialPort.GetPortNames();
Console.WriteLine("Select port:");
for (int i = 0; i < ports.Length; i++)
{
    Console.WriteLine($"{i}: {ports[i]}");
}
int index = -1;
do
{
    string line = Console.ReadLine();
    if (int.TryParse(line, out int l))
    {
        if (l >= 0 && l < ports.Length)
        {
            index = l;
            break;
        }
    }
}
while (index == -1);


using GsmClient gsmClient = new GsmClient(new AtClientLoopReadLine(ports[index]));
gsmClient.Open();


//var qflst = await gsmClient.QFLST().WriteAsync("RAM:*").ConfigureAwait(false);
//var qfdwl = await gsmClient.QFDWL().WriteAsync(qflst.First()).ConfigureAwait(false);
//qfdwl.GetAndCheck();


SimEventUtils simEventUtils = gsmClient.RegisterSimEventUtils();
simEventUtils.OnCallingClip += SimEventUtils_OnCallingClip;
await simEventUtils.EnableClip();

using var registerMsg = await gsmClient.RegisterMessageAsync();
registerMsg.OnSmsReceived += RegisterMsg_OnSmsReceived;

var a = await gsmClient.CMGF().WriteAsync(MessageFormat.PduMode);
var a1 = await gsmClient.CMGF().ReadAsync();

var b = await gsmClient.CNMI().WriteAsync(CNMI_Mode.Class2, CNMI_MT.SmsDeliver);
var b1 = await gsmClient.CNMI().ReadAsync();

var c = await gsmClient.CPMS().WriteAsync(CPMS_MEMR.SM);

var e = await gsmClient.COPS().ReadAsync();

var qspn = await gsmClient.QSPN().ExecuteAsync();

var cmgs = await gsmClient.CMGS().WriteAsync("+84383587291", "test msg");


while (true)
{
    string command = Console.ReadLine()?.Trim();
    gsmClient.Debug(command);
}


void RegisterMsg_OnSmsReceived(ISms obj)
{
    Console.WriteLine($"New message from {obj.From} at {obj.ArrivalTime:HH:mm:ss MM-dd-yyyy}: {obj.Message}");
}
async void SimEventUtils_OnCallingClip(AnswerCallHelper obj)
{
    await obj.DeleteFileAsync();
    var download = await obj.AnswerAsync(10000);
    var data = await download.DownloadAsync();
    File.WriteAllBytes("test.wav", data.GetAndCheck());
}