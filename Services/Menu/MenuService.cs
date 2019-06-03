using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Bubblim.Core;

namespace Bubblim.Core.Services
{
    public class MenuService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private Dictionary<ulong, MenuUser> _users;

        public MenuService(
            DiscordSocketClient discord,
            IServiceProvider services)
        {
            _discord = discord;
            _services = services;

            _users = new Dictionary<ulong, MenuUser>();

            _discord.MessageReceived += MessageReceivedAsync;

            PrettyPrint.WriteLine("Menu Service successfully loaded.");
        }

        public async Task MessageReceivedAsync(SocketMessage rawMsg)
        {
            // Ignore system messages, or messages from other bots
            if (rawMsg.Author.IsBot) return;
            if (!(rawMsg is SocketUserMessage msg)) return;
            if (msg.Source != MessageSource.User) return;

            if (UserInMenu(rawMsg.Author))
            {
                // If not a DM, discard message
                if (!msg.Channel.Name.Equals($"@{msg.Author.Username}#{msg.Author.Discriminator}")) return;

                var user = _users[msg.Author.Id];
                var waitingForInput = WaitingForInput(msg.Author);

                // Execute the command if the screen is requesting input and return to main menu
                if (waitingForInput)
                {
                    ExecuteMenuActionFor(user, msg);
                    
                    if (user.currentMenu is IMenuResult)
                    {
                        NavigateUserToMenuResult(user, (IMenuResult)user.currentMenu);
                    }
                    else
                    {
                        NavigateUserToMainMenu(user);
                    }

                    return;
                }

                // Determine if response is a number
                int userOptionId;
                var msgIsNumber = int.TryParse(rawMsg.Content, out userOptionId);

                // Navigate to selected option if it exists. Do nothing otherwise.
                if (msgIsNumber)
                    user.GoTo(userOptionId);

                // If the menu is not requesting input, but has an action, execute it immediately
                if (user.currentMenu is IMenuAction)
                {
                    if (user.currentMenu.RequestingInput()) return;
                    ExecuteMenuActionFor(user, msg);
                    NavigateUserToMainMenu(user);
                }
            }
        }

        /// <summary>
        /// True or false depending on whether a user is already in a menu.
        /// </summary>
        /// <param name="user">The user to check if their within a menu.</param>
        /// <returns>Whether the user is in a menu.</returns>
        public bool UserInMenu(IUser user)
            => _users.ContainsKey(user.Id);

        /// <summary>
        /// True or false depending on whether the current menu the user is on requires input.
        /// </summary>
        /// <param name="user">The user to check if their current menu is requiring input.</param>
        /// <returns>Whether the user's menu is asking for input.</returns>
        public bool WaitingForInput(IUser user)
        {
            var menu = _users[user.Id].currentMenu;

            return menu.RequestingInput();
        }

        /// <summary>
        /// Executes action provided by the menu.
        /// </summary>
        /// <param name="user">User for which to apply the action to.</param>
        /// <param name="msg">Message to to pass to menu action.</param>
        /// <returns>True or false depending on whether the action successfully executed.</returns>
        public bool ExecuteMenuActionFor(MenuUser user, IMessage msg)
        {
            if (!(user.currentMenu is IMenuAction)) return false;
            var action = (IMenuAction)user.currentMenu;

            // Process input using menu's method
            // Queue message returned from action menu
            try
            {
                user.QueueMessage(action.Execute(msg));
            }
            catch (Exception e)
            {
                PrettyPrint.WriteLine("Unable to execute action:\n" + e.Message + '\n' + e.StackTrace, ConsoleColor.Red);
                return false;
            }
            return true;
        }

        public void EnterMenu(MenuBase menu, IUser user)
        {
            try
            {
                if (UserInMenu(user))
                {
                    _users.TryGetValue(user.Id, out var usermenu);
                    usermenu?.Dispose();
                }
            }
            catch (Exception e)
            {
                PrettyPrint.WriteLine(e.Message);
            }

            // Initialize Menu and Display to User
            var menuUser = new MenuUser(user, menu, _users);
            _users.Add(user.Id, menuUser);

            menuUser.DisplayCurrentMenu();
        }

        /// <summary>
        /// Returns specified user to the module's main menu.
        /// </summary>
        /// <param name="user">Desired user to move back to main menu.</param>
        public void NavigateUserToMainMenu(MenuUser user)
        {
            user.currentMenu = user.mainMenu;
            user.DisplayCurrentMenu();
        }

        public void NavigateUserToMenuResult(MenuUser user, IMenuResult menu)
        {
            if (menu.receivingMenu != null)
            {
                user.currentMenu = menu.receivingMenu;
                user.DisplayCurrentMenu();
            }
            else
            {
                NavigateUserToMainMenu(user);
            }
        }
    }
}
