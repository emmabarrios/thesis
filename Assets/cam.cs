using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    [SerializeField] private Player player = null;
    private Animator animator;

    private const string IS_ATTACKING_RIGHT = "swing_right";
    private const string IS_ATTACKING_LEFT = "swing_left";
    private const string IS_ATTACKING_DOWN = "swing_down";
    private const string IS_ATTACKING_UP = "swing_up";
    private const string IS_DASHING = "dash";
    private const string IS_WALKING = "isWalking";
    private const string IS_BLOCKING = "isBlocking";

    public event EventHandler OnUsingItem;
    public event EventHandler OnAnimating;
    public event EventHandler OnFinishedUsingItem;
    public event EventHandler OnEnterAttack;
    public event EventHandler OnFinishedAction;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player.OnAttack += ProcessPlayerAttack;
        player.OnDash += ExecuteDashAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking);
        animator.SetFloat("speed", Mathf.Clamp(player.CurrentSpeed, 0.5f, 2));
        animator.SetFloat("x", player.DashPoint.x);
        animator.SetFloat("y", player.DashPoint.y);
    }

    private void ProcessPlayerAttack(object sender, Player.OnAttackEventArgs e) {

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
        } else {
            CallFinishedAction();
        }
    }

    public void CallFinishedAction() {
        OnFinishedAction?.Invoke(this, EventArgs.Empty);
    }

    private void ExecuteDashAnimation(object sender, EventArgs e) {
        animator.SetTrigger(IS_DASHING);
        
    }
}
