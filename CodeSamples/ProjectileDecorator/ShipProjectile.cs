using System;
using UnityEngine;
using Voidwalker.Systems.ModularDamage;
using Voidwalker.Player.Ship.Upgrades;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Voidwalker.Ship.Projectile
{
    [RequireComponent(typeof(Rigidbody))]
    public class ShipProjectile : ExtendedMonoBehaviour, ICanExecuteCoroutines
    {
        public event Action OnProjectileDestroyed;
        private event Action<float> TargetDistanceUpdated;

        [Title("Data", TitleAlignment = TitleAlignments.Centered)]
        [SerializeField, InlineEditor, Required]
        private ShipProjectileBehaviorData behaviorData;
        [SerializeField, InlineEditor]
        private BroadsideCannonsUpgrades broadsideCannonsUpgrades;
        [SerializeField]
        private float projectileSpeed;
        [SerializeField]
        private float projectileDamage;
        [SerializeField]
        private float projectileDownwardsForce;
        [SerializeField, Required]
        private GameObject projectileGraphicsObject;

        [Title("Additional effects", TitleAlignment = TitleAlignments.Centered)]
        [SerializeField]
        private ParticleSystem movementVFX;
        [SerializeField]
        private ParticleSystem collisionVFX;
        [SerializeField]
        private ParticleSystem expiredVFX;
        [SerializeField]
        private TrailRenderer trailRenderer;
        [SerializeField]
        private AudioSource projectileHitAudioSource;

        private Rigidbody projectileRigidbody;
        private Transform projectileTransform;
        private StackObjectPool<ParticleSystem> instantiatedCollisionVFXPool;
        private StackObjectPool<ParticleSystem> instantiatedExpiredVFXPool;
        private StackObjectPool<AudioSource> instantiatedHitAudioSourcePool;

        private ProjectileLifetimeComponent lifeTimeComponent;
        private ProjectileExplosionComponent explosionComponent;

        private bool isProjectileDestroyed = false;

        public void Setup(float xDeviation, float yDeviation, Vector3 projectilePosition, Quaternion projectileRotation, Vector3 cannonForward, float distanceToTarget = 0f)
        {
            isProjectileDestroyed = false;
            projectileRigidbody.isKinematic = false;
            projectileGraphicsObject.SetActive(true);
            projectileTransform.SetPositionAndRotation(projectilePosition, projectileRotation);

            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }

            float playerProjectileSpreadMultiplier = broadsideCannonsUpgrades != null && broadsideCannonsUpgrades.DecreaseSpreadAndIncreaseSpeedDescription.IsUpgraded ? broadsideCannonsUpgrades.SpreadMultiplier : 1f;
            var deviationQuaternion = Quaternion.Euler(xDeviation * playerProjectileSpreadMultiplier, yDeviation * playerProjectileSpreadMultiplier, 0f);
            Vector3 finalShotDirection = deviationQuaternion * cannonForward;
            float playerProjectileSpeedMultiplier = broadsideCannonsUpgrades != null && broadsideCannonsUpgrades.DecreaseSpreadAndIncreaseSpeedDescription.IsUpgraded ? broadsideCannonsUpgrades.SpeedMultiplier : 1f;
            projectileRigidbody.velocity = finalShotDirection * (projectileSpeed * playerProjectileSpeedMultiplier);
            TargetDistanceUpdated?.Invoke(distanceToTarget);

            lifeTimeComponent.StartLifetimeTimer();
            TrySetMovementParticlesActive(true);
        }

        public float GetProjectileSpeed()
        {
            float playerProjectileSpeedMultiplier = broadsideCannonsUpgrades != null && broadsideCannonsUpgrades.DecreaseSpreadAndIncreaseSpeedDescription.IsUpgraded ? broadsideCannonsUpgrades.SpeedMultiplier : 1f;

            return projectileSpeed * playerProjectileSpeedMultiplier;
        }

        public float GetProjectileDownwardsAcceleration()
        {
            if (projectileRigidbody == null)
            {
                projectileRigidbody = GetComponent<Rigidbody>();
            }

            return projectileDownwardsForce / projectileRigidbody.mass;
        }

        private void Awake()
        {
            if (projectileRigidbody == null)
            {
                projectileRigidbody = GetComponent<Rigidbody>();
            }

            projectileTransform = transform;

            if (collisionVFX != null)
            {
                SetupObjectPool(collisionVFX, out instantiatedCollisionVFXPool);
            }

            if (expiredVFX != null)
            {
                SetupObjectPool(expiredVFX, out instantiatedExpiredVFXPool);
            }

            if (projectileHitAudioSource != null)
            {
                SetupObjectPool(projectileHitAudioSource, out instantiatedHitAudioSourcePool);
            }

            DecorateLifetimeBehavior();
            DecorateExplosionBehavior();
        }

        private void SetupObjectPool<T>(T objectToPool, out StackObjectPool<T> pool) where T : Component
        {
            pool = new(
                    () => Instantiate(objectToPool, transform.parent),
                    instantiatedObjectToPool => Destroy(instantiatedObjectToPool),
                    1,
                    instantiatedObjectToPool => instantiatedObjectToPool.gameObject.SetActive(true),
                    instantiatedObjectToPool => instantiatedObjectToPool.gameObject.SetActive(false));
        }

        private void FixedUpdate()
        {
            float playerDownwardsForceMultiplier = broadsideCannonsUpgrades != null && broadsideCannonsUpgrades.IncreaseDamageAndWeightDescription.IsUpgraded ? broadsideCannonsUpgrades.DownwardsForceMultiplier : 1f;
            projectileRigidbody.AddForce(projectileDownwardsForce * playerDownwardsForceMultiplier * Vector3.down);
        }

        private void OnDestroy()
        {
            lifeTimeComponent.Dispose();
            explosionComponent.Dispose();
            instantiatedCollisionVFXPool?.Dispose();
            instantiatedExpiredVFXPool?.Dispose();
            instantiatedHitAudioSourcePool?.Dispose();

            OnProjectileDestroyed = null;
            TargetDistanceUpdated = null;
            projectileRigidbody = null;
            projectileTransform = null;
            lifeTimeComponent = null;
            explosionComponent = null;
            instantiatedCollisionVFXPool = null;
            instantiatedExpiredVFXPool = null;
            instantiatedHitAudioSourcePool = null;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isProjectileDestroyed)
            {
                return;
            }

            TryDamageObject(collision.gameObject);
            DisableProjectile(instantiatedCollisionVFXPool);
            PlaySoundOnCollision();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isProjectileDestroyed)
            {
                return;
            }

            bool hasDamagedObject = TryDamageObject(other.gameObject);

            if (hasDamagedObject)
            {
                DisableProjectile(instantiatedCollisionVFXPool);
                PlaySoundOnCollision();
            }
        }

        private void OnTimerExpired()
        {
            if (isProjectileDestroyed)
            {
                return;
            }

            DisableProjectile(instantiatedExpiredVFXPool);
        }

        private void DisableProjectile(StackObjectPool<ParticleSystem> onDisableVFXPool)
        {
            isProjectileDestroyed = true;
            projectileRigidbody.isKinematic = true;
            explosionComponent.Explode();
            StopAllCoroutines();
            TrySetMovementParticlesActive(false);
            projectileGraphicsObject.SetActive(false);

            if (onDisableVFXPool != null)
            {
                var onDisableVFX = onDisableVFXPool.Get();
                onDisableVFX.transform.SetPositionAndRotation(transform.position, transform.rotation);
                onDisableVFX.Play();

                Sequence disableVfxAfterPlayerSequence = DOTween.Sequence();
                disableVfxAfterPlayerSequence.
                    AppendInterval(onDisableVFX.main.duration + onDisableVFX.main.startDelayMultiplier).
                    AppendCallback(() => ReleasePoolableObjectAfterPlayed(onDisableVFX, onDisableVFXPool));
                disableVfxAfterPlayerSequence.Play();
            }

            OnProjectileDestroyed?.Invoke();
        }

        private void PlaySoundOnCollision()
        {
            if (instantiatedHitAudioSourcePool != null)
            {
                var instantiatedHitAudioSource = instantiatedHitAudioSourcePool.Get();
                instantiatedHitAudioSource.transform.position = transform.position;
                instantiatedHitAudioSource.Play();

                Sequence disableAudioSourceAfterPlayed = DOTween.Sequence();
                disableAudioSourceAfterPlayed.
                    AppendInterval(instantiatedHitAudioSource.clip.length).
                    AppendCallback(() => ReleasePoolableObjectAfterPlayed(instantiatedHitAudioSource, instantiatedHitAudioSourcePool));
                disableAudioSourceAfterPlayed.Play();
            }
        }

        private bool TryDamageObject(GameObject objectToDamage)
        {
            if (objectToDamage.TryGetComponent(out IDamageable damageable))
            {
                float playerProjectileUpgradeMultiplier = broadsideCannonsUpgrades != null && broadsideCannonsUpgrades.IncreaseDamageAndWeightDescription.IsUpgraded ? broadsideCannonsUpgrades.DamageMultiplier : 1f;
                damageable.ReceiveDamage(projectileDamage * playerProjectileUpgradeMultiplier);

                if (1 << gameObject.layer is not LayersUtils.PLAYER_PROJECTILE_LAYER and not LayersUtils.DEFAULT_LAYER)
                {
                    return true;
                }

                if (damageable is WeakPointDamageModule weakPoint && !weakPoint.IsInvulnerable || damageable is GunDamageModule)
                {
                    PlayerDealtDamageMessageBroker.InvokeOnCriticalDamageDealt();
                }
                else if (damageable is DamageModule damageModule && !damageModule.IsInvulnerable)
                {
                    PlayerDealtDamageMessageBroker.InvokeOnDamageDealt();
                }

                return true;
            }

            return false;
        }

        private void ReleasePoolableObjectAfterPlayed<T>(T objectToRelease, StackObjectPool<T> pool) where T : Component
        {
            if (pool != null)
            {
                pool.Release(objectToRelease);
            }
            else
            {
                Destroy(objectToRelease);
            }
        }

        private void DecorateLifetimeBehavior()
        {
            lifeTimeComponent = new ProjectileLifetimeConcreteComponent();

            foreach (var lifetimeData in behaviorData.LifetimeDecoratorData)
            {
                if (lifetimeData is ProjectileMaxLifetimeData maxLifetime)
                {
                    lifeTimeComponent = new ProjectileMaxLifetimeDecorator(lifeTimeComponent, this, maxLifetime.ProjectileMaxLifetime, OnTimerExpired);
                }
                else if (lifetimeData is ProjectileTargetProximityLifetimeData playerProximityLifetime)
                {
                    float playerProjectileSpeedMultiplier = broadsideCannonsUpgrades != null && broadsideCannonsUpgrades.DecreaseSpreadAndIncreaseSpeedDescription.IsUpgraded ? broadsideCannonsUpgrades.SpeedMultiplier : 1f;

                    ProjectileTargetProximityLifetimeDecorator targetProximityLifetimeDecorator = new(
                        lifeTimeComponent,
                        this,
                        playerProximityLifetime.MinLifetime,
                        playerProximityLifetime.LowRandomLifetimeRange,
                        playerProximityLifetime.HighRandomLifetimeRange,
                        projectileSpeed * playerProjectileSpeedMultiplier,
                        OnTimerExpired);
                    TargetDistanceUpdated += targetProximityLifetimeDecorator.OnTargetDistanceUpdated;
                    lifeTimeComponent = targetProximityLifetimeDecorator;
                }
            }
        }

        private void DecorateExplosionBehavior()
        {
            explosionComponent = new ProjectileExplosionConcreteComponent();

            foreach (var explosionData in behaviorData.ExplosionDecoratorData)
            {
                if (explosionData is ProjectileSingleExplosionDecoratorData singleExplosion)
                {
                    explosionComponent = new ProjectileSingleExplosionDecorator(explosionComponent, projectileTransform, singleExplosion.ExplosionRadius, singleExplosion.ExlposionDamage, singleExplosion.ExplosionParticles, PlaySoundOnCollision);
                }
            }
        }

        private void TrySetMovementParticlesActive(bool isActive)
        {
            if (movementVFX != null)
            {
                movementVFX.gameObject.SetActive(isActive);
            }
        }
    }
}
