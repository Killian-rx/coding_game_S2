using System;
using System.Windows.Forms;

public class TicTacToeForm : Form
{
    private Button[,] buttons = new Button[3, 3];
    private Client client;

    public TicTacToeForm()
    {
        client = new Client("127.0.0.1", 5000);
        client.OnUpdateReceived += HandleUpdate;
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = "Morpion";
        this.Size = new System.Drawing.Size(300, 300);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                buttons[i, j] = new Button
                {
                    Text = "",
                    Width = 80,
                    Height = 80,
                    Top = i * 80,
                    Left = j * 80
                };
                buttons[i, j].Click += (sender, e) => OnButtonClick(i, j);
                this.Controls.Add(buttons[i, j]);
            }
        }
    }

    private void OnButtonClick(int x, int y)
    {
        try
        {
            string response = client.SendMove(x, y);
            if (response == "INVALID")
            {
                MessageBox.Show("Invalid move!");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void HandleUpdate(string message)
    {
        var parts = message.Split(' ');
        if (parts[0] == "UPDATE" && parts.Length == 4)
        {
            int x = int.Parse(parts[1]);
            int y = int.Parse(parts[2]);
            string player = parts[3];
            buttons[x, y].Invoke((MethodInvoker)(() => buttons[x, y].Text = player));
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        client.Close();
        base.OnFormClosed(e);
    }
}
