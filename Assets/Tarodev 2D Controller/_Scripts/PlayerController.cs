using System.Collections;
using System;
using UnityEngine;
using static System.TimeZoneInfo;

namespace TarodevController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;
        [SerializeField]

        private bool _facingRight = true;

        // Respawn related variables
        public Transform respawnPoint;
        public float deathYLevel = -10f;
        public Vector3 cameraOffset; // Offset per la posizione della camera
        public float respawnDelay = 1f; // Tempo di ritardo prima del respawn
        private bool _isRespawning; // Variabile per tenere traccia dello stato di respawn

        // Audio variables
        public AudioClip jumpSound;
        public AudioClip collectibleSound;
        public AudioClip useCollectibleSound;
        public AudioClip respawnSound;
        private AudioSource audioSource;



        // Double jump related variables
        [SerializeField] private int _jumpsRemaining;
        [SerializeField] private int _maxJumps = 2;

        private LightController lightController;

        public Animator animator;

        public Animator transitionAnimator; // Assegna l'animatore del panel nel Canvas
        public float transitionTime = 0f;

        //nemico inseguitore
        public PlayerPath playerPath; // Riferimento allo script del percorso del personaggio
        public EnemyFollowPath enemyFollowPath; // Riferimento allo script del nemico

        public Miniboss miniboss;


        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            _col = GetComponent<CapsuleCollider2D>();


            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

            _jumpsRemaining = _maxJumps;

            lightController = GetComponentInChildren<LightController>();

            audioSource = GetComponent<AudioSource>();



        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            if (_isRespawning) return; // Ignora l'input se il giocatore � in respawn


            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }

        private void FixedUpdate()
        {

            if (_isRespawning) return;

            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();

            ApplyMovement();

            CheckForDeath();
        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {

            // Parametri del cast
            Vector2 origin = _col.bounds.center;
            Vector2 size = _col.size;
            CapsuleDirection2D direction = _col.direction;
            float angle = 0;
            float distance = _stats.GrounderDistance;
            int layerMask = ~_stats.PlayerLayer;

            Physics2D.queriesStartInColliders = false;

            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            Debug.DrawRay(origin, Vector2.down * distance, groundHit ? Color.green : Color.red);
            Debug.DrawRay(origin, Vector2.up * distance, ceilingHit ? Color.green : Color.red);


            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            if (!_grounded && groundHit)
            {
                _grounded = true;
                _jumpsRemaining = _maxJumps; // Reset dei salti quando il giocatore � a terra
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
                animator.SetTrigger("isGrounded");
                animator.SetBool("isGround", _grounded);
            }
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
                animator.SetBool("isGround", !_grounded);


                // Mantieni il conteggio dei salti rimanenti quando si lascia il terreno
                if (_jumpsRemaining == _maxJumps)
                {
                    _jumpsRemaining = _maxJumps - 1; // Se non � stato utilizzato nessun salto
                }
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }


        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote || _jumpsRemaining > 0)
            {
                ExecuteJump();
                animator.SetTrigger("Jump");
                audioSource.PlayOneShot(jumpSound); // Riproduci il suono del salto

            }



            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;



            if (_grounded || CanUseCoyote)
            {
                _coyoteUsable = false;
                _jumpsRemaining = _maxJumps - 1; // Reset dei salti rimanenti quando si utilizza il coyote time o si salta dal terreno
            }

            
            else
            {
                _jumpsRemaining--; // Decrementa il conteggio dei salti solo se si � in aria
            }

            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
            //animator.SetTrigger("Jump"); // Imposta il trigger Jump per avviare l'animazione di salto

        }


        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
                animator.SetBool("isRunning", false);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
                animator.SetBool("isRunning", true);

                // Flip the character to face the direction of movement
                if (_frameInput.Move.x > 0 && !_facingRight)
                {
                    Flip();
                }
                else if (_frameInput.Move.x < 0 && _facingRight)
                {
                    Flip();
                }
            }
        }

        private void Flip()
        {
            _facingRight = !_facingRight;
            Vector3 rotation = transform.eulerAngles;
            rotation.y += 180f;
            transform.eulerAngles = rotation;
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Respawn

        private void CheckForDeath()
        {
            if (transform.position.y < deathYLevel)
            {
                Respawn();
            }
        }

        public void SetRespawnPoint(Transform newRespawnPoint)
        {
            respawnPoint = newRespawnPoint;
        }

        public void SetDeathYLevel(float newDeathYLevel)
        {
            deathYLevel = newDeathYLevel;
        }

        public void Respawn()
        {
            Debug.Log("Respawn chiamata da: " + Environment.StackTrace);


            StartCoroutine(RespawnRoutine());

            //transitionAnimator.SetTrigger("FadeOut");
            //transitionAnimator.SetTrigger("FadeIn");
            //transitionAnimator.SetTrigger("Normal");

            playerPath.ResetPath();
            enemyFollowPath.ResetEnemy();


            transform.position = respawnPoint.position;
            _rb.velocity = Vector2.zero;
            _frameVelocity = Vector2.zero;
            _jumpsRemaining = _maxJumps; // Reset dei salti al respawn
            lightController.ResetLightAfterRespawn();
            collectibleCount = 0;
            Collectible.CollectiblesReappear();
            FallingPlatformManager.ResetAllPlatforms();
            miniboss.ResetHealth();


        }

        private IEnumerator RespawnRoutine()
        {
            audioSource.PlayOneShot(respawnSound); // Riproduci il suono del respawn

            _isRespawning = true; // Inizia il respawn
            // Avvia la transizione di fade out
            transitionAnimator.SetTrigger("FadeOut");

            // Aspetta che la transizione sia completata
            //yield return new WaitForSeconds(transitionTime); 

            

            // Respawna il personaggio

            // Avvia la transizione di fade in
            transitionAnimator.SetTrigger("FadeIn");
            // Aspetta che la transizione sia completata
            yield return new WaitForSeconds(transitionTime);

            transitionAnimator.SetTrigger("Normal");
            

            yield return new WaitForSeconds(respawnDelay);
            //transform.position = respawnPoint.position;
            yield return new WaitForSeconds(respawnDelay);
            _isRespawning = false; // Termina il respawn


        }

        #endregion

        #region Collectibles

        public static event Action OnCollected;

        [SerializeField] private int collectibleCount = 0;

        public void CollectibleCollected()
        {
            collectibleCount++;
            OnCollected?.Invoke();
            audioSource.PlayOneShot(collectibleSound); // Riproduci il suono del collezionabile raccolto

        }

        public bool UseCollectible()
        {
            if (collectibleCount > 0)
            {
                Debug.Log("Riduci");
                collectibleCount--;
                audioSource.PlayOneShot(useCollectibleSound); // Riproduci il suono del collezionabile utilizzato

                return true;
            }
            return false;
        }

        public bool HasCollectibles() 
        {
            return collectibleCount > 0;
        }

        #endregion

        //private void ApplyMovement() => _rb.velocity = _frameVelocity;
        private void ApplyMovement()
        {
            if (_isRespawning) return; // Ignora il movimento se il giocatore � in respawn
            _rb.velocity = _frameVelocity;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}
