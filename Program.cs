using System;
using System.Threading;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "server")
        {
            Server server = new Server(5000);
            server.Start();
        }
        else if (args.Length > 0 && args[0] == "clients")
        {
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

            client1Thread.Start();
            client2Thread.Start();

            client1Thread.Join();
            client2Thread.Join();
        }
        else
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run -- server   // Start the server");
            Console.WriteLine("  dotnet run -- clients  // Start two clients");
        }
    }
}
