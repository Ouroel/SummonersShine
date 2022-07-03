using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Microsoft.Xna.Framework;

namespace SummonersShine.SpecialData
{
    public class MinionProjectileData : SpecialDataBase
	{

		public Entity moveTarget;
		/*public Entity lastTrueMoveTarget;
		public bool moveTargetTethered = false;
		public Vector2 moveTargetLastPosition;*/

		public float currentTick = 1;
		public float nextTicks = 1;

		public float lastSimRateInv = 1;
		public bool updatedSim = false;

		public bool isRealTick = true;

		public bool isTeleportFrame = false;

		//If tick would be skipped
		//public bool StoringVelocity = false;
		//public Vector2 lastVelocity = Vector2.Zero;

		//relativeVelocity
		public Vector2 lastRelativeVelocity = Vector2.Zero;
		public float minionTrackingAcceleration = 0;
		public float minionTrackingImperfection = 0;

		public MinionTracking_State trackingState = MinionTracking_State.noTracking;

		public MinionSpeedModifier minionSpeedModType = MinionSpeedModifier.normal;

		public int alphaOverride = -1;

		//energy

		public float energy = 0;
		public float maxEnergy = 0;
		public float energyRegenRate = 1f;
		public float energyRegenRateMult = 1;
		public int castingSpecialAbilityTime = -1;
		public NPC specialCastTarget = null;
		public Vector2 specialCastPosition = Vector2.Zero;
		public bool cancelSpecialNextFrame = false;

		public int actualMinionAttackTargetNPC = -1;

		public float minionFlickeringThreshold = 0f;

		public int castingSpecialAbilityType = 0; //stored info to send packets to others
	}
}
