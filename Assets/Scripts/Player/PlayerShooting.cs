using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// handles inputs for player shooting, determines if it is a valid time to shoot, and creates bullets
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    [Header("Bullet")]
    [Tooltip("stores prefab of bullet object")] public GameObject BulletObject;

    [Header("Trajectory")]
    [SerializeField, Tooltip("origin of bullets being fired")] private Transform _gunPosition;
    [SerializeField, Tooltip("max ranged of the shootcast from the camera to detect what the target position is")] private float _maxShootCastRange = 100f;

    [Header("Raycast Exclusions")]
    [SerializeField, Tooltip("Layer that the raycast should not hit")] private LayerMask _ignoreMask;

    [Header("Input Buffer")]
    [SerializeField, Tooltip("Window within which input buffer will store input to check for cooldown")] private float _inputBuffer = 0.25f;
    private float _bufferTimer;

    public float PlayerAngleOffset { get; private set; } = 0;

    private PlayerController _playerController;

    float _timer;

    public delegate void OnBulletFire();
    public static event OnBulletFire onBulletFire; // Event for when the bullet fires

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        _timer = GameManager.Instance.PlayerData.BulletCooldown; // start with no cooldown
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate planar angle offset of player from camera - calculated every frame for external use
        PlayerAngleOffset = _playerController.transform.rotation.eulerAngles.y - Camera.main.transform.rotation.eulerAngles.y;
        if (PlayerAngleOffset < -180) PlayerAngleOffset += 360;
        else if (PlayerAngleOffset > 180) PlayerAngleOffset -= 360;

        // Input buffer
        _bufferTimer -= Time.deltaTime;
        // reset input buffer on mouse input - but also check player state
        if (Input.GetKeyDown(KeyCode.Mouse0)) _bufferTimer = _inputBuffer;

        // cancel buffer if not stationary
        if (_playerController.State != CharacterState.STATIONARY) _bufferTimer = 0;

        // shooting input buffer AND not during cooldown time
        if (_bufferTimer > 0 && _timer > GameManager.Instance.PlayerData.BulletCooldown 
            && !GameManager.IsPaused && !GameManager.Instance.PlayerData.CrumblingDeath)
        {
            // create new bullet
            GameObject newBullet = Instantiate(BulletObject);

            // set position of new bullet
            newBullet.transform.position = _gunPosition.position;

            Vector3 projectileDirection;

            // calculate shoot direction based on 
            Vector3 shootDir = Quaternion.AngleAxis(PlayerAngleOffset, Vector3.up) * Camera.main.transform.forward;

            // raycast to find projectile direction (actual trajectory) from shoot direction (raycast from camera)
            Ray shootRay = new(Camera.main.transform.position, shootDir);
            if(Physics.Raycast(shootRay, out RaycastHit hitInfo, _maxShootCastRange, ~_ignoreMask))
            {
                projectileDirection = (hitInfo.point - _gunPosition.position).normalized;
            }
            else
            {
                projectileDirection = (shootRay.GetPoint(_maxShootCastRange) - _gunPosition.position).normalized;
            }

            // align and apply force
            newBullet.transform.LookAt(newBullet.transform.position + projectileDirection);
            newBullet.GetComponent<Rigidbody>().AddForce(newBullet.GetComponentInChildren<BulletStats>().InitialForce * newBullet.transform.forward, ForceMode.Impulse);

            _timer = 0.0f; // reset cooldown timer
            onBulletFire?.Invoke();
        }

        _timer += Time.deltaTime;
    }
}
