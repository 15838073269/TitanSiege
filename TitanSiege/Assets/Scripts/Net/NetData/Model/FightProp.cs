/****************************************************
    文件：FightProp.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2023/9/1 21:25:6
	功能：数据层--服务端公用的战斗属性
*****************************************************/

using System;

[Serializable]
public class FightProp 
{
    #region 战斗属性
    /// <summary>
    /// 默认基础闪避和基础暴击，各职业怪物不同就单独修改
    /// </summary>
    public  float BaseDodge = 0.01f;
    public  float BaseCrit = 0.01f;

    protected int m_Attack;
    public virtual int Attack {
        get {
            return m_Attack;
        }
        set {
            m_Attack = value;
            //通知ui层数据发生变化

        }
    }
    protected int m_Defense;
    public virtual int Defense {
        get {
            return m_Defense;
        }
        set {
            m_Defense = value;
            //通知ui层数据发生变化

        }
    }
    protected float m_Dodge;//闪避
    public virtual float Dodge {
        get {
            return m_Dodge;
        }
        set {
            m_Dodge = value;
            //通知ui层数据发生变化

        }
    }
    protected float m_Crit;//暴击
    public virtual float Crit {
        get {
            return m_Crit;
        }
        set {
            m_Crit = value;
            //通知ui层数据发生变化

        }
    }
    protected int m_FightHp;//战斗时生命
    public virtual int FightHP {
        get {

            return m_FightHp;
        }
        set {

            if (value <= 0) {
                m_FightHp = 0;
                //m_IsDie = true;
                //通知死亡 todo
            } else {
                m_FightHp = value;
            }
            //通知ui层数据发生变化

        }
    }
    protected int m_FightMaxHp;//战斗时最大生命
    public virtual int FightMaxHp {
        get {

            return m_FightMaxHp;
        }
        set {
            m_FightMaxHp = value;
            //通知ui层数据发生变化

        }
    }
    protected int m_FightMagic;//战斗魔力
    public virtual int FightMagic {
        get {
            return m_FightMagic;
        }
        set {
            m_FightMagic = value;
            //通知ui层数据发生变化

        }
    }
    protected int m_FightMaxMagic;//战斗时最大魔力
    public virtual int FightMaxMagic {
        get {
            return m_FightMaxMagic;
        }
        set {
            m_FightMaxMagic = value;
            //通知ui层数据发生变化

        }
    }
    protected int m_FightLevel = 0;//等级
    public virtual int FightLevel {
        get {
            return m_FightLevel;
        }
        set {
            m_FightLevel = value - 1;
            //通知ui层数据发生变化

        }
    }
   
    #endregion
}
/// <summary>
/// 职业枚举
/// </summary>
public enum Zhiye {
    剑士 = 0,
    法师 = 1,
    游侠 = 2,
}