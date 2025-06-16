using System;
using System.Threading;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Thread serverThread = new Thread(() =>
        {
            Server server = new Server(5000);
            server.Start();
        });

        Thread client1Thread = new Thread(() =>
        {
            Application.EnableVisualStyles();
            Application.Run(new TicTacToeForm());
        });

        Thread client2Thread = new Thread(() =>
        {
            Application.EnableVisualStyles();
            Application.Run(new TicTacToeForm());
        });

        serverThread.Start();
        Thread.Sleep(1000); // Assure que le serveur démarre avant les clients
        client1Thread.Start();
        client2Thread.Start();

        serverThread.Join();
        client1Thread.Join();
        client2Thread.Join();
    }
}