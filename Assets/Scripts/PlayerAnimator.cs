using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAnimator : MonoBehaviour {

    [SerializeField] private Animator animator;

    private const string IS_ATTACKING_RIGHT = "isAttackingRight";
    private const string IS_ATTACKING_LEFT = "isAttackingLeft";
    private const string IS_ATTACKING_DOWN = "isAttackingDown";
    private const string IS_ATTACKING_UP = "isAttackingUp";
    private const string IS_DASHING = "isDashing";
    private const string IS_WALKING = "isWalking";
    private const string IS_BLOCKING = "isBlocking";
    private const string WALK_SPEED = "WalkSpeed";
    private const string IS_USING_ITEM = "isUsingItem";

    [SerializeField] private PlayerController playerController;

    [SerializeField] private float useItemAnimLenght;

    public event EventHandler<OnUsingItemEventArgs> OnUsingItem;
    public event EventHandler OnAnimating;
    public event EventHandler OnFinishedUsingItem;
    public event EventHandler OnEnterAttack;
    public event EventHandler OnFinishedAction;

    public class OnUsingItemEventArgs: EventArgs {
        public float animLength;
    }

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();

        playerController.OnAttack += ProcessPlayerAttack;
        playerController.OnDash += ExecuteDashAnimation;
        playerController.OnBlocking += ExecuteRiseShieldAnimation;
        playerController.OnReleaseBlock += ExecuteLowerShieldAnimation;
        playerController.OnParry += ExecuteParryAnimation;
    }

    // Update is called once per frame
    void Update() {
        animator.SetBool(IS_WALKING, playerController.IsWalking);
        animator.SetFloat(WALK_SPEED, Mathf.Clamp(playerController.CurrentSpeed, 0.1f, 1));
        animator.SetBool(IS_BLOCKING, playerController.IsBlocking);
    }

    public void Trigger_UsingItem_AnimState() {
        animator.SetTrigger(IS_USING_ITEM);
        OnUsingItem?.Invoke(this, new OnUsingItemEventArgs { animLength = useItemAnimLenght });
    }

    //public void LateUpdate() {
    //    AnimatorClipInfo[] clipInfoArray = animator.GetCurrentAnimatorClipInfo(0);

    //    for (int i = 0; i < clipInfoArray.Length; i++) {
    //        if (clipInfoArray[i].clip.name == "Use_Bottle_1") {
    //            Debug.Log(clipInfoArray[i].clip.name);
    //            AnimatorClipInfo clipInfo = clipInfoArray[i];
    //            OnUsingItem?.Invoke(this, new OnUsingItemEventArgs { animLength = clipInfo.clip.length });
    //        }
    //    }
    //}

    public void CallOnFinishedUsingItem() {
        OnFinishedUsingItem?.Invoke(this, EventArgs.Empty);
    }

    public void ExecuteRiseShieldAnimation(object sender, EventArgs e) {
        animator.SetBool(IS_BLOCKING, true);
    }

    public void ExecuteLowerShieldAnimation(object sender, EventArgs e) {
        animator.SetBool(IS_BLOCKING, false);
    }

    public void CallFinishedAction() {
        OnFinishedAction?.Invoke(this, EventArgs.Empty);
    }

    private void ExecuteDashAnimation(object sender, EventArgs e) {
        animator.SetTrigger(IS_DASHING);
    }

    private void ExecuteParryAnimation(object sender, EventArgs e) {
        animator.SetBool("parry", true);
    }

    private void ProcessPlayerAttack(object sender, PlayerController.OnAttackEventArgs e) {

        if (e.swipeDirection == GestureInput.SwipeDir.Right) {
            animator.SetTrigger(IS_ATTACKING_RIGHT);
        } else if (e.swipeDirection == GestureInput.SwipeDir.Left) {
            animator.SetTrigger(IS_ATTACKING_LEFT);
        } else if (e.swipeDirection == GestureInput.SwipeDir.Down) {
            animator.SetTrigger(IS_ATTACKING_DOWN);
        } else if (e.swipeDirection == GestureInput.SwipeDir.Up) {
            animator.SetTrigger(IS_ATTACKING_UP);
        } else {
            CallFinishedAction();
        }
    }
    
    private void DisableParryState() {
        animator.SetBool("parry", false);
    }

    private void OnAnimatorMove() {
        OnAnimating?.Invoke(this, EventArgs.Empty);
    }

}
