using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bubblim.Core.Services
{
    public interface IMenuModule
    {
        string commandText { get; set; }
        IMenuItem NewMenu();
    }

    public interface IMenuAction
    {
        /// <summary>
        /// Execute action performed by the menu.
        /// </summary>
        /// <param name="msg">Message send by the User</param>
        /// <returns>String message that indicates the result of the action</returns>
        string Execute(IMessage msg);
    }

    public interface IMenuResult
    {
        IMenuItem receivingMenu { get; set; }
    }

    public interface IMenuItem
    {
        string title { get; set; }
        string summary { get; set; }
        string instructions { get; set; }
        string oneTimeMessage { get; set; }

        void Display(IUser user);
        bool RequestingInput();
        bool MenuOptionExists(int optionId);
        IMenuItem GetMenuByOptionId(int optionId);
    }

    //public interface IMenuInput
    //{
    //    void Execute(IMessage msg);
    //}
}
