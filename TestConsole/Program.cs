using TqkLibrary.GSM;
using TqkLibrary.GSM.Extensions;
using System;
using System.IO.Ports;
using TqkLibrary.GSM.Extensions.Advances;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.IO;
using static TqkLibrary.GSM.Extensions.Advances.CMTMessage;
using TqkLibrary.GSM.AtClient;

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


using GsmClient gsmClient = new GsmClient(new AtClientParse(ports[index]));
gsmClient.Open();



//await gsmClient.QFLST().ExecuteAsync().ConfigureAwait(false);
//var result = await gsmClient.QFDWL().WriteAsync("RAM:voice.wav").ConfigureAwait(false);
//byte[] buffer = result.GetCommandResponse("QFDWL")?.BinaryData?.ToArray();
//File.WriteAllBytes("test.wav", buffer);

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
    var download = await obj.AnswerAsync(10000);
    var data = await download.DownloadAsync();
    File.WriteAllBytes("test.wav", data.GetAndCheck());
}