/****************************************************
    文件：VersionModule.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2021/6/7 22:20:50
	功能：版本管理更新模块
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GF.Module;
namespace Dark.Module {
    public class VersionModule : GeneralModule {
        public override void Create() {
            base.Create();
            Debug.Log("版本管理模块创建成功！");
        }
    }
}

