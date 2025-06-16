using System;
using System.Threading;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            // Démarrer le serveur en arrière-plan
            var serverThread = new Thread(() =>
            {
                var server = new Server(5000);
                server.Start();
            })
            { IsBackground = true };
            serverThread.Start();

            // Démarrer Client1Form dans un thread STA
            var client1Thread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Client1Form());
            });
            client1Thread.SetApartmentState(ApartmentState.STA);
            client1Thread.Start();

            // Démarrer Client2Form dans un thread STA
            var client2Thread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Client2Form());
            });
            client2Thread.SetApartmentState(ApartmentState.STA);
            client2Thread.Start();

            // Attendre que les threads se terminent
            client1Thread.Join();
            client2Thread.Join();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
        }
    }
}