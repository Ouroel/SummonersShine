using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SummonersShine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SummonersShine
{
    public class EnergyDisplay : UIState
    {
        public static List<EnergyDisplay> AllDisplays;

        public Player player;
        int lifeTimeStart;
        int lifeTime;
        List<Tuple<int, bool, bool>> displayOrder = new();
        Tuple<int, bool, bool> displayData;
        public class EnergyDisplay_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                AllDisplays = new();
            }

            public void Unload()
            {
                AllDisplays = null;
            }
        }

        public EnergyDisplay(Player anchor, int time = 180) {
            player = anchor;
            lifeTimeStart = time;
            lifeTime = time;
            AllDisplays.Add(this);
        }

        public static void DrawAll() {
            AllDisplays.RemoveAll(i => i.player == null || !i.player.active);
            AllDisplays.ForEach(i => i.Draw(Main.spriteBatch));
        }
        public bool Update() {
            lifeTime--;
            bool rv = lifeTime == 0;
            if (rv)
                AllDisplays.Remove(this);
            return rv;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D value = ModTextures.ThoughtBubble;

            Player entity = player;

            Vector2 vector = new Vector2(entity.Top.X, entity.VisualPosition.Y) + new Vector2(entity.width * 2f, -16f) - Main.screenPosition;
            SpriteEffects effect = SpriteEffects.None;

            if (lifeTime == 3 && displayOrder.Count > 0) {
                displayOrder.RemoveAt(0);
                if (displayOrder.Count > 0)
                {
                    lifeTime = lifeTimeStart - 3;
                    displayData = displayOrder.First();
                }
            }

            int bubbleOffset = (lifeTime - 6) % 16 < 8 ? 2 : 1;
            if (lifeTime < 6 || lifeTimeStart - lifeTime < 6)
                bubbleOffset = 0;
            if (displayData.Item3) {
                bubbleOffset += 3;
            }
            Rectangle rectangle = value.Frame(6, 46, bubbleOffset, 0, 0, 0);
            if (!displayData.Item2)
            {
                bubbleOffset -= 3;
            }
            Vector2 origin = new Vector2(rectangle.Width * 0.5f, rectangle.Height);
            if (player.gravDir == -1f)
            {
                origin.Y = 0;
                effect |= SpriteEffects.FlipVertically;
                vector = Main.ReverseGravitySupport(vector, 0f);
            }
            spriteBatch.Draw(value, vector, new Rectangle?(rectangle), Color.White, 0f, origin, 1, effect, 0f);

            Tuple<Texture2D, Rectangle> minionTexture = null;
            MinionDataHandler.ModdedGetDisplayData.ForEach(x =>
            {
                if (minionTexture == null) minionTexture = x(displayData.Item1, bubbleOffset);
            });
            if (minionTexture == null)
                minionTexture = MinionDataHandler.GetEnergyThoughtTexture(displayData.Item1, bubbleOffset);

            if (minionTexture != null)
                spriteBatch.Draw(minionTexture.Item1, vector, new Rectangle?(minionTexture.Item2), Color.White, 0f, origin, 1, effect, 0f);
        }

        public void AddNewThought(int summonItemID, bool multiple, bool golden)
        {
            if(player.whoAmI == Main.myPlayer)
                SoundEngine.PlaySound(SoundID.MaxMana);
            Tuple<int, bool, bool> thought = displayOrder.Find(i => i.Item1 == summonItemID);
            if (thought == null)
            {
                PushThought(summonItemID, multiple, golden);
                return;
            }
            if (golden && !thought.Item3) {
                displayOrder.Remove(thought);
                PushThought(summonItemID, multiple, golden);
            }
        }

        void PushThought(int Item, bool multiple, bool golden) {
            Tuple<int, bool, bool> thought = new Tuple<int, bool, bool>(Item, multiple, golden);
            int insertIndex = -1;
            if(golden)
                insertIndex = displayOrder.FindIndex(i => !i.Item3);
            insertIndex = insertIndex != -1 ? insertIndex : displayOrder.Count;
            displayOrder.Insert(insertIndex, thought);
            int trueStartingPoint = lifeTimeStart - 3;
            if (insertIndex == 0) {
                lifeTime = lifeTime > trueStartingPoint ? lifeTime : trueStartingPoint;
                displayData = thought;
            }
        }
    }
}
