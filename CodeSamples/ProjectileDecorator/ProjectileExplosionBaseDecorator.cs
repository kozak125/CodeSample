using System;
using UnityEngine;

namespace Voidwalker.Ship.Projectile
{
    public abstract class ProjectileExplosionBaseDecorator : ProjectileExplosionComponent, IDisposable
    {
        protected ProjectileExplosionComponent projectileExplosionComponent;
        protected Transform projectileTransform;
        protected float explosionRadius;
        protected float damageAmount;
        protected ParticleSystem explosionParticles;
        protected Action PlayExplosionAudio;
        private bool isDisposed;

        public ProjectileExplosionBaseDecorator(ProjectileExplosionComponent projectileExplosionComponent, Transform projectileTransform, float explosionRadius, float damageAmount, ParticleSystem explosionParticles, Action playExplosionAudio)
        {
            this.projectileExplosionComponent = projectileExplosionComponent;
            this.projectileTransform = projectileTransform;
            this.explosionRadius = explosionRadius;
            this.damageAmount = damageAmount;
            this.explosionParticles = explosionParticles;
            PlayExplosionAudio = playExplosionAudio;
        }

        public override void Explode()
        {
            projectileExplosionComponent.Explode();
        }

        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                projectileExplosionComponent = null;
                projectileTransform = null;
                explosionParticles = null;
                PlayExplosionAudio = null;
            }

            isDisposed = true;

            projectileExplosionComponent?.Dispose();
        }
    }
}
