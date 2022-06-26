using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace ADOFAIModLoader
{
    public class Mod : MonoBehaviour
    {
        public virtual void Startup()
        {

        }
        public virtual string ModName
        {
            get
            {
                return "Unknown";
            }
        }
        public virtual string Author
        {
            get
            {
                return "Unknown";
            }
        }
        public virtual List<GameObject> OnRenderSettings()
        {
            return new List<GameObject>();
        }
    }
    public class ModLogger
    {
        static StreamWriter sw = File.CreateText("modlog.log");
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
                msg1 = "[" + i.ModName + "] " + msg;
            }
            Debug.Log(msg1);
            sw.WriteLine(msg1);
            sw.Flush();
        }

        internal static void InternalLog(object msg)
        {
            var msg1 = "[AML] " + msg;
            Debug.Log(msg1);
            sw.WriteLine(msg1);
            sw.Flush();
        }
    }
}
