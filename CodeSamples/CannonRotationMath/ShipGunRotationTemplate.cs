using System;
using UnityEngine;

namespace Voidwalker.Ship.Guns.Rotation
{
    public abstract class ShipGunRotationTemplate : IDisposable
    {
        public float DistanceToTarget { get; protected set; }

        protected Vector3 targetDirection;
        protected float angleY = 0f;
        protected float angleX = 0f;
        private float lerpedAngleY = 0f;
        private float lerpedAngleX = 0f;
        private IShipGunRotationCompatible rotation;
        private Quaternion rotationX;
        private Quaternion rotationY;
        private bool isDisposed;

        private const float MAX_Y_ANGLE_CONSTRAINT = 179f;

        public ShipGunRotationTemplate(IShipGunRotationCompatible gunRotation)
        {
            rotation = gunRotation;
        }

        public virtual void SetRotationTarget(Vector3 target)
        {
            DistanceToTarget = (target - rotation.GunTransform.position).magnitude;
            targetDirection = (target - rotation.GunTransform.position) / DistanceToTarget;
        }

        public virtual void TryRotateGun()
        {
            float rotationSpeed = Time.deltaTime * rotation.GunRotationSpeed;

            angleY = CalculateInitialYAngle();

            if (angleY > rotation.HorizontalRotationConstraint)
            {
                angleY = rotation.HorizontalRotationConstraint;
            }
            else if (angleY < -rotation.HorizontalRotationConstraint)
            {
                angleY = -rotation.HorizontalRotationConstraint;
            }

            if (lerpedAngleY > 180 && rotation.HorizontalRotationConstraint < MAX_Y_ANGLE_CONSTRAINT)
            {
                lerpedAngleY = Mathf.MoveTowards(lerpedAngleY - 360f, angleY, rotationSpeed);
            }
            else if (rotation.HorizontalRotationConstraint >= MAX_Y_ANGLE_CONSTRAINT)
            {
                lerpedAngleY = Mathf.MoveTowardsAngle(lerpedAngleY, angleY, rotationSpeed);
            }
            else
            {
                lerpedAngleY = Mathf.MoveTowards(lerpedAngleY, angleY, rotationSpeed);
            }

            angleX = CalculateInitialXAngle();

            if (angleX > rotation.VerticalRotationConstraint)
            {
                angleX = rotation.VerticalRotationConstraint;
            }
            else if (angleX < -rotation.VerticalRotationConstraint)
            {
                angleX = -rotation.VerticalRotationConstraint;
            }

            lerpedAngleX = Mathf.MoveTowardsAngle(lerpedAngleX, angleX, rotationSpeed);

            rotationY.Set(0f, Mathf.Sin(lerpedAngleY * Mathf.Deg2Rad / 2), 0f, Mathf.Cos(lerpedAngleY * Mathf.Deg2Rad / 2));
            rotationX.Set(Mathf.Sin(lerpedAngleX * Mathf.Deg2Rad / 2), 0f, 0f, Mathf.Cos(lerpedAngleX * Mathf.Deg2Rad / 2));
            rotation.GunTransform.localRotation = rotationY * rotationX;
        }

        public virtual bool IsAlignedWithTarget()
        {
            bool isOutOfBoundsX = angleX >= rotation.VerticalRotationConstraint || angleX <= -rotation.VerticalRotationConstraint;
            bool isOutOfBoundsY = angleY >= rotation.HorizontalRotationConstraint || angleY <= -rotation.HorizontalRotationConstraint;

            if (isOutOfBoundsX || isOutOfBoundsY)
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
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
        }

        protected virtual float CalculateInitialYAngle()
        {
            Vector3 localTargetDirection = rotation.GunTransform.parent.InverseTransformDirection(targetDirection);

            return Mathf.Atan2(localTargetDirection.x, localTargetDirection.z) * Mathf.Rad2Deg;
        }

        protected virtual float CalculateInitialXAngle()
        {
            Vector3 localTargetDirection = rotation.GunTransform.parent.InverseTransformDirection(targetDirection);

            return Mathf.Asin(localTargetDirection.y) * Mathf.Rad2Deg * -1f;
        }
    }
}
