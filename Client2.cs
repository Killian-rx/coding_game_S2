using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;

public class Client2Form : Form
{
    private readonly Image crossImage;
    private readonly Image circleImage;
    private Button[,] buttons = new Button[3, 3];
    private TextBox currentPlayerTextBox;
    private TextBox gameStatusTextBox;
    private Client client;

    public Client2Form()
    {
        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        crossImage = Image.FromFile(Path.Combine(exeDir, "cross.png"));
        circleImage = Image.FromFile(Path.Combine(exeDir, "circle.png"));
        client = new Client("127.0.0.1", 5000);
        client.OnUpdateReceived += HandleUpdate;
        InitializeUI();
    }

    private void InitializeUI()
    {
        this.Text = "Client 2 - Morpion";
        this.Size = new Size(300, 400);

        currentPlayerTextBox = new TextBox
        {
            Text = "Current Player: O",
            ReadOnly = true,
            Width = 280,
            Top = 10,
            Left = 10
        };
        this.Controls.Add(currentPlayerTextBox);

        gameStatusTextBox = new TextBox
        {
            Text = "Game Status: Ongoing",
            ReadOnly = true,
            Width = 280,
            Top = 40,
            Left = 10
        };
        this.Controls.Add(gameStatusTextBox);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int xi = i, yj = j;
                buttons[xi, yj] = new Button
                {
                    Text = "",
                    Width = 80,
                    Height = 80,
                    Top = xi * 80 + 80,
                    Left = yj * 80,
                    BackgroundImageLayout = ImageLayout.Zoom
                };
                buttons[xi, yj].Click += (s, e) => OnButtonClick(xi, yj);
                this.Controls.Add(buttons[xi, yj]);
            }
        }
    }

    private void OnButtonClick(int x, int y)
    {
        try
        {
            client.SendMove(x, y);
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
            buttons[x, y].Invoke((MethodInvoker)(() =>
            {
                buttons[x, y].BackgroundImage = player == "X"
                    ? crossImage
                    : circleImage;
                buttons[x, y].Enabled = false;
            }));
            currentPlayerTextBox.Invoke((MethodInvoker)(() =>
            {
                currentPlayerTextBox.Text = $"Current Player: {(player == "X" ? "O" : "X")}";
            }));
        }
        else if (parts[0] == "WIN" && parts.Length == 2)
        {
            string winner = parts[1];
            gameStatusTextBox.Invoke((MethodInvoker)(() =>
                gameStatusTextBox.Text = $"Winner: {winner}"
            ));
            DisableAllButtons();
        }
        else if (parts[0] == "DRAW")
        {
            gameStatusTextBox.Invoke((MethodInvoker)(() =>
                gameStatusTextBox.Text = "Game Status: Draw"
            ));
            DisableAllButtons();
        }
    }

    private void DisableAllButtons()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                buttons[i, j].Enabled = false;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        client.Close();
        base.OnFormClosed(e);
    }
}
