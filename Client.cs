using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
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

    public string SendMove(int x, int y)
    {
        try
        {
            string message = $"MOVE {x} {y}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            var buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }

    private void StartListening()
    {
        Thread listeningThread = new Thread(() =>
        {
            var buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    OnUpdateReceived?.Invoke(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    break;
                }
            }
        });
        listeningThread.IsBackground = true;
        listeningThread.Start();
    }

    public void Close()
    {
        stream.Close();
        client.Close();
    }
}
