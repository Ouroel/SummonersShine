using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace SummonersShine.Sounds
{
    public static class ModSounds
    {
        static SoundStyle AGStep;
        static SoundStyle AGTaunt;
        static SoundStyle AGDeath;
        static SoundStyle AGHitPillar;
        static SoundStyle AGRecover;
        public static SoundStyle Shadowraze;
        

        public class ModSounds_Loader : ILoadable
        {
            void ILoadable.Load(Mod mod)
            {
                AGStep = new("SummonersShine/Sounds/AncientGuardian/Minotaur2_step", 0, 5);
                AGTaunt = new("SummonersShine/Sounds/AncientGuardian/Minotaur2_taunt", 0, 4);
                AGDeath = new("SummonersShine/Sounds/AncientGuardian/Minotaur2_death", 0, 2);
                AGHitPillar = new("SummonersShine/Sounds/AncientGuardian/Minotaur2_pillarhit", 0, 3);
                AGRecover = new("SummonersShine/Sounds/AncientGuardian/Minotaur2_recover", 0, 3);
                AGRecover = new("SummonersShine/Sounds/AncientGuardian/Minotaur2_recover", 0, 3);

                Shadowraze = new("SummonersShine/Sounds/Shadowraze");
                Shadowraze.Volume = 0.5f;
            }

            void ILoadable.Unload()
            {
                AGStep = default;
                AGTaunt = default;
                AGDeath = default;
                AGHitPillar = default;
                AGRecover = default;

                Shadowraze = default;
            }
        }
        public static void PlayAGStep(Vector2 pos, float volume = 0.2f)
        {
            AGStep.Volume = volume;
            SoundEngine.PlaySound(AGStep, pos);
        }
        public static void PlayAGTaunt(Vector2 pos)
        {
            AGTaunt.Volume = 0.3f;
            SoundEngine.PlaySound(AGTaunt, pos);
        }

        public static void PlayAGDeath(Vector2 pos)
        {
            AGDeath.Volume = 0.5f;
            SoundEngine.PlaySound(AGDeath, pos);
        }

        public static void PlayAGHitPillar(Vector2 pos)
        {
            AGHitPillar.Volume = 0.5f;
            SoundEngine.PlaySound(AGHitPillar, pos);
        }
        public static void PlayAGRecover(Vector2 pos)
        {
            AGRecover.Volume = 0.5f;
            SoundEngine.PlaySound(AGRecover, pos);
        }
    }
}
