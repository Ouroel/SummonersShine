using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SummonersShine
{
    public static class PseudoRandom
    {
        public const int WHIPSPECIAL_REACH_PRD_CONST = 25;

        public static double[] PseudoRandomCollection;

        public static double GetPseudoRandomRNG(int chance)
        {
            chance = Math.Clamp(chance, 0, 100);
            return PseudoRandomCollection[chance];
        }
        public class PseudoRandom_Loader : ILoadable
        {
            public void Load(Mod mod)
            {
                PseudoRandomCollection = new double[101];
                PseudoRandomCollection[0] = 0;
                for (int x = 1; x < 100; x++)
                {
                    PseudoRandomCollection[x] = CfromP(x * 0.01f);
                    /*float successPerc = 0;
                    double testy = 0;
                    for (int y = 0; y < 10000; y++)
                    {
                        testy += PseudoRandomCollection[x];
                        if(Terraria.Main.rand.NextDouble() < testy)
                        {
                            testy = 0;
                            successPerc++;
                        }
                    }
                    successPerc /= 100;
                    log4net.LogManager.GetLogger("PublicLogger").Info("PRNG: " + x + " SUCCESS: " + successPerc);*/
                }
                PseudoRandomCollection[100] = 1;
            }

            public void Unload()
            {
                PseudoRandomCollection = null;
            }
        }
        static double PfromC(double C)
        {
            double probOnThis = 1;
            double totalProb = 0;

            double currentProcCond = 0;

            while (currentProcCond < 1)
            {
                currentProcCond += C;
                totalProb += probOnThis;
                probOnThis *= (1 - currentProcCond);
            }

            return 1 / totalProb;
        }

        static double CfromP(double P)
        {
            double Cupper = P;
            double Clower = 0;
            double Cmid;

            double p1;
            double p2 = 1;

            while(true)
            {
                Cmid = (Cupper + Clower) / 2;
                p1 = PfromC(Cmid);
                if (Math.Abs(p1 - p2) <= 0)
                    break;
                if (p1 > P)
                    Cupper = Cmid;
                else if (p1 < P)
                    Clower = Cmid;
                else
                    return Cmid;

                p2 = p1;
            }
            return Cmid;
        }
    }
}
