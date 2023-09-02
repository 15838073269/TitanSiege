/****************************************************
    文件：UIBG.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/2 22:14:50
	功能：背景级ui，任何ui都可以遮挡它
*****************************************************/

using GF.Unity.UI;
using UnityEngine;

public class UIBG : UIPanel {
    public void Start() {
        Layer = 0;
    }

   
}