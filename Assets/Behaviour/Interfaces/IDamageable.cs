﻿public interface IDamageable
{
    float hp { get; }
    void damage(float amount, UnityEngine.GameObject offender);
}