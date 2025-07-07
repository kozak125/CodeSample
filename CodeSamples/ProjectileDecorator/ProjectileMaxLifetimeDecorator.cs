using System;
using System.Collections;
using UnityEngine;

namespace Voidwalker.Ship.Projectile
{
    public class ProjectileMaxLifetimeDecorator : ProjectileLifetimeBaseDecorator
    {
        private event Action TimerExpired;
        private ICanExecuteCoroutines executeCoroutines;
        private readonly float projectileMaxLifetime;
        private bool isDisposed;

        public ProjectileMaxLifetimeDecorator(ProjectileLifetimeComponent projectileLifetimeComponent, ICanExecuteCoroutines executeCoroutines, float projectileMaxLifetime, Action onTimerExpired) : base(projectileLifetimeComponent)
        {
            this.executeCoroutines = executeCoroutines;
            this.projectileMaxLifetime = projectileMaxLifetime;
            TimerExpired = onTimerExpired;
        }

        public override void StartLifetimeTimer()
        {
            base.StartLifetimeTimer();
            executeCoroutines.StartCoroutine(ProjectileLifetimeCheck());
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

        private IEnumerator ProjectileLifetimeCheck()
        {
            yield return new WaitForSeconds(projectileMaxLifetime);
            TimerExpired?.Invoke();
        }
    }
}
