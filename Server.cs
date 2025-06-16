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
            Console.WriteLine($"Client {connectedClients + 1} connected...");
            int playerIndex = connectedClients;
            Thread clientThread = new Thread(() => HandleClient(client, playerIndex));
            clientThread.Start();
            connectedClients++;
        }
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
                if (!string.IsNullOrEmpty(response))
                {
                    Broadcast(response);
                }
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
            int x = int.Parse(parts[1]), y = int.Parse(parts[2]);
            if (board[x, y] == " " && currentPlayer == playerIndex)
            {
                board[x, y] = players[playerIndex];
                Broadcast($"UPDATE {x} {y} {players[playerIndex]}");

                if (CheckWin(players[playerIndex]))
                {
                    Broadcast($"WIN {players[playerIndex]}");
                }
                else if (CheckDraw())
                {
                    Broadcast("DRAW");
                }
                else
                {
                    currentPlayer = (currentPlayer + 1) % 2;
                }

                return null;
            }
            return "INVALID";
        }
        return "ERROR";
    }

    private bool CheckWin(string player)
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == player) return true;
            if (board[0, i] == player && board[1, i] == player && board[2, i] == player) return true;
        }
        if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player) return true;
        if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player) return true;
        return false;
    }

    private bool CheckDraw()
    {
        foreach (var cell in board)
        {
            if (cell == " ") return false;
        }
        return true;
    }

    private void Broadcast(string message)
    {
        foreach (var client in clients)
        {
            if (client != null)
            {
                var stream = client.GetStream();
                byte[] responseBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}
