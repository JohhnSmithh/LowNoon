using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public GameObject AttackCollider;
    public float AttackTime;
    public float AttackCooldown;
    private GameObject _player;
    private float _attackTimer;
    private bool _duringAttack;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _duringAttack = false;
        _attackTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _attackTimer += Time.deltaTime;

        if (_duringAttack && _attackTimer > AttackTime)
        {
            _duringAttack = false;
            AttackCollider.SetActive(false);
            _attackTimer = 0;
        }

        if (Vector3.Distance(_player.transform.position, transform.position) <= 2f)
        {
            //Deal Damage here
            if (!_duringAttack && _attackTimer > AttackCooldown)
            {
                _duringAttack = true;
                AttackCollider.SetActive(true);
                _attackTimer = 0;
            }
        }
    }
}
