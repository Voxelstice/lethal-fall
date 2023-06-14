using BepInEx;
using System.Security.Permissions;
using RWCustom;
using UnityEngine;
using System;
using Menu.Remix.MixedUI;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace LethalFall
{
    [BepInPlugin("voxelstice.lethalfall", "Lethal Fall", "0.0.2")]
    public class LethalFall : BaseUnityPlugin
    {
        private LethalFallOptions options;

        public LethalFall()
        {
            try
            {
                options = new LethalFallOptions(this);
            } 
            catch (Exception arg)
            {
                Debug.Log(arg);
            }
        }

        public void OnEnable()
        {
            On.Player.TerrainImpact += TerrainImpactHook;
            On.RainWorld.OnModsInit += OnModsInitHook;
        }

        void OnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                MachineConnector.SetRegisteredOI("voxelstice.lethalfall", options);
            }
            catch (Exception arg)
            {
                Debug.Log(arg);
            }
        }

        void TerrainImpactHook(On.Player.orig_TerrainImpact orig, Player self, int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            orig(self, chunk, direction, speed, firstContact);

            if (firstContact == true)
            {
                if (speed > (options.minSpeedForFallDamage.Value + options.speedForDeath.Value) && direction.y < 0)
                {
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Death, self.mainBodyChunk);
                    UnityEngine.Debug.Log("Fall damage death");
                    self.Die();
                }
                else if (speed > options.minSpeedForFallDamage.Value && direction.y < 0)
                {
                    self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, self.mainBodyChunk);
                    self.Stun((int)Custom.LerpMap(speed, options.minSpeedForFallDamage.Value, (options.minSpeedForFallDamage.Value + options.speedForDeath.Value), 40f, 140f, 2.5f));
                }
            }
        }
    }
    
    public class LethalFallOptions : OptionInterface
    {
        public readonly Configurable<float> minSpeedForFallDamage; // 15
        public readonly Configurable<float> speedForDeath; // 10

        private OpTextBox textBoxMinSpeed;
        private OpTextBox textBoxSpeedDeath;

        public LethalFallOptions(LethalFall plugin)
        {
            minSpeedForFallDamage = base.config.Bind<float>("minSpeedForFallDamage", 15, new ConfigAcceptableRange<float>(0.0f, 60.0f));
            speedForDeath = base.config.Bind<float>("speedForDeath", 10, new ConfigAcceptableRange<float>(0.0f, 60.0f));
        }

        public override void Initialize()
        {
            base.Initialize();

            OpTab val = new OpTab(this, "Options");

            this.Tabs = new OpTab[] { val };

            textBoxMinSpeed = new OpUpdown(minSpeedForFallDamage, new Vector2(16f, 412f), 100f)
            {
                description = "Minimum speed for fall damage"
            };

            textBoxSpeedDeath = new OpUpdown(speedForDeath, new Vector2(140f, 412f), 100f)
            {
                description = "Speed for death (minimum speed + death speed)"
            };

            val.AddItems(new UIelement[1] { textBoxMinSpeed });
            val.AddItems(new UIelement[1] { textBoxSpeedDeath });
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
