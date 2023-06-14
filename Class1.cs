using BepInEx;
using System.Diagnostics;
using System.Security.Permissions;
using RWCustom;
using UnityEngine;
using System;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace LethalFall
{
    [BepInPlugin("voxelstice.lethalfall", "Lethal Fall", "0.0.1")]
    public class LethalFall : BaseUnityPlugin
    {
        public void OnEnable()
        {
            On.Player.TerrainImpact += TerrainImpactHook;
        }

        void TerrainImpactHook(On.Player.orig_TerrainImpact orig, Player self, int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            orig(self, chunk, direction, speed, firstContact);

            if (firstContact == true)
            {
                if (speed > 25 && direction.y < 0)
                {
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Death, self.mainBodyChunk);
                    UnityEngine.Debug.Log("Fall damage death");
                    self.Die();
                }
                else if (speed > 15 && direction.y < 0)
                {
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, self.mainBodyChunk);
                    self.Stun((int)Custom.LerpMap(speed, 15, 25, 40f, 140f, 2.5f));
                }
            }
        }
    }
}
