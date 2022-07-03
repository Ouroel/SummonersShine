using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine.Textures
{
    public abstract class CustomProjectileDrawLayer
    {
        public static List<CustomProjectileDrawLayer> behindProjectiles;


        public static List<CustomProjectileDrawLayer> behindNPCs;
        public static List<CustomProjectileDrawLayer> abovePlayers;
        public Projectile proj;
        public int projID;

        public abstract void Draw(Projectile projectile);
        public abstract void Update(Projectile projectile);

        public int width;
        public int height;
        public Vector2 pos;

        public bool destroyThisNext = false;

        public Vector2 center
        {
            get
            {
                return pos + new Vector2(width * 0.5f, height * 0.5f);
            }
            set
            {
                pos = value - new Vector2(width * 0.5f, height * 0.5f);
            }
        }
        public class CustomPreDrawProjectile_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                behindNPCs = new();
                behindProjectiles = new();
                abovePlayers = new();
            }

            public void Unload()
            {
                behindNPCs = null;
                behindProjectiles = null;
                abovePlayers = null;
            }
        }
        public static void Clear()
        {
            behindNPCs.Clear();
            behindProjectiles.Clear();
            abovePlayers.Clear();
        }

        public static void DrawAll_BehindNPCs(Main main)
        {
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            DrawAll(main, behindNPCs);
            Main.spriteBatch.End();
        }
        public static void DrawAll_BehindProjectiles(Main main)
        {
            DrawAll(main, behindProjectiles);
        }
        public static void DrawAll_AbovePlayers(Main main)
        {
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            DrawAll(main, abovePlayers);
            Main.spriteBatch.End();
        }

        static void DrawAll(Main main, List<CustomProjectileDrawLayer> cache) {

            if (!Main.gamePaused)
            {
                cache.ForEach(i => i.Update());
                cache.RemoveAll(i => i.destroyThisNext);
            }
            cache.ForEach(i => i.Draw(main));

            Main.CurrentDrawnEntity = null;
            Main.CurrentDrawnEntityShader = 0;
        }

        public void Update() {

            if (proj != null && (!proj.active || proj.type != projID))
                proj = null;
            Update(proj);
        }
        public void Draw(Main main) {
            if ((int)Main.Camera.ScaledPosition.X - width > pos.X || (int)Main.Camera.ScaledPosition.Y - height > pos.Y || (int)Main.Camera.ScaledPosition.X + (int)Main.Camera.ScaledSize.X < pos.X || (int)Main.Camera.ScaledPosition.Y + (int)Main.Camera.ScaledSize.Y < pos.Y)
            {
                return;
            }

            int intendedShader = 0;
            if (proj == null)
            {
                intendedShader = GetIntendedShader_NoProj();
            }
            else
            {
                intendedShader = Main.GetProjectileDesiredShader(proj.whoAmI);
            }
            main.PrepareDrawnEntityDrawing(proj, intendedShader);
            Draw(proj);
        }

        public virtual int GetIntendedShader_NoProj() {
            return 0;
        }

        public CustomProjectileDrawLayer(Projectile proj, int width, int height, DrawType drawType = DrawType.BehindProjectiles)
        {
            this.proj = proj;
            if (proj != null)
                projID = proj.type;
            this.width = width;
            this.height = height;
            switch (drawType)
            {
                case DrawType.BehindProjectiles:
                    behindProjectiles.Add(this);
                    break;
                case DrawType.BehindNPCs:
                    behindNPCs.Add(this);
                    break;
                case DrawType.AbovePlayers:
                    abovePlayers.Add(this);
                    break;
            }
        }

        public void RemoveThis(DrawType drawType = DrawType.BehindProjectiles)
        {
            switch (drawType)
            {
                case DrawType.BehindProjectiles:
                    behindProjectiles.Remove(this);
                    break;
                case DrawType.BehindNPCs:
                    behindNPCs.Remove(this);
                    break;
                case DrawType.AbovePlayers:
                    abovePlayers.Remove(this);
                    break;
            }
        }
        public static CustomProjectileDrawLayer Find(Projectile projectile, DrawType drawType = DrawType.BehindProjectiles)
        {
            switch (drawType)
            {
                case DrawType.BehindProjectiles:
                    return behindProjectiles.Find(i => i.proj == projectile);
                case DrawType.BehindNPCs:
                    return behindNPCs.Find(i => i.proj == projectile);
                case DrawType.AbovePlayers:
                    return abovePlayers.Find(i => i.proj == projectile);
                default:
                    throw new ArgumentOutOfRangeException("Invalid DrawType!");
            }
        }
    }

    public enum DrawType
    {
        BehindProjectiles,
        BehindNPCs,
        AbovePlayers,
    }
}
