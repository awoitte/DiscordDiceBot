using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;

namespace DiscordDiceBot
{
    static class Program
    {
        private static DiscordClient _client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() => Start();

        static void Start()
        {
            ;
            string botToken = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "BotToken.txt"));

            _client = new DiscordClient();

            _client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor && MessageHandling.isRollCommand(e.Message.Text))
                    await e.Channel.SendMessage(MessageHandling.handleRoll(e.Message.Text));
            };

            _client.ExecuteAndWait(async () => {
                await _client.Connect(botToken);
            });
        }
    }
}
