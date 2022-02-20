using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace ADOFAIModLoader
{
    public class ADOBASECLASS : ADOBase
    {
       
    }
    public static class ModLoaderMain
    {
        public static Dictionary<Type, Mod> mods = new Dictionary<Type, Mod>();
        public static Dictionary<Type,Mod> plugins = new Dictionary<Type, Mod>();

        public static void LoadMods()
        {
            var stopwatch = Stopwatch.StartNew();
           
            var pluginDir = new DirectoryInfo("./Plugins");
            var modDir = new DirectoryInfo("./Mods");
            if (!pluginDir.Exists)
            {
                pluginDir.Create();
            }
            if (!modDir.Exists)
            {
                modDir.Create();
            }
            foreach (DirectoryInfo d in pluginDir.GetDirectories())
            {
                DirectoryInfo libfolder = new DirectoryInfo(d.FullName + "Libs");
                if (libfolder.Exists)
                {
                    foreach (FileInfo f in libfolder.GetFiles("*.dll"))
                    {
                        Assembly.LoadFile(f.FullName);
                    }
                }
                var fa = d.GetFiles("*.dll")[0];
                var asm = Assembly.LoadFile(fa.FullName);
                foreach (Type t in asm.GetTypes())
                {
                    if (t.BaseType.Equals(typeof(Mod)))
                    {
                        var i = (Mod)Activator.CreateInstance(t);
                        var pluginName = i.GetModName();
                        plugins.Add(t, i);
                        i.Startup();
                        ModLogger.InternalLog("Loaded Plugin " + pluginName);
                    }
                }
            }
            foreach (DirectoryInfo d in modDir.GetDirectories())
            {
                DirectoryInfo libfolder = new DirectoryInfo(d.FullName + "Libs");
                if (libfolder.Exists)
                {
                    foreach (FileInfo f in libfolder.GetFiles("*.dll"))
                    {
                        Assembly.LoadFile(f.FullName);
                    }
                }

                var fa = d.GetFiles("*.dll")[0];
                var asm = Assembly.LoadFile(fa.FullName);
                foreach (Type t in asm.GetTypes())
                {
                    if (t.BaseType.Equals(typeof(Mod)))
                    {
                        var i = (Mod)Activator.CreateInstance(t);
                        var modName = i.GetModName();
                        mods.Add(t, i);
                        i.Startup();
                        ModLogger.InternalLog("Loaded Mod " + modName);
                    }
                    
                }
                stopwatch.Stop();
                ModLogger.InternalLog("Loaded " + mods.Count + " mods and " + plugins.Count + " plugins in " + stopwatch.ElapsedMilliseconds + "ms.");
            }
        }

    }
}
