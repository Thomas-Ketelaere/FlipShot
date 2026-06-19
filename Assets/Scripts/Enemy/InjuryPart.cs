using System;
using UnityEngine;

[Serializable]
enum InjuryBodyPart
{ 
    Head,
    Torso,
    Leg,
    Arm
}

public class InjuryPart : MonoBehaviour //todo better name
{
    [SerializeField] private InjuryBodyPart _part;
    private HealthComponent _health;

    void Start()
    {
        _health = GetComponentInParent<HealthComponent>();
    }

    public void BodyPartHit(Vector3 direction, Vector3 hitPoint) //send location and direction for blood vfx and ragdoll later
    {
        switch (_part)
        {
            case InjuryBodyPart.Head:
                _health.InstantKill(direction, hitPoint);
                break;
            case InjuryBodyPart.Torso:
                _health.InstantKill(direction, hitPoint);
                break;
            case InjuryBodyPart.Leg:
                _health.GetHit(direction, hitPoint);
                break;
            case InjuryBodyPart.Arm:
                _health.GetHit(direction, hitPoint);
                break;
        }

        
    }
}
