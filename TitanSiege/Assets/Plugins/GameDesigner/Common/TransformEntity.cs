using System;
using UnityEngine;

namespace Net.Common
{
    /// <summary>
    /// 新的Trasnform实体类, 性能可能要好点
    /// </summary>
    [Serializable]
    public class TransformEntity
    {
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;

        public TransformEntity()
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Vector3 LocalScale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Quaternion LocalRotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 EulerAngles
        {
            get { return rotation.eulerAngles; }
            set { rotation = Quaternion.Euler(value); }
        }

        public Vector3 Left
        {
            get { return -rotation * Vector3.right; }
        }

        public Vector3 Right
        {
            get { return rotation * Vector3.right; }
        }

        public Vector3 Up
        {
            get { return rotation * Vector3.up; }
        }

        public Vector3 Down
        {
            get { return -rotation * Vector3.up; }
        }

        public Vector3 Forward
        {
            get { return rotation * Vector3.forward; }
        }

        public Vector3 Back
        {
            get { return -rotation * Vector3.forward; }
        }

        public void Translate(Vector3 translation, Space relativeTo = Space.Self)
        {
            if (relativeTo == Space.Self)
            {
                position += rotation * translation;
            }
            else
            {
                position += translation;
            }
        }

        public void Rotate(Vector3 eulerRotation)
        {
            rotation *= Quaternion.Euler(eulerRotation);
        }

        public void ScaleBy(Vector3 scaleFactor)
        {
            scale *= scaleFactor;
        }

        public void LookAt(Vector3 targetPosition)
        {
            var direction = targetPosition - position;
            rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}