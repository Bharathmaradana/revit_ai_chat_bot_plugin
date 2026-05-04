using System;
using System.Windows;
using System.Threading.Tasks;

namespace ChatUI
{
    public partial class MainWindow : Window
    {
        // 🔥 Bridge (IMPORTANT)
        public static Func<string, Task<string>> ProcessMessage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = ChatInput.Text;

            if (string.IsNullOrWhiteSpace(input))
                return;

            ChatOutput.Text += "\nYou: " + input;
            ChatInput.Text = "";

            ChatOutput.Text += "\nBot: thinking...";

            try
            {
                string response = "No processor connected.";

                if (ProcessMessage != null)
                {
                    response = await ProcessMessage(input);
                }

                ChatOutput.Text = ChatOutput.Text.Replace("Bot: thinking...", "Bot: " + response);
            }
            catch (Exception ex)
            {
                ChatOutput.Text += "\nError: " + ex.Message;
            }
        }
    }
}