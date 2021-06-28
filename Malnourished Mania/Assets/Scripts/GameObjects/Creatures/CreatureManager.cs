using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class CreatureManager : MonoBehaviour
    {
        public Vector3 velocity = Vector3.zero;

        protected bool velocityExternallyModified = false;
        protected bool canDoubleJump = true;
        public bool hit = false;
        protected bool inAir = false;

        public virtual void Hit() { }

        #region Velocity stuff
        public void MultVelocity(float mult)
        {
            velocity *= mult;
        }

        // PLAYER ONLY - DEAL WITH THIS? This is because sometimes the collision isn't detected on things like arrows and trampolines so it makes it more consistent
        public void ResetDoubleJump()
        {
            canDoubleJump = true;
        }

        public void SetVelocity(Vector2 vel)
        {
            velocity = vel;
            velocityExternallyModified = true;
        }

        public void SetVerticalVelocity(float vel, bool resetDoubleJump = true)
        {
            velocity.y = vel;
            velocityExternallyModified = true;

            if (resetDoubleJump)
                ResetDoubleJump();
        }

        public void AddVelocity(Vector3 vel)
        {
            velocity += vel;
            velocityExternallyModified = true;
        }
        #endregion
    }
}

