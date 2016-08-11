using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NCalc;

namespace DiscordDiceBot
{
    public static class MessageHandling
    {
        public const string rollCommand = "/roll";
        public const string macroCommand = "/macro";
        public static Regex getNumberOfDice = new Regex(@"(\d*)d");
        public static Regex getValueOfDice = new Regex(@"d(\d+)");
        public static Random rand = new Random();

        public static bool isMacroCommane(string message) =>
            message.StartsWith(macroCommand);


        // "/macro macroname [expression]
        public static string handleMacro(string message, string username, DiceBotDB db, bool logging = true)
        {
            var id = db.getUserId(username);
            if (id == -1) id = db.addUser(username);

            var parametersString = message.Replace(macroCommand, "").Trim();
            var parameters = parametersString.Split(' ').Where(param => param != String.Empty);

            if (parameters.Count() == 0)
                return "Command not understood";
            if(parameters.Count() == 1)
            {
                var macroExpression = db.getMacro(parameters.First(), id);

                if (macroExpression == DiceBotDB.getMacroError) return "You must set the macro first";

                return handleRoll(macroExpression);
            }
            else
            {
                // Create a macro
                var macroTitle = parameters.First();
                var macroExpression = parameters.Skip(1).Aggregate("", (curr, param) => curr + " " + param);

                return handleRoll(db.addMacro(macroTitle, macroExpression, id));
            }
        }


        public static bool isRollCommand(string message)=>
            message.StartsWith(rollCommand);

        public static string handleRoll(string message, bool logging = true)
        {
            var expresssion = message.Replace(rollCommand, "");
            var numDiceMatch = getNumberOfDice.Match(message);
            var diceValueMatch = getValueOfDice.Match(message);

            if (!numDiceMatch.Success || !diceValueMatch.Success) return "Invalid Format";

            int numDice = 1;
            if (numDiceMatch.Groups.Count > 1 && numDiceMatch.Groups[1].Value != "")
                int.TryParse(numDiceMatch.Groups[1].Value, out numDice);

            if (numDice < 0)
                return "Can't roll negative dice >:)";

            if (numDice > 100)
                return "YO COOL YER JETS M8";

            int diceValue = 0;

            if (diceValueMatch.Groups.Count <= 1 || !int.TryParse(diceValueMatch.Groups[1].Value, out diceValue))
                return "Couldn't determine dice value";

            if (diceValue < 1)
                return "Dice must have a positive value";

            var output = "Rolling ";

            output += numDice + "d" + diceValue + " : ";

            List<int> results = new List<int>();

            for (int i = 0; i < numDice; i++)
            {
                if (i > 0) output += ", ";
                int result = rand.Next(1, diceValue + 1);
                results.Add(result);
                output += result;
            }

            int sum = results.Aggregate(0, (curr, roll) => curr + roll);

            if (numDice > 1)
                output += " SUM: " + sum;

            if(message.Length > diceValueMatch.Index + diceValueMatch.Length)
            {
                var expressionString = message.Remove(0, diceValueMatch.Index + diceValueMatch.Length);
                Expression e = new Expression(sum + expressionString);
                try
                {
                    var result = e.Evaluate();

                    if (logging)
                    {
                        Console.WriteLine(output);
                        Console.WriteLine(expressionString);
                        Console.WriteLine(result);
                    }
                    
                    if (result is int)
                        return numDice + "d" + diceValue + expressionString + " : " + result.ToString();
                }
                catch (Exception)
                {
                    output += " !error evaluating expression!";
                }
            }

            return output;
        }
    }
}
