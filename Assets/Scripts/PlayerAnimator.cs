using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    [SerializeField] Animator camAnimator = null;

    [SerializeField] private float useItemAnimLenght;

    private bool quickItemActionPerformed = false;

    public Action OnOpenedFlask;
    public Action OnDropedFlask;

    public event Action OnAnimating;
    public event Action OnSwingLeft;
    public event Action OnSwingRight;
    public event Action<bool> OnQuickItemAction;

   // public event EventHandler OnFinishedUsingItem;
    public event EventHandler OnEnterAttack;
    public event EventHandler OnFinishedAction;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    //public void Trigger_UsingItem_AnimState() {
    //    //animator.SetTrigger(IS_USING_ITEM);
    //    OnUsingItem?.Invoke(this, new OnUsingItemEventArgs { animLength = useItemAnimLenght });
    //}

    //public void CallOnFinishedUsingItem() {
    //    OnFinishedUsingItem?.Invoke(this, EventArgs.Empty);
    //}

    //public void ExecuteRiseShieldAnimation(object sender, EventArgs e) {
    //    animator.SetBool(IS_BLOCKING, true);
    //}

    //public void ExecuteLowerShieldAnimation(object sender, EventArgs e) {
    //    animator.SetBool(IS_BLOCKING, false);
    //}

    //public void CallFinishedAction() {
    //    OnFinishedAction?.Invoke(this, EventArgs.Empty);
    //}

    //private void PlayAttackAnimation(GestureInput.SwipeDir swipeDirection) {

    //    if (swipeDirection == GestureInput.SwipeDir.Right) {
    //        animator.SetTrigger(IS_ATTACKING_RIGHT);
    //    } else if (swipeDirection == GestureInput.SwipeDir.Left) {
    //        animator.SetTrigger(IS_ATTACKING_LEFT);
    //    } else if (swipeDirection == GestureInput.SwipeDir.Down) {
    //        animator.SetTrigger(IS_ATTACKING_DOWN);
    //    } else if (swipeDirection == GestureInput.SwipeDir.Up) {
    //        animator.SetTrigger(IS_ATTACKING_UP);
    //    } else {
    //        CallFinishedAction();
    //    }
    //}

    private void OnAnimatorMove() {
        OnAnimating?.Invoke();
    }

    public void OpenFlask() {
        OnOpenedFlask?.Invoke();
    }

    public void DropFlask() {
        OnDropedFlask?.Invoke();
    }

    public void RotateCameraRight() {
        OnSwingLeft?.Invoke();
    }

    public void RotateCameraLeft() {
        OnSwingRight?.Invoke();
    }

    public void QuickItemAction() {
        quickItemActionPerformed = !quickItemActionPerformed;
        OnQuickItemAction?.Invoke(quickItemActionPerformed);
    }

}
