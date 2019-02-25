using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bubblim.Core
{
    public class CommandContext : ICommandContext
    {
        public DiscordSocketClient Client { get; }
        public SocketGuild Guild { get; }
        public ISocketMessageChannel Channel { get; }
        public SocketUser User { get; }
        public SocketUserMessage Message { get; }

        public bool IsPrivate => Channel is IPrivateChannel;

        public CommandContext(DiscordSocketClient client, SocketUserMessage message, SocketUser user = null)
        {
            Client = client;
            Guild = (message.Channel as SocketGuildChannel)?.Guild;
            Channel = message.Channel;
            User = user ?? message.Author;
            Message = message;
        }

        //ICommandContext
        IDiscordClient ICommandContext.Client => Client;
        IGuild ICommandContext.Guild => Guild;
        IMessageChannel ICommandContext.Channel => Channel;
        IUser ICommandContext.User => User;
        IUserMessage ICommandContext.Message => Message;
    }
}
