using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Client
{
    private TcpClient client;
    private NetworkStream stream;
    public event Action<string> OnUpdateReceived;

    public Client(string serverIp, int port)
    {
        client = new TcpClient(serverIp, port);
        stream = client.GetStream();
        StartListening();
    }

    public void SendMove(int x, int y)
    {
        string message = $"MOVE {x} {y}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private void StartListening()
    {
        Thread listeningThread = new Thread(() =>
        {
            var buffer = new byte[1024];
            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead <= 0) break;
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    OnUpdateReceived?.Invoke(message);
                }
            }
            catch { }
        });
        listeningThread.IsBackground = true;
        listeningThread.Start();
    }

    public void Close()
    {
        stream?.Close();
        client?.Close();
    }
}
