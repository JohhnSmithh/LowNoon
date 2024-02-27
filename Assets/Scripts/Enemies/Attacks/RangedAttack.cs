using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    [Header("Reference Objects")]
    [Tooltip("Stores the ranged movement script on the Enemy")] public RangedMovement RangedMovement;
    [Tooltip("Stores bullet predab")] public GameObject BulletObject;

    [Header("Bullet Stats")]
    [Tooltip("The force of the bullet when it spawns")] public float BulletForce;
    [Tooltip("How far away from the player left and right the bullet will hit")] public float BulletSpread;

    [Header("Bullet Timer")]
    [Tooltip("How much time should pass between each shot")] public float ShotTimer;
    [Tooltip("Minimum amount of time to change the shot timer by")] public float MinShotVar;
    [Tooltip("Maximum amount of time to change the shot timer by")] public float MaxShotVar;
    
    private GameObject _player;
    private float _cooldownTimer;
    private float _shotTimerRandom;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _cooldownTimer = ShotTimer + TimerRandom();
        _shotTimerRandom = ShotTimer + TimerRandom();
    }

    // Update is called once per frame
    void Update()
    {
        bool isStill = RangedMovement.StayStill;

        if (isStill)
        {
            if (_cooldownTimer > _shotTimerRandom)
            {
                _shotTimerRandom = ShotTimer + TimerRandom();
                _cooldownTimer = 0;
                GameObject bullet = Instantiate(BulletObject);
                bullet.AddComponent<DestroyOnTrigger>();
                bullet.transform.position = transform.position + transform.forward * 4f;
                bullet.transform.LookAt(_player.transform.position);
                Vector3 bulletRotation = bullet.transform.rotation.eulerAngles;
                bulletRotation.y += Random.Range(-BulletSpread, BulletSpread);
                bullet.transform.rotation = Quaternion.Euler(bulletRotation);
                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * BulletForce, ForceMode.Impulse);
            }
        }

        _cooldownTimer += Time.deltaTime;
    }

    float TimerRandom()
    {
        return Random.Range(MinShotVar, MaxShotVar);
    }
}
