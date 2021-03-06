using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MalnourishedMania
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerAnimatorSystem))]
    public class PlayerManager : MonoBehaviour
    {
        #region Variables
        [Header("Gameplay Variables")]
        [SerializeField] float traumaOnHit = 0.3f;

        //Jump stuff
        [SerializeField] float maxJumpHeight = 2.5f;
        [SerializeField] float minJumpHeight = 1;
        [SerializeField] float timeToJumpApex = 0.3f;
        [SerializeField] float coyoteTime = 0.5f;

        //Run stuff
        [SerializeField] float accelerationTimeInAir = 0.2f;
        [SerializeField] float acceelerationTimeGrounded = 0.1f;
        [SerializeField] float moveSpeed = 6;

        //Wall stuff
        [SerializeField] float wallSlideSpeedMax = 3;
        [SerializeField] float wallStickTime = 0.25f;
        [SerializeField] float wallJumpDelay = 0.25f;
        [SerializeField] Vector2 wallJumpClimb;
        [SerializeField] Vector2 wallJumpOff;
        [SerializeField] Vector2 wallLeap;

        //Audio
        [Header("Audio")]
        [SerializeField] AudioClip jumpClip;
        [SerializeField] AudioClip hitClip;
        [SerializeField] AudioClip dissapearClip;

        float gravity;
        float maxJumpVelocity;
        float minJumpVelocity;
        float velocityXSmoothing;

        // Coyote time
        float elapsedCoyoteTime = 0.0f;
        bool onGroundLastFrame = false;
        bool triggerCoyoteTime = false;

        // Wall variables
        float timeToWallUnstick;
        bool wallSliding = false;
        int wallDirX;
        float wallJumpDelayTimer = 0;

        int lastFacingDirection = 1;

        bool velocityExternallyModified = false;
        bool canDoubleJump = true;
        bool inAir = false;

        [HideInInspector] public bool hit = false;

        Vector3 velocity = Vector3.zero;

        // Particle variables
        Vector3 particlesAtFeetOffset = new Vector3(0, -0.5f, -1);
        Vector3 particleWallJumpTowardsLeftOffset = new Vector3(0.289f, -0.44f, -1);
        Vector3 particleWallJumpTowardsLeftRotation = new Vector3(0, 0, 45);
        Vector3 particleWallJumpTowardsRightOffset = new Vector3(-0.289f, -0.44f, -1);
        Vector3 particleWallJumpTowardsRightRotation = new Vector3(0, 0, -45);

        // Components
        [Header("Components")]
        PlayerController playerController;
        PlayerAnimatorSystem playerAnimatorSystem;
        AudioSource audioSource;
        [SerializeField] GameObject runningParticles;
        [HideInInspector] public PlayerInput playerInput;
        #endregion

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            InitAnimatorSystem();
            audioSource = GetComponent<AudioSource>();
            InitJumpValues();
        }

        private void InitAnimatorSystem()
        {
            playerAnimatorSystem = gameObject.AddComponent<PlayerAnimatorSystem>();
            playerAnimatorSystem.Init();
        }

        private void Update()
        {
            HandlePauseMenu();

            if (hit || FindObjectOfType<GameUIMaster>() != null && FindObjectOfType<GameUIMaster>().state != GameUIState.ingame)
                return;

            CalculateVelocity();
            playerController.Move(velocity * Time.deltaTime);
            AnimateSprite();
            HandleLandingParticles();
        }

        private void HandlePauseMenu()
        {
            if (playerInput.PauseInputIsPressed())
            {
                if (FindObjectOfType<GameUIMaster>().state == GameUIState.ingame)
                    FindObjectOfType<GameUIMaster>().PauseGame();
                else if (FindObjectOfType<GameUIMaster>().state == GameUIState.paused)
                    FindObjectOfType<GameUIMaster>().ResumeGame();
            }
        }

        #region Health 
        public void Hit()
        {
            if (hit)
                return;

            velocity = Vector2.zero;
            velocityExternallyModified = true;

            playerAnimatorSystem.ChangeAnimationState(playerAnimatorSystem.hit, GetDirection() == -1 ? true : false);
            hit = true;

            Invoke("RenableGameObjects", 1.0f);

            if (FindObjectOfType<CameraShake>())
                FindObjectOfType<CameraShake>().Trauma += 0.5f;

            if (FindObjectOfType<BackgroundFlash>())
                FindObjectOfType<BackgroundFlash>().Flash();

            //gameObject.layer = 2; //ignore raycast

        }

        void RenableGameObjects()
        {
            Debug.Log("reneabled");

            if (FindObjectOfType<MonoLevelManager>() != null)
            {
                FindObjectOfType<MonoLevelManager>().DropFruit();
                FindObjectOfType<MonoLevelManager>().ReenableCreatures();
            }

        }

        public void Appear()
        {
            FindObjectOfType<MonoLevelManager>().TransitionToWaypoint();
            playerAnimatorSystem.ChangeAnimationState(playerAnimatorSystem.appear, GetDirection() == -1 ? true : false);
        }

        public void EndAppearAnimation()
        {
            hit = false;
            playerAnimatorSystem.ChangeAnimationState(playerAnimatorSystem.idle, GetDirection() == -1 ? true : false);
            velocity = Vector2.zero;
            velocityExternallyModified = true;
            //gameObject.layer = 11; //player
        }

        #endregion

        #region Animation
        private void AnimateSprite()
        {
            if (playerController.collisions.below)
            {
                if (playerInput.GetHorizontalInput() != 0)
                {
                    bool flipSprite = GetDirection() == 1 ? false : true;
                    playerAnimatorSystem.ChangeAnimationState(playerAnimatorSystem.run, flipSprite);
                }
                else
                {
                    bool flipSprite = GetDirection() == 1 ? false : true;
                    playerAnimatorSystem.ChangeAnimationState(playerAnimatorSystem.idle, flipSprite);
                }
            }
            else if (wallSliding)
            {
                bool flipSprite = playerController.collisions.left ? true : false;
                playerAnimatorSystem.ChangeAnimationState(playerAnimatorSystem.wallSlide, flipSprite);
            }
            else //in air
            {
                if (velocity.y >= 0) //rising
                {
                    bool flipSprite = GetDirection() == 1 ? false : true;
                    playerAnimatorSystem.ChangeAnimationState(playerAnimatorSystem.jump, flipSprite);
                }
                else //falling
                {
                    bool flipSprite = GetDirection() == 1 ? false : true;
                    playerAnimatorSystem.ChangeAnimationState(playerAnimatorSystem.fall, flipSprite);
                }
            }
        }
        #endregion

        #region Calculating velocity

        void InitJumpValues()
        {
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        }

        private void CalculateVelocity()
        {
            wallDirX = playerController.collisions.left ? -1 : 1;

            float targetVelocityX = playerInput.GetHorizontalInput() * moveSpeed;

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, ((playerController.collisions.below) ? acceelerationTimeGrounded : accelerationTimeInAir));

            wallSliding = false;
            if (OnWall() && PlayerWantsToSlideOnWall())
            {
                wallJumpDelayTimer = 0;
                wallSliding = true;
                AddWallSlideVelocity();
            }
            else
                wallJumpDelayTimer += Time.deltaTime;

            if ((playerController.collisions.above || playerController.collisions.below) && !velocityExternallyModified)
            {
                velocity.y = 0;
            }

            canDoubleJump = playerController.collisions.below && !wallSliding ? true : canDoubleJump;

            if (wallSliding)
                velocity.x = 0;

            HandleGroundCoyoteTime();

            if (playerInput.GetJumpInput())
            {
                AddJumpVelocity();
            }
            if (playerInput.IsJumpInputKeyUp())
            {
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }

            velocity.y += gravity * Time.deltaTime;
            velocityExternallyModified = false;
        }

        private void HandleGroundCoyoteTime()
        {
            if (!playerController.collisions.below && onGroundLastFrame && velocity.y <= 0.0f)
            {
                triggerCoyoteTime = true;
            }
            else if(!onGroundLastFrame && playerController.collisions.below)
            {
                ResetCoyoteTime();
            }

            if (triggerCoyoteTime)
            {
                elapsedCoyoteTime += Time.deltaTime;
            }

            onGroundLastFrame = playerController.collisions.below;
        }

        private void AddJumpVelocity()
        {
            if (wallJumpDelayTimer <= wallJumpDelay)
            {
                if (wallDirX == Mathf.Sign(playerInput.GetHorizontalInput()) && Mathf.Abs(playerInput.GetHorizontalInput()) > 0.5f)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                    BurstWallJumpParticles();
                    PlayJumpAudio();
                }
                else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                    BurstWallJumpParticles();
                    PlayJumpAudio();
                }

                //Setting double jump off for a short period of time so he doesn't immdediatley double jump and bug out
                canDoubleJump = false;
                Invoke("ResetDoubleJump", 0.1f);
            }
            else if (playerController.collisions.below || WithinCoyoteTime())
            {
                velocity.y = maxJumpVelocity;
                PlayJumpAudio();
                ResetCoyoteTime();
            }
            else if (canDoubleJump)
            {
                velocity.y = maxJumpVelocity;
                canDoubleJump = false;
                BurstDoubleJumpParticles();
                PlayJumpAudio();
            }
        }

        private void AddWallSlideVelocity()
        {
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (Mathf.Sign(playerInput.GetHorizontalInput()) != wallDirX && Mathf.Abs(playerInput.GetHorizontalInput()) > 0.5f)
                    timeToWallUnstick -= Time.deltaTime;
                else
                {
                    timeToWallUnstick = 0;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        private bool OnWall()
        {
            return (playerController.collisions.left || playerController.collisions.right) && !playerController.collisions.below && velocity.y < 0;
        }

        bool PlayerWantsToSlideOnWall()
        {
            return Mathf.Sign(playerInput.GetHorizontalInput()) == Mathf.Sign(wallDirX) && Mathf.Abs(playerInput.GetHorizontalInput()) > 0.5f;
        }

        public bool PlayerWantsToDropThroughPlatform()
        {
            return playerInput.GetVerticalInput() == -1;
        }

        bool WithinCoyoteTime()
        {
            if (!triggerCoyoteTime)
                return false;
            return elapsedCoyoteTime <= coyoteTime;
        }

        void ResetCoyoteTime()
        {
            elapsedCoyoteTime = 0.0f;
            triggerCoyoteTime = false;
        }

        public void ResetDoubleJump()
        {
            canDoubleJump = true;
        }

        public void AddVelocity(Vector3 vel)
        {
            velocity += vel;
            velocityExternallyModified = true;
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

        #endregion

        #region Facing direction
        private int GetDirection()
        {
            if (Mathf.Abs(playerInput.GetHorizontalInput()) > 0.5f)
            {
                int newDirection = (int)playerInput.GetHorizontalInput();
                SetLastFacingDirection(newDirection);
                return newDirection;
            }

            return lastFacingDirection;
        }

        private void SetLastFacingDirection(int newDirection)
        {
            lastFacingDirection = newDirection;
        }
        #endregion

        #region Particles
        public void PlayFrontLegParticles()
        {
            if (!GetComponent<SpriteRenderer>().flipX)
            {
                runningParticles.transform.localPosition = new Vector3(-0.204f, -0.396f, 0f);
                runningParticles.transform.localEulerAngles = new Vector3(0, 0, 75f);
            }
            else
            {
                runningParticles.transform.localPosition = new Vector3(0.204f, -0.396f, 0f);
                runningParticles.transform.localEulerAngles = new Vector3(0, 0, -75f);
            }

            runningParticles.GetComponent<ParticleSystem>().Play();
        }

        public void PlayBackLegParticles()
        {
            if (!GetComponent<SpriteRenderer>().flipX)
            {
                runningParticles.transform.localPosition = new Vector3(-0.204f, -0.396f, 0f);
                runningParticles.transform.localEulerAngles = new Vector3(0, 0, 75f);
            }
            else
            {
                runningParticles.transform.localPosition = new Vector3(0.204f, -0.396f, 0f);
                runningParticles.transform.localEulerAngles = new Vector3(0, 0, -75f);
            }

            runningParticles.GetComponent<ParticleSystem>().Play();

        }

        private void HandleLandingParticles()
        {
            if (inAir && playerController.collisions.below)
                PlayLandingParticles();
            inAir = !playerController.collisions.below;
        }

        void BurstWallSlideParticles()
        {

        }

        public void BurstDoubleJumpParticles()
        {
            GameObject landingParticlesGameObject = GameManager.GetObjectPooler().RequestObject("Double Jump Particles");
            landingParticlesGameObject.GetComponent<ParticleSystem>().Stop();
            landingParticlesGameObject.transform.position = transform.position + particlesAtFeetOffset;
            landingParticlesGameObject.GetComponent<ParticleSystem>().Play();
        }

        public void BurstWallJumpParticles()
        {
            GameObject landingParticlesGameObject = GameManager.GetObjectPooler().RequestObject("Wall Jump Particles");
            landingParticlesGameObject.GetComponent<ParticleSystem>().Stop();

            if (playerController.collisions.faceDir == 1)
            {
                landingParticlesGameObject.transform.position = transform.position + particleWallJumpTowardsLeftOffset;
                landingParticlesGameObject.transform.localEulerAngles = particleWallJumpTowardsLeftRotation;
            }
            else
            {
                landingParticlesGameObject.transform.position = transform.position + particleWallJumpTowardsRightOffset;
                landingParticlesGameObject.transform.localEulerAngles = particleWallJumpTowardsRightRotation;
            }

            landingParticlesGameObject.GetComponent<ParticleSystem>().Play();
        }

        public void PlayLandingParticles()
        {
            GameObject landingParticlesGameObject = GameManager.GetObjectPooler().RequestObject("Landing Particles");
            landingParticlesGameObject.GetComponent<ParticleSystem>().Stop();
            landingParticlesGameObject.transform.position = transform.position + particlesAtFeetOffset;
            landingParticlesGameObject.GetComponent<ParticleSystem>().Play();
        }
        #endregion

        #region Audio  

        void PlayJumpAudio()
        {
            audioSource.Stop();
            audioSource.clip = jumpClip;
            audioSource.Play();
        }

        void PlayHitAudio()
        {
            audioSource.Stop();
            audioSource.clip = hitClip;
            audioSource.Play();
        }

        public void PlayDissapearAudio()
        {
            audioSource.Stop();
            audioSource.clip = dissapearClip;
            audioSource.Play();
        }

        #endregion
    }
}
