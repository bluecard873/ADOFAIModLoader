using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace ADOFAIModLoader
{
    public class Mod
    {
        public virtual void Startup()
        {

        }
        public virtual string GetModName()
        {
            return "Unknown";
        }
    }
    public class ModLogger
    {
        public ModLogger(Type MainClass)
        {
            mainclass = MainClass;
        }
        private Type mainclass;

        public void Log(object msg)
        {
            var msg1 = "UNKNOWN";
            if (mainclass.BaseType.Equals(typeof(Mod)))
            {
                var i = ModLoaderMain.mods[mainclass];
                 msg1 = "[" + i.GetModName() + "] " + msg;
            }
            if (mainclass.BaseType.Equals(typeof(Plugin)))
            {
                var i = ModLoaderMain.plugins[mainclass];
                msg1 = "[" + i.GetPluginName() + "] " + msg;
            }
            Debug.Log(msg1);
            
        }

        internal static void InternalLog(object msg)
        {
            var msg1 = "[AML] " + msg;
            Debug.Log(msg1);
        }
    }
}
