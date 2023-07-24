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
