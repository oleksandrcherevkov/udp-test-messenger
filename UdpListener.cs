using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;

namespace udp;

class UdpListener
{
    public event Action<string> ReceivedMessage;
    private readonly UdpClient client;
    private Task listeningLoop;
    private CancellationTokenSource cancelSource = new CancellationTokenSource();
    public UdpListener(UdpClient client)
    {
        this.client = client;
    }

    public void StartListening()
    {
        if (listeningLoop != null)
        {
            System.Console.WriteLine("Listener can not be started more than one time.");
        }
        cancelSource = new CancellationTokenSource();
        var groupEP = new IPEndPoint(IPAddress.Any, 0);
        listeningLoop = Task.Factory.StartNew(() =>
        {
            while (true)
            {
                cancelSource.Token.ThrowIfCancellationRequested();
                byte[] bytes = client.Receive(ref groupEP);
                string message = Encoding.ASCII.GetString(bytes);
                ReceivedMessage?.Invoke(message);
            }
        }, cancelSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        System.Console.WriteLine("Started listening.");
    }
    public void StopListening()
    {
        cancelSource.Cancel();
    }
}