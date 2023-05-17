/****************************************************
    文件：InfoUIPage.cs
	作者：昔莘
    邮箱: 304183153@qq.com
    日期：2022/3/7 14:22:42
	功能：Nothing
*****************************************************/

using GF.MainGame.Data;
using GF.MainGame.Module;
using GF.Unity.UI;
using UnityEngine.UI;
namespace GF.MainGame.UI {
    public class InfoUIPage : UIPage {
        #region ui拖拽
        public Button opentianfubtn;
        public Image tianfushuxing;
        public Button closetianfubtn;
        //ID
        //public Text id;
        //名称
        public Text xingming;
        //年龄
        public Text age;
        //生命
        public Text hp;
        //灵力
        public Text mp;
        //等级
        public Text lv;
        //臂力//加层外功伤害，外功减伤
        public Text bili;
        //根骨//加层防御和生命值
        public Text gengu;
        //神识//加强暴击几率，加强功法特殊效果的出现几率，例如功法加力，起死回生概率提升，致盲，暴击，连击等
        public Text shenshi;
        //身法//加层闪避和命中，战斗移动范围，加层战斗条速度
        public Text shenfa;
        //元魂//加层法术类伤害，法术减伤和灵力值
        public Text yuanhun;
        //资质
        public Text zizhi;
        //魅力
        public Text meili;
        //福源
        public Text fuyuan;
        //医道
        public Text yidao;
        //天工
        public Text tiangong;
        //拳脚
        public Text quanjiao;
        //兵器
        public Text bingqi;
        //剑道
        public Text jiandao;
        //木灵根
        public Text mulg;
        //水灵根
        public Text shuilg;
        //土灵根
        public Text tulg;
        //火灵根
        public Text huolg;
        //雷灵根
        public Text leilg;
        //鬼道//加成鬼道标识类武学，例如冥王决
        public Text guidao;
        //佛法//加成佛门标识类的武学，例如罗汉神功
        public Text fofa;
        //道法//加成道门标识类的武学，例如先天道藏
        public Text daofa;
        //儒家//加成道门标识类的武学，例如先天道藏
        public Text rujia;
        //毒蛊//加成道门标识类的武学，例如先天道藏
        public Text dugu;
        //称号
        public Text chenghao;
        //性别
        public Text xingbie;
        //性格
        public Text xingge;//相性和性格随机
        //相性
        public Text xiangxing;
        //正邪//正数为正，负数为邪，正邪可以改变相性，每提升或者下降20正邪，就上升或者下降一个相性品级
        public Text zhengxie;
        #endregion
        #region 需要计算的UI数据
        //外功攻击
        public Text wgongji;
        //术法攻击
        public Text sgongji;
        //防御
        public Text fangyu;
        //外功减伤
        public Text wjianshang;
        //术法减伤
        public Text sjianshang;
        //命中
        public Text mingzhong;
        //闪避
        public Text shanbi;
        //暴击
        public Text baoji;
        //暴击率
        public Text baojilv;
        //抗暴系数
        public Text kaobaoxishu;
        #endregion
        //配置表的数据，读取存档的也是这个数据
        private NPCDataBase nb;
        public override void Close(bool bClear = false, object arg = null) {
            base.Close(bClear, arg);
        }

        public void Start() {
            closetianfubtn.onClick.AddListener(CloseTianfu);
            opentianfubtn.onClick.AddListener(ShowTianfu);
        }

        protected override void OnClose(bool bClear = false, object arg = null) {
            base.OnClose(bClear, arg);
            AppTools.Send<bool>((int)MoveEvent.OpenHeadCamera,false);
        }

     
        protected override void OnOpen(object args = null) {
            base.OnOpen(args);
            nb = args as NPCDataBase;
            AppTools.Send<bool>((int)MoveEvent.OpenHeadCamera, true);
            PiLiang();
            tianfushuxing.gameObject.SetActive(false);
        }
        private void ShowTianfu() {
            tianfushuxing.gameObject.SetActive(true);
        }
        private void CloseTianfu() {
            tianfushuxing.gameObject.SetActive(false);
        }
        private void PiLiang(NPCDataBase nbarg = null) {
            if (nbarg!=null) {
                nb = nbarg;
            }
            //xingming.text = nb.name;
            //age.text = nb.age+"";
            //hp.text = nb.hp + "";
            //mp.text = nb.mp + "";
            //lv.text = nb.lv + "";
            //bili.text = nb.bili + "";
            //gengu.text = nb.gengu + "";
            //shenshi.text = nb.shenshi + "";
            //shenfa.text = nb.shenfa + "";
            //yuanhun.text = nb.yuanhun + "";
            //zizhi.text = nb.zizhi + "";
            //meili.text = nb.meili + "";
            //fuyuan.text = nb.fuyuan + "";
            //yidao.text = nb.yidao + "";
            //tiangong.text = nb.tiangong + "";
            //quanjiao.text = nb.quanjiao + "";
            //bingqi.text = nb.bingqi + "";
            //jiandao.text = nb.jiandao + "";
            //mulg.text = nb.mulg + "";
            //shuilg.text = nb.shuilg + "";
            //tulg.text = nb.tulg + "";
            //huolg.text = nb.huolg + "";
            //leilg.text = nb.leilg + "";
            //guidao.text = nb.guidao + "";
            //fofa.text = nb.fofa + "";
            //daofa.text = nb.daofa + "";
            //rujia.text = nb.rujia + "";
            //dugu.text = nb.dugu + "";
            //chenghao.text = nb.chenghao + "";
            //xingbie.text = ((Xingbie)nb.xingbie).ToString();
            //xingge.text = ((Xingge)nb.xingge).ToString();
            //xiangxing.text = ((Xiangxing)nb.xiangxing).ToString();
            //zhengxie.text = nb.zhengxie + "";
        }
        private void PiLiangJS(NPCDataBase nbarg = null) { 
            
        }
    }
}
