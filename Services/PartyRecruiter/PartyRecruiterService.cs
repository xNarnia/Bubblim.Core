using Bubblim.Core.Services.PartyRecruiter;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Bubblim.Core.Services
{
    public class PartyRecruiterService
    {
        private DiscordSocketClient _discord { get; set; }
        private IServiceProvider _services { get; set; }
        private Dictionary<string, PartyContainer> _parties { get; set; }

        public PartyRecruiterService(
            DiscordSocketClient discord,
            IServiceProvider services)
        {
            _discord = discord;
            _services = services;
            _parties = new Dictionary<string, PartyContainer>();
        }

        public string RecruitUsers(string id, Action<Party> callback)
            => RecruitUsers(id, 30, callback);

        public string RecruitUsers(string id, int timeoutSeconds, Action<Party> callback)
        {
            // Temporary
            if (_parties.ContainsKey(id))
                throw new ArgumentException("Party already exists with that id!");

            var container = new PartyContainer()
            {
                Party = new Party(),
                Timer = new Timer(timeoutSeconds * 1000),
                Callback = callback
            };

            _parties.Add(id, container);

            container.Timer.Elapsed += delegate (object sender, ElapsedEventArgs e) {
                RunCallbackAndCleanUp(id);
            };
            container.Timer.Start();

            return id;
        }

        public bool AddUserToParty(string id, IUser user)
        {
            Party party = RetrieveParty(id);

            if (party == null)
                return false;

            if (party.IsFull() || party.UserInParty(user))
                return false;

            party.AddUser(user);
            return true;
        }

        public void FinishRecruiting(string id)
        {
            RunCallbackAndCleanUp(id);
        }

        private Party RetrieveParty(string id)
        {
            PartyContainer container;
            _parties.TryGetValue(id, out container);

            if (container == null)
                return null;

            return container.Party;
        }

        /// <summary>
        /// Returns the Party's timer with the assigned key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Timer RetrieveTimer(string id)
        {
            PartyContainer container;
            _parties.TryGetValue(id, out container);

            if (container == null)
                return null;

            return container.Timer;
        }

        private void RunCallbackAndCleanUp(string id)
        {
            PartyContainer container;
            _parties.TryGetValue(id, out container);

            if (container == null) return;

            // Send formed party to callback
            container.Callback(container.Party);

            // Remove party from dictionary or else it's never removed
            _parties.Remove(id);

            // TODO: Need a way to reference this timer again so it can be fast forwarded, canceled, or rewinded
            // TODO: Get time left?
            container.Timer.Stop();
            container.Timer.Dispose();
        }
    }
}