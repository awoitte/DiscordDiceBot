using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDiceBot
{
    public class DiceBotDB
    {
        public SQLiteConnection m_dbConnection;
        public const string getMacroError = "ERROR";
        public const string createMacros = @"CREATE TABLE 'macros' (
    Macro_id        INTEGER PRIMARY KEY
                            NOT NULL,
    User_id         INTEGER REFERENCES users (User_id) 
                            NOT NULL,
    Title           STRING  NOT NULL,
    Dice_expression STRING  NOT NULL
);";
        public const string createUsers = @"CREATE TABLE 'users' (
    User_id  INTEGER PRIMARY KEY AUTOINCREMENT
                     NOT NULL,
    Username STRING  NOT NULL
);
";

        public DiceBotDB()
        {
            if (!File.Exists("DiceBot.db"))
            {
                SQLiteConnection.CreateFile("DiceBot.db");
                m_dbConnection = new SQLiteConnection("Data Source=DiceBot.db;Version=3;");
                m_dbConnection.Open();
                new SQLiteCommand(createUsers, m_dbConnection).ExecuteNonQuery();
                new SQLiteCommand(createMacros, m_dbConnection).ExecuteNonQuery();
            }
            else
            {
                m_dbConnection = new SQLiteConnection("Data Source=DiceBot.db;Version=3;");
                m_dbConnection.Open();
            }

            
        }

        public int getUserId(string username)
        {
            SQLiteCommand command = new SQLiteCommand(@"select User_id from users WHERE Username = $username", m_dbConnection);
            command.Parameters.AddWithValue("$username", username);
            SQLiteDataReader reader = command.ExecuteReader();

            if (!reader.HasRows || !reader.Read()) return -1;
            var id = reader.GetInt64(0);

            return (int)id;
        }

        public int addUser(string username)
        {
            SQLiteCommand command = new SQLiteCommand(@"INSERT INTO users (Username) VALUES($username);", m_dbConnection);
            command.Parameters.AddWithValue("$username", username);
            SQLiteDataReader reader = command.ExecuteReader();
            return getUserId(username);
        }

        public string addMacro(string title, string expression, int userId)
        {

            var isNew = getMacro(title, userId) == getMacroError;
            SQLiteCommand command;

            if (isNew)
            {
                command = new SQLiteCommand(@"INSERT INTO macros (User_id,Title,Dice_expression) VALUES($userId,$title,$expression);", m_dbConnection);
                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("$title", title);
                command.Parameters.AddWithValue("$expression", expression);
            }
            else
            {
                command = new SQLiteCommand(@"UPDATE macros SET Dice_expression = $expression WHERE title = $title;", m_dbConnection);
                command.Parameters.AddWithValue("$title", title);
                command.Parameters.AddWithValue("$expression", expression);
            }
            SQLiteDataReader reader = command.ExecuteReader();

            return getMacro(title, userId);
        }

        public string getMacro(string title, int userId)
        {
            SQLiteCommand command = new SQLiteCommand(@"SELECT Dice_expression  FROM macros  Where User_id = $userId  AND Title = $title;", m_dbConnection);
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("$title", title);
            SQLiteDataReader reader = command.ExecuteReader();

            if (!reader.HasRows || !reader.Read())
            {
                Console.WriteLine("Error getting macro");
                return getMacroError;
            };
            return reader.GetString(0);
        }

    }
}
