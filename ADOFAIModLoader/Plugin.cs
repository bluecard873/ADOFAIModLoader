using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOFAIModLoader
{
    public class Plugin
    {
        public virtual void Startup()
        {

        }
        public virtual string GetPluginName()
        {
            return "Unknown";
        }
    }
}
