using System;
using UnityEngine;

namespace _Scripts.Occupants
{
    public class FireFighterOccupantController : OccupantController, IPlayerOccupant
    {
        private static readonly int Unstick = Animator.StringToHash("Unstick");
        private static readonly int Move = Animator.StringToHash("Move");
        
        public bool IsUnstick {get; private set;}
        
        public Animator Animator { get; private set; }

        private void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
        }
        
        public void TriggerMove()
        {
            IsUnstick = false;
            Animator.SetTrigger(Move);
        }

        public void TriggerUnstick()
        {
            Animator.SetTrigger(Unstick);
        }
        
        private void UnstickFinished()
        {
            IsUnstick = true;
        }
    }
}
