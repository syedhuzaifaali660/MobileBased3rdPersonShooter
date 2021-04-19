
using System;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : UnityEvent<string>
{
    internal void AddListener()
    {
        throw new NotImplementedException();
    }
}
public class WeaponAnimationEvents : MonoBehaviour
{
    public AnimationEvent WeaponAnimationEvent = new AnimationEvent();


    public void OnAnimationEvent(string eventName)
    {
        WeaponAnimationEvent.Invoke(eventName);
    }
}
