using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.SpecialData
{
    public class PygmyPlatformCollection : SpecialDataBase
    {
        public List<Projectile> pygmiesCasted = new();

        int[] randArray = new int[8];
        int randArrayPos = 7;

        void singleShuffle(int cur, int max, int offset) {
            int pos = Main.rand.Next(cur, max);
            cur += offset;
            pos += offset;
            int temp = randArray[cur];
            randArray[cur] = randArray[pos];
            randArray[pos] = temp;
        }
        void resetRandArray(int last)
        {
            for (int x = 0; x < 4; x++) {
                if (Main.rand.NextBool())
                {
                    randArray[x] = x * 2;
                    randArray[x + 4] = x * 2 + 1;
                }
                else
                {
                    randArray[x] = x * 2 + 1;
                    randArray[x + 4] = x * 2;
                }
            }

            for (int x = 0; x < 4; x++)
            {
                singleShuffle(x, 4, 0);
                singleShuffle(x, 4, 4);
            }

            if((last - last % 2) == (randArray[0] - randArray[0] % 2))
            {
                int temp = randArray[0];
                randArray[0] = randArray[3];
                randArray[3] = temp;
            }
            if ((randArray[3] - randArray[3] % 2) == (randArray[4] - randArray[4] % 2))
            {
                int temp = randArray[4];
                randArray[4] = randArray[7];
                randArray[7] = temp;
            }

        }
        int getGroundType() {
            randArrayPos++;
            if (randArrayPos == 8)
            {
                resetRandArray(randArray[7]);
                randArrayPos = 0;
            }
            return randArray[randArrayPos];
        }

        public List<Projectile> GetPygmyFromValidList(List<Projectile> validPygmies) {
            if (validPygmies.Count == 0)
                return validPygmies;
            Projectile rv = validPygmies.Find(i => !pygmiesCasted.Contains(i));
            if (rv == null) {
                rv = pygmiesCasted[0];
                pygmiesCasted.RemoveAt(0);
            }
            pygmiesCasted.Add(rv);
            validPygmies.Clear();
            validPygmies.Add(rv);
            rv.GetGlobalProjectile<ReworkMinion_Projectile>().GetMinionProjData().castingSpecialAbilityType = getGroundType();
            return validPygmies;
        }

        public void DeletePygmy(Projectile pygmy) {
            pygmiesCasted.Remove(pygmy);
        }
    }
}
