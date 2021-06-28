using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    public class AnimatorSystem : MonoBehaviour
    {
        string currentAnimString;

        Animator anim;
        SpriteRenderer sr;

        public void Init()
        {
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
        }

        public void ChangeAnimationState(string newAnimString, bool flipX = false)
        {
            if (newAnimString == currentAnimString && sr.flipX == flipX)
                return;

            anim.CrossFade(newAnimString,0.00f);
            currentAnimString = newAnimString;
            FlipSprite(flipX);
        }

        void FlipSprite(bool value)
        {
            sr.flipX = value;
        }
    }

    public class PlayerAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string run = "Run";
        public string jump = "Jump";
        public string fall = "Fall";
        public string hit = "Hit";
        public string doubleJump = "Double Jump";
        public string wallSlide = "Wall Slide";
        public string appear = "Appear";
    }

    public class TrampolineAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string triggered = "Triggered";
    }

    public class FanAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string triggered = "Triggered";
    }

    public class ArrowAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string triggered = "Triggered";
        public string appear = "Appear";
    }

    public class FallingPlatformAnimatorSystem : AnimatorSystem
    {
        public string on = "On";
        public string off = "Off";
        public string fallDelay = "Fall Delay";
    }

    public class WaypointAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string noFlag = "No Flag";
        public string flagOut = "Flag Out";
    }

    public class SawAnimatorSystem : AnimatorSystem
    {
        public string rotateLeft = "FullLeft";
        public string rotateRight = "FullRight";
        public string idle = "Idle";
    }

    public class FruitAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string collected = "Collected";
    }

    public class BlockAnimatorSystem : AnimatorSystem
    {
        public string triggered = "Triggered";
        public string idle = "Idle";
    }

    public class EndWaypointAnimtorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string triggered = "Triggered";
    }

    public class FireAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string shoot = "Shoot";
    }

    public class SpikeHeadAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string bottomHit = "Bottom Hit";
        public string topHit = "Top Hit";
        public string leftHit = "Left Hit";
        public string rightHit = "Right Hit";
        public string blink = "Blink";

    }

    public class MovingPlatformAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string on = "On";
    }

    public class PlantAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string shoot = "Shoot";
        public string hit = "Hit";
    }

    public class MushroomAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string run = "Run";
        public string hit = "Hit";
    }

    public class TurtleAnimatorSystem : AnimatorSystem
    {
        public string idleSpikesIn = "Idle Spikes In";
        public string idleSpikesOut = "Idle Spikes Out";
        public string spikesIn = "Spikes In";
        public string spikesOut = "Spikes Out";
        public string hit = "Hit";
    }

    public class CharmeleonAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string attack = "Attack";
        public string run = "Run";
        public string hit = "Hit";
    }

    public class BluebirdAnimatorSystem : AnimatorSystem
    {
        public string flying = "Flying";
        public string hit = "Hit";
    }

    public class SkullAnimatorSystem : AnimatorSystem
    {
        public string idle = "Idle";
        public string idleEnrage = "Idle Enrage";
        public string enrage = "Enrage";
        public string derage = "Derage";
        public string hit = "Hit";
    }

}

