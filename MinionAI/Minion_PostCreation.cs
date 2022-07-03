using SummonersShine.SpecialData;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace SummonersShine.MinionAI
{
    public static class Minion_PostCreation
    {
        public static void InitMiniMinion<T, U>(Projectile projectile, ReworkMinion_Projectile projFuncs, ReworkMinion_Player playerFuncs) where T : miniMinionStat, new() where U : SpecialDataBase, IMiniMinionStatCollection<T>, new()
        {

            T stats = projFuncs.GetSpecialData<T>();
            stats.init(projectile, projFuncs);
            U collection = playerFuncs.GetSpecialData<U>();
            collection.Add(stats);
        }

        public static void InitMegaMinion<T, U>(Projectile projectile, ReworkMinion_Projectile projFuncs, ReworkMinion_Player playerFuncs) where T : miniMinionStat, new() where U : miniMinionStatCollection, new()
        {
            MinionProjectileData projData = projFuncs.GetMinionProjData();

            U collection = playerFuncs.GetSpecialData<U>();
            collection.InitMegaMinion(projectile, projFuncs, projData);
        }
        public static void OnPostCreation(Projectile projectile, Player owner)
        {
            ReworkMinion_Player playerFuncs = owner.GetModPlayer<ReworkMinion_Player>();
            ReworkMinion_Projectile projFuncs = projectile.GetGlobalProjectile<ReworkMinion_Projectile>();
            switch (projectile.type)
            {
                case ProjectileID.SoulscourgePirate:
                case ProjectileID.PirateCaptain:
                case ProjectileID.OneEyedPirate:
                    InitMiniMinion<PirateStat, PirateStatCollection>(projectile, projFuncs, playerFuncs);
                    break;
                case ProjectileID.StormTigerGem:
                    InitMiniMinion<DesertTigerStat, DesertTigerStatCollection>(projectile, projFuncs, playerFuncs);
                    break;
                case ProjectileID.StormTigerTier1:
                case ProjectileID.StormTigerTier2:
                case ProjectileID.StormTigerTier3:
                    InitMegaMinion<DesertTigerStat, DesertTigerStatCollection>(projectile, projFuncs, playerFuncs);
                    break;
                case ProjectileID.AbigailCounter:
                    InitMiniMinion<AbigailCounterStat, AbigailStatCollection>(projectile, projFuncs, playerFuncs);
                    break;
                case ProjectileID.AbigailMinion:
                    InitMegaMinion<AbigailCounterStat, AbigailStatCollection>(projectile, projFuncs, playerFuncs);
                    break;
                case ProjectileID.HoundiusShootius:
                    InitMiniMinion<HoundiusShootiusStat, HoundiusShootiusStatCollection>(projectile, projFuncs, playerFuncs);
                    break;
                case ProjectileID.StardustDragon1:
                case ProjectileID.StardustDragon2:
                case ProjectileID.StardustDragon3:
                case ProjectileID.StardustDragon4:
                    InitMiniMinion<StardustDragonStat, StardustDragonStatCollection>(projectile, projFuncs, playerFuncs);
                    break;

                    /*case ProjectileID.StardustDragon2:
                        //attach head to body
                        Projectile head = Main.projectile[(int)projectile.ai[0]];
                        ModUtils.SetStardustDragonChild(head, projectile);
                        break;
                    case ProjectileID.StardustDragon3:
                        //updates the tail's attack speed
                        head = Main.projectile[(int)projectile.ai[0]];
                        ModUtils.SetStardustDragonChild(head, projectile);
                        Projectile tail = Main.projectile[(int)projectile.localAI[1]];
                        if (tail.type == ProjectileID.StardustDragon4) {
                            ModUtils.SetStardustDragonChild(projectile, tail);
                            ModUtils.SetStardustDragonChild(tail, null);
                        }
                        tail.GetGlobalProjectile<ReworkMinion_Projectile>().MinionASMod = projFuncs.MinionASMod;
                        break;*/
            }
        }
    }
}
