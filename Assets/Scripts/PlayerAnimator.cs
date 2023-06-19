using System;
using UnityEngine;

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

    [SerializeField] private Controller controller;
    [SerializeField] private HitArea hitArea = null;

    [SerializeField] private float useItemAnimLenght;

    public Action OnOpenedFlask;
    public Action OnDropedFlask;

    public event EventHandler<OnUsingItemEventArgs> OnUsingItem;
    public event EventHandler OnAnimating;
    public event EventHandler OnFinishedUsingItem;
    public event EventHandler OnEnterAttack;
    public event EventHandler OnFinishedAction;

    [SerializeField] private Player player;

    public class OnUsingItemEventArgs: EventArgs {
        public float animLength;
    }

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<Controller>();
        player = GetComponentInParent<Player>();

        controller.OnAttack += ProcessPlayerAttack;
        controller.OnDash += ExecuteDashAnimation;
        //playerController.OnBlocking += ExecuteRiseShieldAnimation;
        //playerController.OnReleaseBlock += ExecuteLowerShieldAnimation;
        controller.OnParry += ExecuteParryAnimation;

        hitArea.OnHitDeflected += ExecuteHitDeflectionAnimation;
    }

    // Update is called once per frame
    void Update() {
        animator.SetBool(IS_WALKING, controller.IsWalking);
        animator.SetFloat(WALK_SPEED, Mathf.Clamp(controller.CurrentSpeed, 0.1f, 1));
        animator.SetBool(IS_BLOCKING, controller.IsBlocking);
    }

    public void Trigger_UsingItem_AnimState() {
        //animator.SetTrigger(IS_USING_ITEM);
        OnUsingItem?.Invoke(this, new OnUsingItemEventArgs { animLength = useItemAnimLenght });
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

    public void CallFinishedAction() {
        OnFinishedAction?.Invoke(this, EventArgs.Empty);
    }

    private void ExecuteDashAnimation(Vector2 dir) {
        animator.SetTrigger(IS_DASHING);
    }

    private void ExecuteParryAnimation() {
        animator.SetBool("parry", true);
    }

    private void ProcessPlayerAttack(GestureInput.SwipeDir swipeDirection) {

        if (swipeDirection == GestureInput.SwipeDir.Right) {
            animator.SetTrigger(IS_ATTACKING_RIGHT);
        } else if (swipeDirection == GestureInput.SwipeDir.Left) {
            animator.SetTrigger(IS_ATTACKING_LEFT);
        } else if (swipeDirection == GestureInput.SwipeDir.Down) {
            animator.SetTrigger(IS_ATTACKING_DOWN);
        } else if (swipeDirection == GestureInput.SwipeDir.Up) {
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

    private void ExecuteHitDeflectionAnimation(object sender, EventArgs e) {
        animator.SetTrigger("deflectedHit");
    }

    public void OpenFlask() {
        OnOpenedFlask?.Invoke();
    }
    
    public void DropFlask() {
        OnDropedFlask?.Invoke();
    }
    
    public void RotateCameraRight() {
        GameObject.Find("Camera Holder").GetComponent<Animator>().Play("rotate_right", -1,0f);
    }
    public void RotateCameraLeft() {
        GameObject.Find("Camera Holder").GetComponent<Animator>().Play("rotate_left", -1,0f);
    }

}
