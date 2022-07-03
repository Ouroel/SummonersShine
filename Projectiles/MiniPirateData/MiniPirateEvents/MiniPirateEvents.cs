using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SummonersShine.Projectiles.MiniPirateData
{
    public partial class MiniPirate
    {
        public class MiniPirateEventWrapper_KickPirate
        {
            MiniPirateEvent_KickPirate wrapped;
            public MiniPirateEventWrapper_KickPirate(MiniPirate pirate)
            {
                wrapped = new();
                pirate.AddEvent(wrapped);
            }

            public void Add(Projectile projectile) {
                if (wrapped.entsToBoot.Contains(projectile))
                    return;
                wrapped.entsToBoot.Add(projectile);
            }
            public void Remove(Projectile projectile)
            {
                wrapped.entsToBoot.Remove(projectile);
            }
        }
        abstract class MiniPirateEvent
        {
            public virtual bool CanDestroy => true;
            public abstract bool CanHook(MiniPirate pirate, MiniPirateActions currentAction);

            public abstract MiniPirateActions HookEvent(MiniPirate pirate, MiniPirateActions currentAction);
        }

        class MiniPirateEvent_KickPirate : MiniPirateEvent
        {
            public List<Projectile> entsToBoot = new List<Projectile>();
            public override bool CanDestroy => false;
            public override bool CanHook(MiniPirate pirate, MiniPirateActions currentAction)
            {
                //prioritize bullet storm

                int targets = pirate.AnyTargets(new Vector2(0, -BulletStormHeight));
                if (pirate.attackCooldown <= 0 && targets != -1)
                {
                    return false;
                }

                return currentAction.Urgent <= 0 && entsToBoot.Count != 0;
            }

            public override MiniPirateActions HookEvent(MiniPirate pirate, MiniPirateActions currentAction)
            {
                int index = Main.rand.Next(0, entsToBoot.Count);
                MiniPirateActions_Blink next = new MiniPirateActions_Blink(pirate.desiredParentEntity, pirate.desiredRelativePosition, currentAction, 0, 20, 0);
                currentAction.ActionDuration = 0;
                MiniPirateActions transitionTarget = currentAction.TransitionTarget;
                if (transitionTarget != null)
                {
                    currentAction.ForceKill();
                    next.next = transitionTarget;
                }
                MiniPirateActions_Kick rv = new(entsToBoot[index], next, 10, 10);
                entsToBoot.RemoveAt(index);
                return rv;
            }
        }
    }
}
