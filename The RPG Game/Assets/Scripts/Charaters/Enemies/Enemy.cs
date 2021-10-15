﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterStats))]
public class Enemy : Interactable
{
    public string enemyName;
    public float lookRadius = 10f;

    public Transform target;
    public EnemyStats enemyStats;
    public CharacterCombat combat;
    private NavMeshAgent agent;
    public Animator animator;

    public bool inCombat;
    public bool isDetected;
    public float speed;

    public bool isWalkingAnimation;
    public bool isAttackingAnimation;

    private int isWalkingHash;
    private int isAttackingHash;

    public bool hasMoved;
    public float timeBeforeMoving;
    public Vector3 randomPoint = Vector3.zero;

    private void Start() {
        combat = GetComponent<CharacterCombat>();
        enemyStats = GetComponent<EnemyStats>();
        agent = GetComponent<NavMeshAgent>();
        target = GameManager.instance.player.transform;

        SetAnimations();
    }



    protected override void InitUpdate() {
        if (isFocus) {
            float distance = Vector3.Distance(playerTransform.position, interactionTransform.position);
            if (!hasInteracted) {
                if (distance <= player.playerStats.attackRange.GetValue()) {
                    Interact();
                    hasInteracted = true;
                    player.inCombat = true;
                }
                else if (distance > player.playerStats.attackRange.GetValue()) {
                    hasInteracted = false;
                    player.inCombat = false;
                }
            }
            else if (distance > player.playerStats.attackRange.GetValue() && !player.isBowAnimation) {
                hasInteracted = false;
                player.inCombat = false;
            }
        }
    }


    private void Update() {
        MoveToTarget();
        AnimatorManager();
        Animation();
        MoveRandomlyAround();
        InitUpdate();
    }

    protected override void Interact() {
        base.Interact();
    }

    private void MoveToPoint(Vector3 point) {
        agent.SetDestination(point);
    }

    private void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); ;
    }
    private void MoveToTarget() {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius) {
            isDetected = true;
            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance) {
                CharacterStats targetStats = target.GetComponent<CharacterStats>();
                if(targetStats != null) {
                    inCombat = true;
                }
                FaceTarget();
            }
            else {
                inCombat = false;
            }
        }
        else if(distance > lookRadius) {
            inCombat = false;
            isDetected = false;

        }
    }




    //  RANDOM MOVEMENT
    private void MoveRandomlyAround() {
        if (!isDetected) {
            if (timeBeforeMoving <= 0) {
                timeBeforeMoving = Random.Range(10, 15);
                randomPoint = GetNewRandomPoint(15);
            }
            else if (timeBeforeMoving > 0) {
                timeBeforeMoving -= Time.deltaTime;
                MoveToPoint(randomPoint);
            }
        }
    }


    private Vector3 GetNewRandomPoint(int range) {
        return new Vector3(transform.position.x + Random.Range(-range, range), 0, transform.position.z + Random.Range(-range, range));
    }
    



    //ANIMATIONS
    public void Animation() {
        //movement animations
        if (agent.velocity.magnitude >= 0.1) {
                animator.SetBool(isWalkingHash, true);
                SetMovementSpeed(2);
        }
        else if (agent.velocity.magnitude == 0f) {
            animator.SetBool(isWalkingHash, false);
        }

        if (inCombat) {
            animator.SetBool(isAttackingHash, true);
            animator.SetBool(isWalkingHash, false);
        }
        else {
            animator.SetBool(isAttackingHash, false);
        }
    }
    private void AnimatorManager() {
        isWalkingAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Walk");
        isAttackingAnimation = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }
    private void SetAnimations() {
        isWalkingHash = Animator.StringToHash("Walking");
        isAttackingHash = Animator.StringToHash("Attacking");
    }









    //Setters
    public void SetMovementSpeed(float speed) {
        this.speed = speed;
        agent.speed = speed;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
    private void OnMouseOver() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        Tooltip.ShowTooltip_static(enemyName);

        //MenuBar BAR
        if (Input.GetMouseButtonDown(1)) {
            MenuBar.SetTarget_static(this);
        }
    }
    private void OnMouseExit() {
        Tooltip.HideTooltip_static();
    }

}
