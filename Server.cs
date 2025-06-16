using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private TcpListener listener;
    private string[,] board = new string[3, 3];
    private int currentPlayer = 0;
    private string[] players = { "X", "O" };
    private TcpClient[] clients = new TcpClient[2];
    private int connectedClients = 0;

    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                board[i, j] = " ";
    }

    public void Start()
    {
        listener.Start();
        Console.WriteLine("Server started...");
        while (connectedClients < 2)
        {
            var client = listener.AcceptTcpClient();
            clients[connectedClients] = client;
            connectedClients++;
            Console.WriteLine($"Client {connectedClients} connected...");
        }

        Thread client1Thread = new Thread(() => HandleClient(clients[0], 0));
        Thread client2Thread = new Thread(() => HandleClient(clients[1], 1));
        client1Thread.Start();
        client2Thread.Start();
    }

    private void HandleClient(TcpClient client, int playerIndex)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                string response = ProcessMessage(message, playerIndex);
                Broadcast(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                break;
            }
        }
    }

    private string ProcessMessage(string message, int playerIndex)
    {
        var parts = message.Split(' ');
        if (parts[0] == "MOVE" && parts.Length == 3)
        {
            int x = int.Parse(parts[1]);
            int y = int.Parse(parts[2]);
            if (board[x, y] == " " && currentPlayer == playerIndex)
            {
                board[x, y] = players[playerIndex];
                currentPlayer = (currentPlayer + 1) % 2;
                return $"UPDATE {x} {y} {players[playerIndex]}";
            }
            return "INVALID";
        }
        return "ERROR";
    }

    private void Broadcast(string message)
    {
        foreach (var client in clients)
        {
            var stream = client.GetStream();
            byte[] responseBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}
