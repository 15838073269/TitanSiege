using System;
using System.Collections.Generic;
using Net.Serialize;

namespace Binding
{
    public class BindingType : IBindingType
    {
		public int SortingOrder { get; } = 0;
		public Dictionary<Type, Type> BindTypes { get; } = new Dictionary<Type, Type>
        {
			{ typeof(Net.Vector2), typeof(NetVector2Bind) },
			{ typeof(Net.Vector2[]), typeof(NetVector2ArrayBind) },
			{ typeof(List<Net.Vector2>), typeof(NetVector2GenericBind) },
			{ typeof(Net.Vector3), typeof(NetVector3Bind) },
			{ typeof(Net.Vector3[]), typeof(NetVector3ArrayBind) },
			{ typeof(List<Net.Vector3>), typeof(NetVector3GenericBind) },
			{ typeof(Net.Vector4), typeof(NetVector4Bind) },
			{ typeof(Net.Vector4[]), typeof(NetVector4ArrayBind) },
			{ typeof(List<Net.Vector4>), typeof(NetVector4GenericBind) },
			{ typeof(Net.Quaternion), typeof(NetQuaternionBind) },
			{ typeof(Net.Quaternion[]), typeof(NetQuaternionArrayBind) },
			{ typeof(List<Net.Quaternion>), typeof(NetQuaternionGenericBind) },
			{ typeof(Net.Rect), typeof(NetRectBind) },
			{ typeof(Net.Rect[]), typeof(NetRectArrayBind) },
			{ typeof(List<Net.Rect>), typeof(NetRectGenericBind) },
			{ typeof(Net.Color), typeof(NetColorBind) },
			{ typeof(Net.Color[]), typeof(NetColorArrayBind) },
			{ typeof(List<Net.Color>), typeof(NetColorGenericBind) },
			{ typeof(Net.Color32), typeof(NetColor32Bind) },
			{ typeof(Net.Color32[]), typeof(NetColor32ArrayBind) },
			{ typeof(List<Net.Color32>), typeof(NetColor32GenericBind) },
			{ typeof(UnityEngine.Vector2), typeof(UnityEngineVector2Bind) },
			{ typeof(UnityEngine.Vector2[]), typeof(UnityEngineVector2ArrayBind) },
			{ typeof(List<UnityEngine.Vector2>), typeof(UnityEngineVector2GenericBind) },
			{ typeof(UnityEngine.Vector3), typeof(UnityEngineVector3Bind) },
			{ typeof(UnityEngine.Vector3[]), typeof(UnityEngineVector3ArrayBind) },
			{ typeof(List<UnityEngine.Vector3>), typeof(UnityEngineVector3GenericBind) },
			{ typeof(UnityEngine.Vector4), typeof(UnityEngineVector4Bind) },
			{ typeof(UnityEngine.Vector4[]), typeof(UnityEngineVector4ArrayBind) },
			{ typeof(List<UnityEngine.Vector4>), typeof(UnityEngineVector4GenericBind) },
			{ typeof(UnityEngine.Quaternion), typeof(UnityEngineQuaternionBind) },
			{ typeof(UnityEngine.Quaternion[]), typeof(UnityEngineQuaternionArrayBind) },
			{ typeof(List<UnityEngine.Quaternion>), typeof(UnityEngineQuaternionGenericBind) },
			{ typeof(UnityEngine.Rect), typeof(UnityEngineRectBind) },
			{ typeof(UnityEngine.Rect[]), typeof(UnityEngineRectArrayBind) },
			{ typeof(List<UnityEngine.Rect>), typeof(UnityEngineRectGenericBind) },
			{ typeof(UnityEngine.Color), typeof(UnityEngineColorBind) },
			{ typeof(UnityEngine.Color[]), typeof(UnityEngineColorArrayBind) },
			{ typeof(List<UnityEngine.Color>), typeof(UnityEngineColorGenericBind) },
			{ typeof(UnityEngine.Color32), typeof(UnityEngineColor32Bind) },
			{ typeof(UnityEngine.Color32[]), typeof(UnityEngineColor32ArrayBind) },
			{ typeof(List<UnityEngine.Color32>), typeof(UnityEngineColor32GenericBind) },
			{ typeof(Net.Share.Operation), typeof(NetShareOperationBind) },
			{ typeof(Net.Share.Operation[]), typeof(NetShareOperationArrayBind) },
			{ typeof(List<Net.Share.Operation>), typeof(NetShareOperationGenericBind) },
			{ typeof(Net.Share.OperationList), typeof(NetShareOperationListBind) },
			{ typeof(Net.Share.OperationList[]), typeof(NetShareOperationListArrayBind) },
			{ typeof(List<Net.Share.OperationList>), typeof(NetShareOperationListGenericBind) },
        };
    }
}
