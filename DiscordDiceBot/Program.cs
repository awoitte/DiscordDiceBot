using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;

namespace DiscordDiceBot
{
    static class Program
    {
        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() => Start();

        static void Start()
        {
            var bot = new DiceBot();
            bot.Start();            
        }
    }
}
