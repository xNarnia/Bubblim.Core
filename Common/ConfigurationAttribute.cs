using System;
using System.Collections.Generic;
using System.Text;

namespace Bubblim.Core.Common
{
    public class OnLoadConfiguration : System.Attribute
    {
        private string name;

        public OnLoadConfiguration(string name)
        {
            this.name = name;
        }
    }
}
