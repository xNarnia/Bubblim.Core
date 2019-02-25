using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bubblim.Core.Services.PartyRecruiter
{
    public class Party
    {
        public int PartyLimit { get; set; }
        public Dictionary<ulong, IUser> Users { get; set; }

        private int _UserCount { get; set; }

        public Party()
        {
            PartyLimit = 0;
            _UserCount = 0;
            Users = new Dictionary<ulong, IUser>();
        }

        /// <summary>
        /// Adds a user into the party. Throws an exception if the user is already in the party.
        /// </summary>
        /// <param name="User">User to add.</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddUser(IUser User)
        {
            if (UserInParty(User))
                throw new ArgumentException("User is already in this party.");

            // Party limit here. Exception if exceeded.
            Users.Add(User.Id, User);
            _UserCount++;
        }

        /// <summary>
        /// Removes a user from the party. Throws an exception if the user is not in the party.
        /// </summary>
        /// <param name="User">User to remove.</param>
        /// <exception cref="ArgumentException"></exception>
        public void RemoveUser(IUser User)
        {
            if (!UserInParty(User))
                throw new ArgumentException("User is not in this party.");

            Users.Remove(User.Id);
            _UserCount--;
        }
        
        /// <summary>
        /// Verifies whether the user is already in the party.
        /// </summary>
        /// <param name="User">User to check.</param>
        /// <returns>True or false depending on whether the user is in the party.</returns>
        public bool UserInParty(IUser User)
            => Users.ContainsKey(User.Id);

        /// <summary>
        /// Returns whether the number of users has reached the max limit the party can hold. Always returns false if PartyLimit is 0.
        /// </summary>
        /// <returns>True if full. False if not full or PartyLimit = 0.</returns>
        public bool IsFull()
        {
            if (_UserCount >= PartyLimit && PartyLimit != 0)
                return true;
            return false;
        }
    }
}
