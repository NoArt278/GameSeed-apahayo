using System;
using UnityEngine;

// This class is used to sync execution call of OnEnable and OnDisable methods

// THE ISSUES:
// ---
// Singletons are initiated in Awake method
// Based on unity documentation, Awake is called before OnEnable - https://docs.unity3d.com/Manual/ExecutionOrder.html
// This means, singleton is not accessible in OnEnable method, if they are initiated in the same scene

// WHY NEED TO CALL IN ONENABLE INSTEAD OF START:
// ---
// We might enable and disable objects in runtime
// Events registration and deregistration is important to be handled in this case
// Use case example: Input registration and deregistration

// WHAT THIS CLASS RESOLVES:
// ---
// This class is used to sync execution call of OnEnable and OnDisable methods
// On the first initiation, OnEnable is called in start method
// Thus ensures that singleton is initiated before OnEnable is called
// After that, enable and disabling objects in runtime will call OnEnable and OnDisable methods as expected
public abstract class SingletonSyncBehaviour : MonoBehaviour {
    protected abstract Action OnEnableCallback();
    protected abstract Action OnDisableCallback();

    private bool startFlag = false;

    protected virtual void OnEnable() {
        if (!startFlag) return;
        OnEnableCallback()?.Invoke();
    }

    protected virtual void Start() {
        startFlag = true;
        OnEnable();
    }

    protected virtual void OnDisable() {
        OnDisableCallback()?.Invoke();
    }
}