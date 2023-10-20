**注：本项目美术及音乐资源均来源于网络，个人魔改整合，本人不具备资源版权；**

**本项目仅用于我面试找工作使用，以及GDNet社区学习框架使用，不会进行任何商业用途，任何非本人的商业用途与本人无关，如资源涉及侵权，请联系我删除，联系邮件304183153@qq.com；**
# 项目效果预览

<video
src="http://121.89.205.252/titansiege.mp4" controls=""
height=720 
width=1280> 
</video>
# 项目说明
**项目正在开发中目前完成70%，业余时间开发，进度较慢，发出来主要是为了治疗我自己的拖延症。**

**具体的项目指引我会在项目计划功能开发完成后写一个文档出来，目前精力有限，找工作还是头等大事**

**另外，目前项目clone后报错，需要导入GDNet包，包体已在项目内，具体操作指引待补充，也可参考GDNet操作指引导入项目内的包，数据库文件已更新.....**

《神话纪元》是我个人独立开发的一款MMORPG状态同步网络游戏，基于Unity3D和.net6.0，前端是我个人总结的GF游戏框架，后端使用冷月开发的GDNet网络框架(GDNet链接：https://gitee.com/leng_yue/GameDesigner )， 开发的主旨是用来总结自身知识，以及用来找工作面试。
本项目开发过程中受到冷月多次帮助，因此，特将项目共享，用于分享个人学习经验，促进GDNet社区发展，希望更多人关注GDNet。


# 项目结构
>开发环境
前端：Unity3D 2021.3.11f1c2  
服务端：CneterOS 7.4 + Nginx 1.22 + Mysql 5.4 + .Net 6.0   
开发环境：.Net 6.0 SDK +.NetFrameWork 4.6.1+MySql5.4   
本地测试话，建议安装wamp,个人喜欢使用phpmyadmin操作mysql   
外网服务器配置采用Nginx1.22，安装--with-stream模块，配置Socket转发，安装参数：
configure arguments: --add-module=/www/server/nginx/src/lua_nginx_module --with-stream --with-stream_ssl_module --with-stream_ssl_preread_module。

TitanSiege文件夹为Unity3D项目

GameFrameLite文件夹为个人开发的GF前端游戏框架，个人知识总结，Unity3D项目中已包含打包好的DLL，自用，大佬勿喷

GDServer文件夹为基于GDNet的服务端项目

GameDesigner文件夹为GDNet框架源码（已经更新了最新版本，完整源码参考GDNet链接：https://gitee.com/leng_yue/GameDesigner ）

Tools文件夹中目前仅包含冷月开发的mysql工具

详细子目录介绍待补充......
# 项目规划及进度
因本项目仅用于本人找工作及学习，功能规划不多，仅实现MMORPG基本的游戏功能，不包含完整的游戏剧情文本及全地图场景，支付系统以及商城，具体功能如下：

场景管理及切换(已完成)

AssetBundle动态资源加载（已完成）

配置文件管理（已完成）

登录注册(已完成)

角色创建删除(已完成)

主场景及UI(80%)

状态同步(已实现玩家，玩家技能，怪物行为的同步，以及基础战斗同步)

战斗UI(已完成)

技能系统(已完成)

战斗系统(已完成)

NPC系统(待开始)

剧情任务系统(待开始)

背包道具系统(已完成)

聊天系统(待开始)

好友系统(待开始)

接入hybridclr(待开始)

# 游戏效果预览
清参考游戏进度预览图文件夹中的图片

或者参考子目录的“现阶段完成功能演示视频”