namespace Net.Component
{
    using global::System;
    using UnityEngine;
    using Matrix4x4 = Matrix4x4;
    using Quaternion = Quaternion;
    using Vector3 = Vector3;

    /// <summary>
    /// 游戏物体转换实体组建
    /// 作者:彼岸流年  QQ:317392507
    /// 后期修改:龙兄 QQ:1752062104
    /// </summary>
    [Serializable]
    public class EntityTransform
    {
        public Matrix4x4 matrix;

        public Vector3 position
        {
            get => matrix.GetPosition();
            set => Matrix4Utils.SetPosition(ref matrix, value);
        }

        public Quaternion rotation
        {
            get => matrix.GetRotation();
            set => Matrix4Utils.Rotate(ref matrix, value);
        }

        public UnityEngine.Vector3 localScale
        {
            get => matrix.GetScale();
        }

        public UnityEngine.Quaternion localRotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 eulerAngles
        {
            get => rotation.eulerAngles;
            set => rotation = Quaternion.Euler(value);
        }

        public Vector3 left
        {
            get => matrix.left;
            set => matrix.left = value;
        }

        public Vector3 right
        {
            get => matrix.right;
            set => matrix.right = value;
        }

        public Vector3 up
        {
            get => matrix.up;
            set => matrix.up = value;
        }

        public Vector3 down
        {
            get => matrix.down;
            set => matrix.down = value;
        }

        public Vector3 forward
        {
            get => matrix.forward;
            set => matrix.forward = value;
        }

        public Vector3 back
        {
            get => matrix.back;
            set => matrix.back = value;
        }

        public EntityTransform()
        {
            matrix = Matrix4Utils.GetPosition(Vector3.zero);
        }

        public EntityTransform(Vector3 position, Quaternion rotation)
        {
            Matrix4Utils.SetPosition(ref matrix, position);
            Matrix4Utils.Rotate(ref matrix, rotation);
        }

        public void Translate(float x, float y, float z)
        {
            Translate(new Vector3(x, y, z));
        }

        public void Translate(Vector3 direction)
        {
            Translate(direction, Space.Self);
        }

        public void Translate(Vector3 translation, Space relativeTo)
        {
            if (relativeTo == Space.World)
            {
                position += translation;
            }
            else
            {
                matrix *= Matrix4x4.Translate(translation);
            }
        }

        public void Rotate(Vector3 eulers, Space relativeTo)
        {
            var rhs = Quaternion.Euler(eulers.x, eulers.y, eulers.z);
            if (relativeTo == Space.Self)
            {
                matrix *= Matrix4x4.Rotate(rhs);
            }
            else
            {
                rotation *= Quaternion.Inverse(rotation) * rhs * rotation;
            }
        }

        public void Rotate(Vector3 eulers)
        {
            Rotate(eulers, Space.Self);
        }

        public void Rotate(float xAngle, float yAngle, float zAngle, Space relativeTo)
        {
            Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
        }

        public void Rotate(float xAngle, float yAngle, float zAngle)
        {
            Rotate(new Vector3(xAngle, yAngle, zAngle), Space.Self);
        }

        public void LookAt(Vector3 worldPosition)
        {
            LookAt(worldPosition, Vector3.up);
        }

        public void LookAt(Vector3 worldPosition, Vector3 worldUp)
        {
            rotation = Quaternion.LookRotation(position, worldPosition, worldUp);
        }
    }

    [Serializable]
    public class NTransform : EntityTransform 
    {
        public NTransform() : base() { }

        public NTransform(Vector3 position, Quaternion rotation) : base(position, rotation) { }
    }
}