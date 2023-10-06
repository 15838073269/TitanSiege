/****************************************************
    文件：AppTools.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/10/5 0:36:5
	功能：Nothing
*****************************************************/

using GF.MainGame;
using GF.Module;
using GF.Msg;
using GF.Unity.Audio;
using GF.Unity.UI;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace GF.MainGame {
    public static class AppTools {
        public static int GetModuleId(int enumid) {
            int mdefid = (enumid / AppConfig.ScriptSpan) * AppConfig.ScriptSpan;
            return mdefid;
        }
        #region Aciton Register  Remove  Send
        public static void Regist<T1>(int enumid, Action<T1> actionname) {
            MsgCenter.Register<T1>(GetModuleId(enumid), enumid, actionname);
        }
        public static void Regist(int enumid, Action actionname) {
            MsgCenter.Register(GetModuleId(enumid), enumid, actionname);
        }
        public static void Regist<T1, T2>(int enumid, Action<T1, T2> actionname) {
            MsgCenter.Register<T1, T2>(GetModuleId(enumid), enumid, actionname);
        }
        public static void Regist<T1, T2, T3>(int enumid, Action<T1, T2, T3> actionname) {
            MsgCenter.Register<T1, T2, T3>(GetModuleId(enumid), enumid, actionname);
        }
        public static void Regist<T1, T2, T3, T4>(int enumid, Action<T1, T2, T3, T4> actionname) {
            MsgCenter.Register<T1, T2, T3, T4>(GetModuleId(enumid), enumid, actionname);
        }
        public static void Remove<T1>(int enumid, Action<T1> actionname) {
            MsgCenter.Remove<T1>(GetModuleId(enumid), enumid, actionname);

        }
        public static void Remove(int enumid, Action actionname) {
            MsgCenter.Remove(GetModuleId(enumid), enumid, actionname);
        }
        public static void Remove<T1, T2>(int enumid, Action<T1, T2> actionname) {
            MsgCenter.Remove<T1, T2>(GetModuleId(enumid), enumid, actionname);

        }
        public static void Remove<T1, T2, T3>(int enumid, Action<T1, T2, T3> actionname) {
            MsgCenter.Remove<T1, T2, T3>(GetModuleId(enumid), enumid, actionname);

        }
        public static void Remove<T1, T2, T3, T4>(int enumid, Action<T1, T2, T3, T4> actionname) {
            MsgCenter.Remove<T1, T2, T3, T4>(GetModuleId(enumid), enumid, actionname);

        }

        public static void Send<T1>(int enumid, T1 arg1) {
            MsgCenter.SendMessage<T1>(GetModuleId(enumid), enumid, arg1);
        }
        public static void Send(int enumid) {
            MsgCenter.SendMessage(GetModuleId(enumid), enumid);
        }
        public static void Send<T1, T2>(int enumid, T1 arg1, T2 arg2) {
            MsgCenter.SendMessage<T1, T2>(GetModuleId(enumid), enumid, arg1, arg2);
        }
        public static void Send<T1, T2, T3>(int enumid, T1 arg1, T2 arg2, T3 arg3) {
            MsgCenter.SendMessage<T1, T2, T3>(GetModuleId(enumid), enumid, arg1, arg2, arg3);
        }
        public static void Send<T1, T2, T3, T4>(int enumid, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
            MsgCenter.SendMessage<T1, T2, T3, T4>(GetModuleId(enumid), enumid, arg1, arg2, arg3, arg4);
        }
        #endregion
        #region Func Register remove Send
        public static void Regist<T1>(int enumid, Func<T1> actionname) {
            MsgCenter.Register<T1>(GetModuleId(enumid), enumid, actionname);
        }
        public static void Regist<T1, T2>(int enumid, Func<T1, T2> actionname) {
            MsgCenter.Register<T1, T2>(GetModuleId(enumid), enumid, actionname);
        }
        public static void Regist<T1, T2, T3>(int enumid, Func<T1, T2, T3> actionname) {
            MsgCenter.Register<T1, T2, T3>(GetModuleId(enumid), enumid, actionname);
        }
        public static void Regist<T1, T2, T3, T4>(int enumid, Func<T1, T2, T3, T4> actionname) {
            MsgCenter.Register<T1, T2, T3, T4>(GetModuleId(enumid), enumid, actionname);
        }
        public static T1 SendReturn<T1>(int enumid) {
            return MsgCenter.SendMessageWithReturn<T1>(GetModuleId(enumid), enumid);
        }
        public static T2 SendReturn<T1, T2>(int enumid, T1 arg1) {
            return MsgCenter.SendMessageWithReturn<T1, T2>(GetModuleId(enumid), enumid, arg1);
        }
        public static T3 SendReturn<T1, T2, T3>(int enumid, T1 arg1, T2 arg2) {
            return MsgCenter.SendMessageWithReturn<T1, T2, T3>(GetModuleId(enumid), enumid, arg1, arg2);
        }
        public static T4 SendReturn<T1, T2, T3, T4>(int enumid, T1 arg1, T2 arg2, T3 arg3) {
            return MsgCenter.SendMessageWithReturn<T1, T2, T3, T4>(GetModuleId(enumid), enumid, arg1, arg2, arg3);
        }
        public static void Remove<T1>(int enumid, Func<T1> actionname) {
            MsgCenter.Remove<T1>(GetModuleId(enumid), enumid, actionname);

        }
        public static void Remove<T1, T2>(int enumid, Func<T1, T2> actionname) {
            MsgCenter.Remove<T1, T2>(GetModuleId(enumid), enumid, actionname);

        }
        public static void Remove<T1, T2, T3>(int enumid, Func<T1, T2, T3> actionname) {
            MsgCenter.Remove<T1, T2, T3>(GetModuleId(enumid), enumid, actionname);

        }
        public static void Remove<T1, T2, T3, T4>(int enumid, Func<T1, T2, T3, T4> actionname) {
            MsgCenter.Remove<T1, T2, T3, T4>(GetModuleId(enumid), enumid, actionname);

        }
        #endregion
        public static void RemoveModuleMsg(int moduleid) {
            MsgCenter.ClearModuleMsg(moduleid);

        }
        /// <summary>
        /// 管理模块管理器的封装，方便使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public static T GetModule<T>(MDef m) where T : GeneralModule {
            T tempt = null;
            GeneralModule module = ModuleManager.GetInstance.GetModule(m.ToString());
            if (module != null) {
                tempt = module as T;
            }
            return tempt;
        }
        public static bool HasModule(MDef m) {
            return ModuleManager.GetInstance.HasModule(m.ToString()); ;
        }
        public static T CreateModule<T>(MDef m) where T : GeneralModule {
            return ModuleManager.GetInstance.CreateModule(m.ToString()) as T;
        }
        public static T CreateModule<T, T1>(MDef m, T1 arg) where T : GeneralModule {
            return ModuleManager.GetInstance.CreateModule<T1>(m.ToString(), arg) as T;
        }
        /// <summary>
        /// 播放UI点击的音乐
        /// </summary>
        public static void PlayBtnClick() {
            AudioService.GetInstance.PlayUIMusic("audio/uiClickBtn.mp3");
        }
        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="ConvertString"></param>
        /// <returns></returns>
        public static string GetStrMd5_32X(string ConvertString) {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(ConvertString)));
            t2 = t2.Replace("-", "");
            return t2.ToLower();
        }
        
    }

}