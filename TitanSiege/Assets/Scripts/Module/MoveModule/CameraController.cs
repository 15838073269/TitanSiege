
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
namespace GF.MainGame.Module {
    public class CameraController : MonoBehaviour {
        private Transform target;//控制角色

        private Vector3 offset;//摄像机和角色的偏差
        public bool IsCanFollow = false;
        public bool IsInBigMap = true;

        public void BeginFollow(Transform npc) {
            target = npc;
            offset = target.position - transform.position;
        }

        void LateUpdate() {
            if (IsControl) {
                //如果没有得到中心模块则此脚本无效
                if (target == null)
                    return;
                ////初始坐标还原点
                //LPosition = transform.position;
                ////初始中心物体坐标还原点
                //LTPosition = target.position;

                if (IsCanFollow) {
                    //if (IsInBigMap) {//在大地图上相机跟随限制和旋转存在bug，暂时没法解决，暂时不添加限制，考虑大地图上禁止旋转
                    //    Vector3 newvector = target.position - offset;
                    //    transform.position = new Vector3(Mathf.Clamp(newvector.x, 6.5f, 34f), newvector.y, Mathf.Clamp(newvector.z, -1.5f, 30.6f));
                    //} else {
                        Vector3 newvector = target.position - offset;
                        transform.position = newvector;
                    //}
                }
            }
        }
        public float StartRoate;
        public void SetStartRoate() {//设置初始旋转
            StartRoate = transform.localEulerAngles.y;
            IsCanRotate = true;
        }
        public void SetCameraRoate(float x){//拖动旋转
            transform.RotateAround(target.transform.position, Vector3.down, x* xSpeed * Time.deltaTime);
            target.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));//相机目标的旋转和相机保持一致
            offset = target.position - transform.position;
            IsCanRotate = false;
        }
        public bool IsControl = true;
        //-----------------  初始参数  -----------------
        //--旋转速度
        private float xSpeed = 0.372f;

        //--是否允许旋转相机
        public bool IsCanRotate = false;


        //是否跟随事件移动
        public  bool IsFollow = false;
        //-----------------  无需改动，中间变量  -----------------

        //旋转角度（二维屏幕）
        public  float x = 90.0f;
        public  float y = 45.0f;
        public  float z = 0.0f;
        ////坐标还原点
        //private Vector3 LPosition;
        ////中心物体坐标还原点
        //private Vector3 LTPosition;

        public void ShakeCamera() {
            transform.DOShakeRotation(0.8f,0.4f);
        }
        public void Start() {
            AppTools.Regist((int)MoveEvent.ShakeCamera, ShakeCamera);
        }
        public void OnDestroy() {
            AppTools.Remove((int)MoveEvent.ShakeCamera, ShakeCamera);
        }
    }
}

