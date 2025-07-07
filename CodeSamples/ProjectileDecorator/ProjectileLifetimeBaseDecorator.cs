namespace Voidwalker.Ship.Projectile
{
    public class ProjectileLifetimeBaseDecorator : ProjectileLifetimeComponent
    {
        protected ProjectileLifetimeComponent projectileLifetimeComponent;
        private bool isDisposed;

        public ProjectileLifetimeBaseDecorator(ProjectileLifetimeComponent projectileLifetimeComponent)
        {
            this.projectileLifetimeComponent = projectileLifetimeComponent;
        }

        public override void StartLifetimeTimer()
        {
            projectileLifetimeComponent.StartLifetimeTimer();
        }

        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                projectileLifetimeComponent = null;
            }

            isDisposed = true;

            projectileLifetimeComponent?.Dispose();
        }
    }
}
