using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordDiceBot
{

    class DiceBot
    {
        private static DiscordClient _client;
        public DiceBotDB db = new DiceBotDB();

        public void Start()
        {
            string botToken = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "BotToken.txt"));

            _client = new DiscordClient();

            _client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor && MessageHandling.isMacroCommane(e.Message.Text))
                    await e.Channel.SendMessage(MessageHandling.handleMacro(e.Message.Text, e.User.Name, db));
                else if (!e.Message.IsAuthor && MessageHandling.isRollCommand(e.Message.Text))
                    await e.Channel.SendMessage(MessageHandling.handleRoll(e.Message.Text));
            };

            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(botToken);
            });
        }

    }
}
