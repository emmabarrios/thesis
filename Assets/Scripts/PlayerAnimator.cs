using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerAnimator : MonoBehaviour {
    [SerializeField] private Animator animator;
    private const string IS_ATTACKING_RIGHT = "isAttackingRight";
    private const string IS_ATTACKING_LEFT = "isAttackingLeft";
    private const string IS_ATTACKING_DOWN = "isAttackingDown";
    private const string IS_ATTACKING_UP = "isAttackingUp";
    private const string IS_DASHING = "isDashing";
    private const string IS_WALKING = "isWalking";
    private const string IS_BLOCKING = "isBlocking";
    private const string PARRY = "parry";

    [SerializeField] private Player player;
    [SerializeField] private Transform handAttachPoint;

    public event EventHandler OnUsingItem;
    public event EventHandler OnFinishedUsingItem;
    public event EventHandler OnEnterAttack;
    public event EventHandler OnFinishedAttack;

    // Start is called before the first frame update
    void Start() {
        animator = this.GetComponent<Animator>();
        player = this.GetComponentInParent<Player>();

        player.OnAttack += ProcessPlayerAttack;
        player.OnDash += ExecuteDashAnimation;
        player.OnBlocking += ExecuteRiseShieldAnimation;
        player.OnReleaseBlock += ExecuteLowerShieldAnimation;
        player.OnParry += ExecuteParryAnimation;

    }

    // Update is called once per frame
    void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking);
    }


    public void CallOnUsingItem() {
        OnUsingItem?.Invoke(this, EventArgs.Empty);
    }

    public void CallOnFinishedUsingItem() {
        OnFinishedUsingItem?.Invoke(this, EventArgs.Empty);
    }

    public void ExecuteRiseShieldAnimation(object sender, EventArgs e) {
        animator.SetBool(IS_BLOCKING, true);
    }

    public void ExecuteLowerShieldAnimation(object sender, EventArgs e) {
        animator.SetBool(IS_BLOCKING, false);

    }

    //public void CallOnEnterAttack() {
    //    OnEnterAttack?.Invoke(this, EventArgs.Empty);
    //}

    public void CallOnFinishedAttack() {
        OnFinishedAttack?.Invoke(this, EventArgs.Empty);
    }


    private void ExecuteDashAnimation(object sender, EventArgs e) {
        animator.SetTrigger(IS_DASHING);
    }

    private void ExecuteParryAnimation(object sender, EventArgs e) {
        animator.SetBool("parry", true);
    }

    private void ProcessPlayerAttack(object sender, Player.OnAttackEventArgs e) {
        //Debug.Log(e.swipeDirection);

        if (e.swipeDirection == SwipeDetector.SwipeDir.Right) {
            animator.SetTrigger(IS_ATTACKING_RIGHT);
            //OnEnterAttack?.Invoke(this, EventArgs.Empty);
        } else if (e.swipeDirection == SwipeDetector.SwipeDir.Left) {
            animator.SetTrigger(IS_ATTACKING_LEFT);
            //OnEnterAttack?.Invoke(this, EventArgs.Empty);
        } else if (e.swipeDirection == SwipeDetector.SwipeDir.Down) {
            animator.SetTrigger(IS_ATTACKING_DOWN);
            //OnEnterAttack?.Invoke(this, EventArgs.Empty);
        } else if (e.swipeDirection == SwipeDetector.SwipeDir.Up) {
            animator.SetTrigger(IS_ATTACKING_UP);
            //OnEnterAttack?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private void DisableParryState() {
        animator.SetBool("parry", false);
    }
}
