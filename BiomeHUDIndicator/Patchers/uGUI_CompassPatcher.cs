﻿namespace BiomeHUDIndicator.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Linq;
    using System.Text;
    using Harmony;
    using SMLHelper.V2.Handlers;
    using UnityEngine;
    using Fabricator;

    [HarmonyPatch(typeof(uGUI_DepthCompass))]
    [HarmonyPatch("IsCompassEnabled")]
    internal class DepthCompass_IsCompassEnabledPatcher
    {
        private static string _cachedBiome = "unassigned";
        private static string _cachedBiomeFriendly = "Unassigned";

        [HarmonyPrefix]
        public static bool Prefix(ref uGUI_DepthCompass __instance, ref bool __result)
        {
            if (__instance == null)
            {
                __result = false;
                return false;
            }
            if (!__instance._initialized)
            {
                __result = false;
                return false;
            }
            if (!uGUI.isMainLevel)
            {
                __result = false;
                return false;
            }
            if (LaunchRocket.isLaunching)
            {
                __result = false;
                return false;
            }
            if (uGUI.isIntro)
            {
                __result = false;
                return false;
            }
            Player main = Player.main;
            if (main == null)
            {
                __result = false;
                return false;
            }
            PDA pda = main.GetPDA();
            if (pda != null && pda.isInUse)
            {
                __result = false;
                return false;
            }
            Player.Mode mode = main.GetMode();
            if (mode == Player.Mode.Piloting)
            {
                __result = false;
                return false;
            }
            Inventory main2 = Inventory.main;
            if (main2 != null && main2.equipment != null && TechTypeCheck(main2))
            {
                __result = true;
                int biomeChip = main2.equipment.GetCount(CompassCore.BiomeChipID);
                if (biomeChip > 0)
                {
                    string curBiome = main.GetBiomeString();
                    int index = curBiome.IndexOf('_');
                    curBiome = curBiome.Substring(0, index);
                    curBiome = curBiome.ToLower();
                    if (curBiome != _cachedBiome)
                    {
                        _cachedBiomeFriendly = biomeList[curBiome];
                        ErrorMessage.AddMessage("[BiomeHUDIndicator] Value of _cachedBiome is currently: " + _cachedBiome);
                        ErrorMessage.AddMessage("[BiomeHUDIndicator] Value of curBiome is currently: " + curBiome); // Remove after verifying it updates
                        ErrorMessage.AddMessage("[BiomeHUDIndicator] Value of _cachedBiomeFriendly is currently: " + _cachedBiomeFriendly); // Remove after verifying it updates
                    }
                }
                return false;
            }
            uGUI_CameraDrone main3 = uGUI_CameraDrone.main;
            __result = main3 != null && main3.GetCamera() != null;
            return false;
        }

        // This checks and returns whether or not the compass and/or biome chip are present.
        private static bool TechTypeCheck(Inventory inv)
        {
            int compassID = inv.equipment.GetCount(TechType.Compass);
            int biomeChip = inv.equipment.GetCount(CompassCore.BiomeChipID);
            if(compassID > 0 || biomeChip > 0)
            {
                return true;
            }
            return false;
        }

        private static Dictionary<string, string> biomeList = new Dictionary<string, string>()
        {
            { "safeshallows", "Safe Shallows" },
            { "kelp", "Kelp Forest" },
            { "grassyplateaus", "Grassy Plateaus" },
            { "mushroomforest", "Mushroom Forest" },
            { "jellyshroomcaves", "Jellyshroom Caves" },
            { "sparsereef", "Sparse Reef" },
            { "floatingislands" , "Floating Island" },
            { "shipspecial" , "Aurora" },
            { "shipinterior", "Aurora" },
            { "crashhome" , "Aurora" },
            { "aurora" , "Aurora" },
            { "crashzone" , "Crash Zone" },
            { "underwaterislands" , "Underwater Islands" },
            { "seatreaderpath" , "Sea Treader's Path" },
            { "grandreef" , "Grand Reef" },
            { "deepgrandreef" , "Deep Grand Reef" },
            { "mountains" , "Mountains" },
            { "dunes" , "Dunes" },
            { "lostriverjunction" , "Lost River Junction" },
            { "lostrivercorridor" , "Lost River Corridor" },
            { "skeletoncave" , "Skeleton Cave" },
            { "treecove" , "Tree Cove" },
            { "ghosttree" , "Ghost Tree" },
            { "cragfield" , "Crag Field" },
            { "bonesfield" , "Bone Field" },
            { "kooshzone" , "Bulb Zone" },
            { "bloodkelp" , "Blood Kelp Zone" },
            { "inactivelavazone" , "Inactive Lava Zone" },
            { "activelavazone" , "Active Lava Zone" },
            { "mesas" , "Mesas" },
            { "prisonaquarium" , "Primary Containment Facility" },
            { "observatory" , "Observatory" },
            { "generatorroom" , "Generator Room" },
            { "crashedship" , "Aurora" },
            { "precursorgun" , "Precursor Facility" },
            { "prison" , "Primary Containment Facility" },
            { "unassigned" , "Unassigned" },
        };
    }
}