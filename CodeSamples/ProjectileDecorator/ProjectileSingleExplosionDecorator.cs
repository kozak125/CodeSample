using System;
using UnityEngine;

namespace Voidwalker.Ship.Projectile
{
    public class ProjectileSingleExplosionDecorator : ProjectileExplosionBaseDecorator
    {
        public ProjectileSingleExplosionDecorator(ProjectileExplosionComponent projectileExplosionComponent, Transform projectileTransform, float explosionRadius, float damageAmount, ParticleSystem explosionParticles, Action playExplosionAudio) :
            base(projectileExplosionComponent, projectileTransform, explosionRadius, damageAmount, explosionParticles, playExplosionAudio)
        {

        }

        public override void Explode()
        {
            base.Explode();
            SingleExplosion();
        }

        private void SingleExplosion()
        {
            Collider[] colliders = Physics.OverlapSphere(projectileTransform.position, explosionRadius, LayersUtils.ENEMY_LAYER | LayersUtils.PLAYER_LAYER);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IDamageable damagable))
                {
                    damagable.ReceiveDamage(damageAmount);
                }
            }

            if (explosionParticles != null)
            {
                explosionParticles.Play();
            }

            PlayExplosionAudio?.Invoke(); // move this and particles to base class?
        }
    }
}
