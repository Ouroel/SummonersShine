using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SummonersShine.SpecialData;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI;
using Terraria.ID;

namespace SummonersShine.VanillaMinionILMods
{
    public static partial class VanillaILModification
    {
        public static void DrawProjectiles_CustomPostDrawProjectiles(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchStsfld<Main>(nameof(Main.CurrentDrawnEntityShader))))
            {
                SummonersShine.logger.Error("[DrawProjectiles_CustomPostDrawProjectiles] i => i.MatchStsfld<Main>(\"CurrentDrawEntityShader\") not found!");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(CustomProjectileDrawLayer.DrawAll_BehindProjectiles);
        }

        private static void DoDraw_DrawNPCsOverTiles_CustomPostDrawProjectiles(On.Terraria.Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            CustomProjectileDrawLayer.DrawAll_BehindNPCs(self);
            orig(self);
        }
        private static void DoDraw_CustomPostDrawProjectiles(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdfld<Main>(nameof(Main.DrawCacheProjsOverPlayers))))
            {
                SummonersShine.logger.Error("[DoDraw_CustomPostDrawProjectiles] i => i.MatchLdfld<Main>(nameof(Main.DrawCacheProjsOverPlayers)) not found!");
                return;
            }
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate(CustomProjectileDrawLayer.DrawAll_AbovePlayers);
        }
    }
}
