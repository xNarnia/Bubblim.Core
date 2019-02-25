using System;
using System.Collections.Generic;
using System.Text;

namespace Bubblim.Core.Services.PartyRecruiter
{
    public class PartyContainer
    {
        public Party Party { get; set; }
        public System.Timers.Timer Timer { get; set; }
        public Action<Party> Callback { get; set; }
    }
}
