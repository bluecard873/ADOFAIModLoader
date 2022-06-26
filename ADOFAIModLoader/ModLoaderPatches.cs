using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ADOFAIModLoader
{
    public class ModLoaderPatches
    {
        #region version text patch
        [HarmonyPatch(typeof(scrVersionText), "Awake")]
        internal static class PatchVersionText_Awake
        {   
            internal static bool Prefix(scrVersionText __instance)
            {
                int RELEASE_NUMBER_FIELD = (int)AccessTools.Field(typeof(GCNS), "releaseNumber").GetValue(null);
                __instance.text.text = string.Format("v{0} (r{1}) (Modified)", Application.version.ToString(), RELEASE_NUMBER_FIELD);
                return false;
            }
        }
        [HarmonyPatch(typeof(scrVersionText), "UpdatePage")]
        internal static class PatchVersionText_UpdatePage
        {
            internal static bool Prefix(scrVersionText __instance, ref int ___page)
            {
                int RELEASE_NUMBER_FIELD = (int)AccessTools.Field(typeof(GCNS), "releaseNumber").GetValue(null);
                ___page = (___page == 0) ? 1 : 0;
                if (___page == 0)
                {
                    __instance.text.text = string.Format("v{0} (r{1}) (Modified)", Application.version.ToString(), RELEASE_NUMBER_FIELD);
                    return false;
                }
                __instance.text.text = GCNS.buildDate;
                return false;
            }
        }

        #endregion
    }
}
