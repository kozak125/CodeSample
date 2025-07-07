using UnityEngine;
using Voidwalker.Ship.Guns.Rotation;

namespace Voidwalker.Enemy.Ship.Guns.Rotation
{
    public class EnemyShipGunRotation : ShipGunRotationTemplate
    {
        private IEnemyGunRotationCompatible rotation;
        private bool isDisposed;

        private const int CURVED_SHOT_CALCULATIONS_AMOUNT = 5;
        private const float INITIAL_ANGLE_RADIANS_PREDICTION = 0f;
        private const float ERROR_MARGING_RADIANS = 1f * Mathf.Deg2Rad;

        public EnemyShipGunRotation(IEnemyGunRotationCompatible gunRotation) : base(gunRotation)
        {
            rotation = gunRotation;
        }

        public void SetRotationTarget(out Vector3 targetRotation)
        {
            targetRotation = PredictTarget();
            base.SetRotationTarget(targetRotation);
        }

        public bool HasVerticalAngle()
        {
            return Mathf.Abs(angleX) < rotation.VerticalRotationConstraint;
        }

        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                rotation = null;
            }

            isDisposed = true;

            base.Dispose(disposing);
        }

        protected override float CalculateInitialXAngle()
        {
            if (rotation.ProjectileDownwardsAcceleration == 0f)
            {
                return base.CalculateInitialXAngle();
            }

            float numerator = rotation.ProjectileDownwardsAcceleration * (DistanceToTarget * DistanceToTarget) / (2 * rotation.ProjectileSpeed * rotation.ProjectileSpeed);
            float predictedRadiansAngle = INITIAL_ANGLE_RADIANS_PREDICTION;

            for (int i = 0; i < CURVED_SHOT_CALCULATIONS_AMOUNT; i++)
            {
                float projectileAngleFunction = DistanceToTarget * Mathf.Tan(predictedRadiansAngle) - numerator / Mathf.Pow(Mathf.Cos(predictedRadiansAngle), 2) + (rotation.GunTransform.position.y - rotation.PlayerRuntimeInfo.PlayerTransform.position.y);
                float projectileAngleFunctionDerivative = DistanceToTarget * (1 / Mathf.Pow(Mathf.Cos(predictedRadiansAngle), 2)) + 2 * numerator * Mathf.Sin(predictedRadiansAngle) / Mathf.Pow(Mathf.Cos(predictedRadiansAngle), 3);
                float newPredictedAngle = predictedRadiansAngle - projectileAngleFunction / projectileAngleFunctionDerivative;

                if (Mathf.Abs(newPredictedAngle - predictedRadiansAngle) < ERROR_MARGING_RADIANS)
                {
                    return newPredictedAngle * Mathf.Rad2Deg * -1f;
                }

                predictedRadiansAngle = newPredictedAngle;
            }

            return predictedRadiansAngle * Mathf.Rad2Deg * -1f;
        }

        private Vector3 PredictTarget()
        {
            var tempPosition = rotation.PlayerRuntimeInfo.PlayerTransform.position;

            for (int i = 0; i < 3; i++)
            {
                var timeToReach = (tempPosition - rotation.GunTransform.position).magnitude / rotation.ProjectileSpeed;
                tempPosition = rotation.PlayerRuntimeInfo.PlayerTransform.position + rotation.PlayerRuntimeInfo.PlayerRigidbody.velocity * timeToReach;
            }

            return tempPosition;
        }
    }
}
