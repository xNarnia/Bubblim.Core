using Discord;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace Bubblim.Core.Common
{
    public class BubblimModuleBase : ModuleBase<CommandContext>
    { 
        public async Task<IUserMessage> SendMessageAsync(Embed embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync("", false, embed, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> SendMessageAsync(string message, Embed embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync(message, false, embed, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> SendMessageAsync(Stream stream, string fileName, string message = null, RequestOptions options = null)
        {
            return await Context.Channel.SendFileAsync(stream, fileName, message, false, null, options).ConfigureAwait(false);
            //return await Context.Channel.SendFileAsync(stream, fileName, message, false, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> SendSuccessMessageAsync(string message, Embed embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync($"**{ message }** - :white_check_mark:", false, embed, options).ConfigureAwait(false);
        }

        public async Task<IUserMessage> SendFailMessageAsync(string message, Embed embed = null, RequestOptions options = null)
        {
            return await Context.Channel.SendMessageAsync($"**{ message }** - :x:", false, embed, options).ConfigureAwait(false);
        }
    }
}
