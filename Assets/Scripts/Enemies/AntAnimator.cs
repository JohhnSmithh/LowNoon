using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntAnimator : EnemyAnimator
{
    [SerializeField] private GameObject _vanishEffect;
    private CapsuleCollider _collider;
    private float _maxHealth;

    private MeleeMovement _meleeMovement;
    private MeleeAttack _meleeAttack;
    new void Start()
    {
        base.Start();
        _maxHealth = _damageReceiver.HealthLevel; // Store max HP to use to revive later
        _collider = GetComponent<CapsuleCollider>();
        _meleeMovement = GetComponent<MeleeMovement>();
        _meleeAttack = GetComponent<MeleeAttack>();
    }

    // Update is called once per frame
    new void Update()
    {
        _animator.SetFloat("HP", _damageReceiver.HealthLevel);
        _animator.SetBool("isMoving", !_meleeMovement.IsIdle);
        _animator.SetBool("isAttacking", _meleeMovement.IsAtPlayer);

        if (prevHP > _damageReceiver.HealthLevel)    // If the enemy has been damaged
        {
            if (_damageReceiver.HealthLevel <= 0 && !_animator.GetBool("isDead")) // If the enemy is dead
            {
                StartCoroutine(DoDeathAnim());
            }
            else // If only hurt
            {
                // Ant has no hurt anim, 2 small, but there WILL be a sound
            }
        }
        prevHP = _damageReceiver.HealthLevel;
    }
    protected override IEnumerator DoDeathAnim()
    {
        _animator.SetBool("isDead", true);
        yield return new WaitForSeconds(_deathAnimDuration);
        Instantiate(_vanishEffect, this.transform.position, _vanishEffect.transform.rotation);
        Destroy(this.gameObject);
    }
}
