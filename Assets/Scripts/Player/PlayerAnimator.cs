using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // Player animations essentials
    private PlayerController _playerController;
    private AudioSource _audioSource;
    [SerializeField] private Animator _animator;
    [SerializeField] private List<AudioClip> _clips = new List<AudioClip>();
    // Audio clip order list
    // 0 = Standard gunshot
    // 1 = Standard reload
    // 2 = Six shot reload

    // Accessory animation essentials
    [SerializeField, Tooltip("The model for the player's hat")] private GameObject _hat;
    [SerializeField, Tooltip("The model for the player's lamp")] private GameObject _lamp;
    private Animator _hatAnimator;  // The hat and lamp animators are used p. much exclusively for the death animation
    private Animator _lampAnimator;
    private int _sixShotCount;

    // Gun GameObject and animator no longer required now that gun is parented to player + doesn't need the firing animation


    private void OnEnable()
    {
        PlayerShooting.onBulletFire += fireGun;
    }

    private void OnDisable()
    {
        PlayerShooting.onBulletFire -= fireGun;
    }

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _audioSource = GetComponent<AudioSource>();
        _sixShotCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("moveSpdMult", GameManager.Instance.PlayerData.MoveSpeed); // Move speed multiplier so animation stays tuned to actual speed

        if(_playerController.enabled == false)  // If the player lacks control, default the animation to the idle
        {
            _animator.SetBool("isDashing", false);
            _animator.SetBool("isMoving", false);
        }
        else
        {
            // Player animator is primarily controlled by whether the player is (a) dashing and (b) moving
            if (_playerController.State == CharacterState.STATIONARY)
            {
                _animator.SetBool("isMoving", false);
                if (_playerController.State == CharacterState.DASH)
                {
                    StopAllCoroutines();
                    _animator.SetBool("isDashing", true);
                }
                else
                {
                    _animator.SetBool("isDashing", false);
                    
                }

            }
            else
            {
                if (_playerController.State == CharacterState.DASH)
                {
                    StopAllCoroutines();
                    _animator.SetBool("isDashing", true);

                }
                else
                {
                    StopAllCoroutines();
                    _animator.SetBool("isDashing", false);
                    _animator.SetBool("isMoving", true);
                }

            }
        }
        
   
    }

    private void fireGun() // Method that does prep for the coroutine and calls it (can't call a coroutine with a delegate)
    {
        StopAllCoroutines();
        _animator.Play("Idle", 0, 0);
        StartCoroutine(DoFireGun());
    }

    private IEnumerator DoFireGun() // Unique sequence for firing the gun
    {
        _audioSource.PlayOneShot(_clips[0]);
        _animator.Play("Fire", 0, 0);
        _sixShotCount++;
        yield return new WaitForSeconds(.5f);
        if(_sixShotCount == 6)
        {
            _audioSource.PlayOneShot(_clips[2]);
            _sixShotCount = 0;
        }
        else
        {
            _audioSource.PlayOneShot(_clips[1]);
        }
            
        _animator.Play("Idle", 0, 0);

    }
}
