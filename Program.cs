using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Transactions;
using udp;

if (args.Length <= 0)
{
    System.Console.WriteLine("Enter application port.");
    return;
}
var port = int.Parse(args[0]);

var udpClient = new UdpClient(port);
var listener = new UdpListener(udpClient);
listener.ReceivedMessage += (string message) => Console.WriteLine(message);
listener.StartListening();
Console.WriteLine("Write 'exit' to close the program or the message to send.");
while (true)
{
    var message = Console.ReadLine();
    if (message == "exit")
    {
        try
        {
            listener.StopListening();
            udpClient.Close();
            System.Console.WriteLine("=============================");
            await Task.Delay(5000);
            return;
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    Console.WriteLine("Enter port to send.");
    var portOut = int.Parse(Console.ReadLine());
    Byte[] messageOut = Encoding.ASCII.GetBytes(message);
    udpClient.Send(messageOut, messageOut.Length, "localhost", portOut);
}


