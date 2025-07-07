using System;
using System.Collections;
using UnityEngine;

namespace Voidwalker.Ship.Projectile
{
    public class ProjectileTargetProximityLifetimeDecorator : ProjectileLifetimeBaseDecorator
    {
        private event Action TimerExpired;
        private ICanExecuteCoroutines executeCoroutines;
        private float targetDistance;
        private readonly float projectileSpeed;
        private readonly float minLifetime;
        private readonly float lowRandomLifetimeRange;
        private readonly float highRandomLifetimeRange;
        private bool isDisposed;

        public ProjectileTargetProximityLifetimeDecorator(
            ProjectileLifetimeComponent projectileLifetimeComponent,
            ICanExecuteCoroutines executeCoroutines,
            float minLifetime,
            float lowRandomLifetimeRange,
            float highRandomLifetimeRange,
            float projectileSpeed,
            Action onTimerExpired) : base(projectileLifetimeComponent)
        {
            this.executeCoroutines = executeCoroutines;
            this.minLifetime = minLifetime;
            this.lowRandomLifetimeRange = lowRandomLifetimeRange;
            this.highRandomLifetimeRange = highRandomLifetimeRange;
            this.projectileSpeed = projectileSpeed;
            TimerExpired = onTimerExpired;
        }

        public override void StartLifetimeTimer()
        {
            base.StartLifetimeTimer();
            executeCoroutines.StartCoroutine(TargetProximityLifetimeCheck());
        }

        public void OnTargetDistanceUpdated(float amount)
        {
            targetDistance = amount;
        }

        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                TimerExpired = null;
                executeCoroutines = null;
            }

            isDisposed = true;

            base.Dispose(disposing);
        }

        private IEnumerator TargetProximityLifetimeCheck()
        {
            float timeToTarget = targetDistance / projectileSpeed;
            float randomLifetimeOffset = UnityEngine.Random.Range(lowRandomLifetimeRange, highRandomLifetimeRange);
            timeToTarget += randomLifetimeOffset;
            float lifeTime = minLifetime > timeToTarget ? minLifetime : timeToTarget;

            yield return new WaitForSeconds(lifeTime);
            TimerExpired?.Invoke();
        }
    }
}
