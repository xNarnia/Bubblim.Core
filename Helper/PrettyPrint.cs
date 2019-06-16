using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Bubblim.Core
{
    // Based on PrettyConsole by Dogey
    public static class PrettyPrint
    {
        /// <summary> Write a string to the console on an existing line. </summary>
        /// <param name="text">String written to the console.</param>
        /// <param name="foreground">The text color in the console.</param>
        /// <param name="background">The background color in the console.</param>
        public static void Write(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(text);
        }

        /// <summary> Write a string to the console on an new line. </summary>
        /// <param name="text">String written to the console.</param>
        /// <param name="foreground">The text color in the console.</param>
        /// <param name="background">The background color in the console.</param>
        public static void WriteLine(string text = "", ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            Console.WriteLine();

            TimeStamp();

            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(text);
        }

        private static void TimeStamp()
        {
            PrettyPrint.Write($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
        }

        public static void Log(object severity, string source, string message)
        {
            PrettyPrint.WriteLine($"[{severity}] ", ConsoleColor.DarkMagenta);
            PrettyPrint.Write($"{source}: ", ConsoleColor.Blue);
            PrettyPrint.Write(message, ConsoleColor.White);
        }

        public static Task LogAsync(LogSeverity severity, string source, string message)
        {
            PrettyPrint.WriteLine($"[{severity}] \t", ConsoleColor.DarkMagenta);
            PrettyPrint.Write($"{source}: ", ConsoleColor.Blue);
            PrettyPrint.Write(message, ConsoleColor.White);
            return Task.CompletedTask;
        }

        public static void Log(IUserMessage msg)
        {
            var channel = (msg.Channel as IGuildChannel);
            PrettyPrint.WriteLine();

            if (channel?.Guild == null)
                PrettyPrint.Write($"[PM] ", ConsoleColor.Magenta);
            else
                PrettyPrint.Write($"[{channel.Guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

            PrettyPrint.Write($"{msg.Author}: ", ConsoleColor.Green);
            PrettyPrint.Write(msg.Content, ConsoleColor.White);
        }

        public static void Log(CommandContext c)
        {
            var channel = (c.Channel as SocketGuildChannel);
            PrettyPrint.WriteLine();

            if (channel == null)
                PrettyPrint.Write($"[PM] ", ConsoleColor.Magenta);
            else
                PrettyPrint.Write($"[{c.Guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

            PrettyPrint.Write($"{c.User}: ", ConsoleColor.Green);
            PrettyPrint.Write(c.Message.Content, ConsoleColor.White);
        }
    }
}