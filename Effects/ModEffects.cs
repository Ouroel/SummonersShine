using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SummonersShine.Effects
{
    public static class ModEffects {
        public static int SnowPuff;
        public static int Coin;
        public static int Gem;
        public static int[] HornetHive = new int[3];
        public static int HornetCocoon;
        public static int[] SpiderEgg = new int[2];
        public static int SpiderCocoon;
        public static int Retinassembler;
        public static int Retinassembler_Lights;
        public static int Spazmatiassembler;
        public static int Spazmatiassembler_Lights;
        public static int SightSoul;
        public static int RavenFeather;
        public static int FinchFeather;
        public static int ImpSigil;
        public static int ChiunStar;
        public static int Lifesteal_Sanguine;
        public static int DD2Hammer;

        public static int PoisonPuff;
        public static int PoisonPuffSmall;

        public static void Populate()
        {
            if (!Main.dedServ)
            {
                SnowPuff = ModContent.Find<ModGore>("SummonersShine/SnowPuff").Type;
                Coin = ModContent.Find<ModDust>("SummonersShine/Coin").Type;
                Gem = ModContent.Find<ModDust>("SummonersShine/Gem").Type;
                HornetHive[0] = ModContent.Find<ModGore>("SummonersShine/HornetHive0").Type;
                HornetHive[1] = ModContent.Find<ModGore>("SummonersShine/HornetHive1").Type;
                HornetHive[2] = ModContent.Find<ModGore>("SummonersShine/HornetHive2").Type;
                HornetCocoon = ModContent.Find<ModGore>("SummonersShine/HornetCocoon").Type;
                SpiderEgg[0] = ModContent.Find<ModGore>("SummonersShine/SpiderEgg0").Type;
                SpiderEgg[1] = ModContent.Find<ModGore>("SummonersShine/SpiderEgg1").Type;
                SpiderCocoon = ModContent.Find<ModGore>("SummonersShine/SpiderCocoon").Type;
                Retinassembler = ModContent.Find<ModGore>("SummonersShine/Retinassembler").Type;
                Retinassembler_Lights = ModContent.Find<ModGore>("SummonersShine/Retinassembler_Lights").Type;
                Spazmatiassembler = ModContent.Find<ModGore>("SummonersShine/Spazmatiassembler").Type;
                Spazmatiassembler_Lights = ModContent.Find<ModGore>("SummonersShine/Spazmatiassembler_Lights").Type;
                SightSoul = ModContent.Find<ModDust>("SummonersShine/SightSoul").Type;
                RavenFeather = ModContent.Find<ModDust>("SummonersShine/RavenFeather").Type;
                FinchFeather = ModContent.Find<ModDust>("SummonersShine/FinchFeather").Type;
                ImpSigil = ModContent.Find<ModGore>("SummonersShine/ImpSigil").Type;
                ChiunStar = ModContent.Find<ModGore>("SummonersShine/ChiunStar").Type;
                Lifesteal_Sanguine = ModContent.Find<ModDust>("SummonersShine/Lifesteal_Sanguine").Type;
                DD2Hammer = ModContent.Find<ModDust>("SummonersShine/DD2Hammer").Type;

                PoisonPuff = ModContent.Find<ModGore>("SummonersShine/PoisonPuff").Type;
                PoisonPuffSmall = ModContent.Find<ModGore>("SummonersShine/PoisonPuffSmall").Type;
            }
        }
        public static void DrawLineWithParticles(Vector2 start, Vector2 end, int[] particleIDs, int particleCount, Action<Dust> OnCreate = null)
        {
            for (int x = 0; x < particleCount; x++)
            {
                float lerpVal = particleCount != 0 ? x / (float)(particleCount - 1) : 0;
                Vector2 result = Vector2.Lerp(start, end, lerpVal);
                Dust particle = Main.dust[Dust.NewDust(result, 0, 0, particleIDs[Main.rand.Next(particleIDs.Length)])];
                particle.velocity *= 0.01f;
                particle.noGravity = true;
                if (OnCreate != null)
                {
                    OnCreate(particle);
                }
            }
        }
        public static void DrawArcWithParticles(Vector2 center, float startAngle, float angleToTurn, float startLength, float endLength, int[] particleIDs, int particleCount, Action<Dust> OnCreate = null)
        {
            if (particleCount <= 1) {
                SummonersShine.logger.Error("Tried to pass 1 into particleCount! Just spawn it yourself...");
                return;
            }
            for (int x = 0; x < particleCount; x++)
            {
                float ratio = (float)x / (particleCount - 1);
                float len = (endLength - startLength) * ratio + startLength;
                Vector2 result = center + new Vector2(0, len).RotatedBy(startAngle + angleToTurn * ratio);
                Dust particle = Main.dust[Dust.NewDust(result, 0, 0, particleIDs[Main.rand.Next(particleIDs.Length)])];
                if (OnCreate != null) {
                    OnCreate(particle);
                }
            }
        }
    }
}
