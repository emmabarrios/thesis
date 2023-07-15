using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour {
    
    public static Thrower instance;

    public float _timer;
    public bool _isTiming = false;

    [SerializeField] private Projectile _projectile;
    [SerializeField] private float _throwForce;
    [SerializeField] private float _throwUpwardForce;
    [SerializeField] private float _rotationForce;
    [SerializeField] private Vector3 _offset;

    private void Awake() {
        if (instance != null) { return; }
        instance = this;
    }

    public void LoadThrower(Projectile projectile, float throwForce, float throwUpwardForce, float rotationForce, Vector3 offset, float loadTime) {
        _timer = loadTime;
        _projectile = projectile;
        _throwForce = throwForce;
        _throwUpwardForce = throwUpwardForce;
        _rotationForce = rotationForce;
        _offset = offset;
        _isTiming = true;
    }

    public void Throw() {
        Projectile projectile = Instantiate(_projectile, transform.position + _offset, transform.rotation);
        Vector3 force = transform.forward * _throwForce + (transform.up * _throwUpwardForce);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(transform.up * _rotationForce + transform.right * _rotationForce, ForceMode.Impulse);
    }

    private void Update() {
        // Throw timer
        if (_isTiming) {
            _timer -= Time.deltaTime;
            if (_timer < 0.1f) {
                _timer = 0;
                Throw();
                _isTiming = false;
            }
        }
    }
}
