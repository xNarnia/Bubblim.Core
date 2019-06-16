using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Bubblim.Core.Services
{
    public class MenuUser
    {
        public IUser user;
        public IMenuItem currentMenu;
        public IMenuItem mainMenu;
        private Dictionary<ulong, MenuUser> _parent;
        private string _queuedMessage;
        private bool rateLimited;
        private Timer _rateTimer;
        private Timer _timeoutTimer;

        public MenuUser(IUser user, MenuBase menu, Dictionary<ulong, MenuUser> parentContainer, int timeOut = 120000)
        {
            this.user = user;
            currentMenu = menu;
            mainMenu = currentMenu;
            _parent = parentContainer;
            rateLimited = true;

            // Prevent user from spamming bot and being rate limited
            _rateTimer = new Timer(2000);
            _rateTimer.Start();
            _rateTimer.Elapsed += EndRateLimit;

            // Remove self from queue when timed out
            _timeoutTimer = new Timer(timeOut);
            _timeoutTimer.Start();
            _timeoutTimer.Elapsed += RemoveSelf;
        }

        /// <summary>
        /// Sets the current menu for the user to the desired menu. If menu option doesn't exist, nothing will change.
        /// </summary>
        /// <param name="menuId">The id assigned to the menu in the list.</param>
        public void GoTo(int optionId)
        {
            if (currentMenu.MenuOptionExists(optionId) && !rateLimited)
            {
                BeginRateLimit();
                currentMenu = currentMenu.GetMenuByOptionId(optionId);
                currentMenu.Display(user);
                RefreshTimeOut();
            }
        }

        public void DisplayCurrentMenu()
        {
            currentMenu.oneTimeMessage = GetQueuedMessages();
            currentMenu.Display(user);
        }

        public void BeginRateLimit()
        {
            rateLimited = true;
            _rateTimer.Start();
        }

        public void RefreshTimeOut()
        {
            _timeoutTimer.Stop();
            _timeoutTimer.Start();
        }

        public void QueueMessage(string input)
        {
            if (_queuedMessage == null)
                _queuedMessage = "";

            _queuedMessage += input + '\n';
        }

        public string GetQueuedMessages()
        {
            var message = _queuedMessage;
            _queuedMessage = null;
            return message;
        }

        public void Dispose()
        {
            _parent.Remove(user.Id);
            _timeoutTimer.Dispose();
        }

        private void EndRateLimit(Object source, ElapsedEventArgs e)
        {
            rateLimited = false;
            _rateTimer.Stop();
        }

        private void RemoveSelf(Object source, ElapsedEventArgs e)
        {
            Dispose();
            user.SendMessageAsync("Menu has closed. Please open the menu again if you wish to continue.");
        }
    }
}
