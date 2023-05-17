-- phpMyAdmin SQL Dump
-- version 4.1.14
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: 2023-04-13 11:07:10
-- 服务器版本： 5.6.17
-- PHP Version: 5.5.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `titansiege`
--

-- --------------------------------------------------------

--
-- 表的结构 `characters`
--

CREATE TABLE IF NOT EXISTS `characters` (
  `ID` bigint(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Zhiye` tinyint(4) NOT NULL DEFAULT '0' COMMENT '职业',
  `Level` tinyint(4) NOT NULL DEFAULT '1' COMMENT '等级',
  `Exp` int(11) NOT NULL DEFAULT '0' COMMENT '经验',
  `Shengming` int(11) NOT NULL DEFAULT '100' COMMENT '生命',
  `Fali` int(11) NOT NULL DEFAULT '100' COMMENT '法力',
  `Tizhi` smallint(6) NOT NULL DEFAULT '1' COMMENT '体质',
  `Liliang` smallint(6) NOT NULL DEFAULT '1' COMMENT '力量',
  `Minjie` smallint(6) NOT NULL DEFAULT '1' COMMENT '敏捷',
  `Moli` smallint(6) NOT NULL DEFAULT '1' COMMENT '魔力',
  `Meili` smallint(6) NOT NULL DEFAULT '1' COMMENT '魅力',
  `Xingyun` smallint(6) NOT NULL DEFAULT '1' COMMENT '幸运',
  `Lianjin` smallint(6) NOT NULL DEFAULT '0' COMMENT '炼金',
  `Duanzao` smallint(6) NOT NULL DEFAULT '0' COMMENT '锻造',
  `Jinbi` int(11) NOT NULL DEFAULT '0' COMMENT '金币',
  `Zuanshi` int(11) NOT NULL DEFAULT '0' COMMENT '钻石',
  `Chenghao` varchar(50) DEFAULT NULL COMMENT '称号',
  `Friends` varchar(400) DEFAULT NULL COMMENT '亲朋',
  `Skills` varchar(200) DEFAULT NULL COMMENT '技能',
  `Prefabpath` varchar(100) DEFAULT NULL COMMENT '预制体路径',
  `Headpath` varchar(100) DEFAULT NULL COMMENT '头像路径',
  `Lihuipath` varchar(100) DEFAULT NULL COMMENT '立绘路径',
  `Wuqi` smallint(6) NOT NULL DEFAULT '-1' COMMENT '武器',
  `Toukui` smallint(6) NOT NULL DEFAULT '-1' COMMENT '头盔',
  `Yifu` smallint(6) NOT NULL DEFAULT '-1' COMMENT '衣服',
  `Xiezi` smallint(6) NOT NULL DEFAULT '-1' COMMENT '鞋子',
  `MapID` int(11) NOT NULL DEFAULT '0',
  `MapPosX` int(11) NOT NULL DEFAULT '0',
  `MapPosY` int(11) NOT NULL DEFAULT '0',
  `MapPosZ` int(11) NOT NULL DEFAULT '0',
  `Uid` bigint(11) NOT NULL,
  `LastDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '最后登录时间',
  `DelRole` tinyint(1) NOT NULL DEFAULT '0' COMMENT '是否删除',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- 转存表中的数据 `characters`
--

INSERT INTO `characters` (`ID`, `Name`, `Zhiye`, `Level`, `Exp`, `Shengming`, `Fali`, `Tizhi`, `Liliang`, `Minjie`, `Moli`, `Meili`, `Xingyun`, `Lianjin`, `Duanzao`, `Jinbi`, `Zuanshi`, `Chenghao`, `Friends`, `Skills`, `Prefabpath`, `Headpath`, `Lihuipath`, `Wuqi`, `Toukui`, `Yifu`, `Xiezi`, `MapID`, `MapPosX`, `MapPosY`, `MapPosZ`, `Uid`, `LastDate`, `DelRole`) VALUES
(1, '天海无双', 0, 1, 0, 500, 100, 8, 8, 6, 2, 1, 1, 0, 0, 500, 100, '无名菜鸟', '0|', '5|1|2|3|4|', 'NPCPrefab/jianshi.prefab', 'UIRes/head/jianshi.png', 'UIRes/fight/fightlihui/1.png', -1, -1, -1, -1, 0, 0, 0, 0, 1, '2022-11-18 23:16:31', 0),
(2, '豆新雨', 2, 1, 0, 300, 300, 6, 4, 8, 4, 1, 1, 0, 0, 500, 100, '无名菜鸟', '0|', '9|10|11|12|', 'NPCPrefab/longnv.prefab', 'UIRes/head/longnv.png', 'UIRes/fight/fightlihui/1.png', -1, -1, -1, -1, 0, 0, 0, 0, 1, '2022-11-19 00:30:25', 0),
(3, '沉默u', 0, 1, 0, 500, 100, 8, 8, 6, 2, 1, 1, 0, 0, 500, 100, '无名菜鸟', '0|', '5|1|2|3|4|', 'NPCPrefab/jianshi.prefab', 'UIRes/head/jianshi.png', 'UIRes/fight/fightlihui/1.png', -1, -1, -1, -1, 0, 0, 0, 0, 2, '2022-12-11 10:58:28', 0);

-- --------------------------------------------------------

--
-- 表的结构 `config`
--

CREATE TABLE IF NOT EXISTS `config` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '表id',
  `tname` varchar(20) CHARACTER SET utf8mb4 NOT NULL COMMENT '表名称',
  `count` int(11) NOT NULL COMMENT '表计数',
  `describle` varchar(200) CHARACTER SET utf8mb4 DEFAULT NULL COMMENT '表描述',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 COMMENT='配置表，将数据库所有数据表配置到这里面备用' AUTO_INCREMENT=3 ;

--
-- 转存表中的数据 `config`
--

INSERT INTO `config` (`id`, `tname`, `count`, `describle`) VALUES
(1, 'users', 2, '玩家账号信息表'),
(2, 'characters', 3, '玩家角色信息表');

-- --------------------------------------------------------

--
-- 表的结构 `users`
--

CREATE TABLE IF NOT EXISTS `users` (
  `ID` bigint(20) NOT NULL,
  `Username` varchar(50) NOT NULL,
  `Password` varchar(50) NOT NULL,
  `RegisterDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `Email` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID` (`ID`),
  UNIQUE KEY `Username_2` (`Username`),
  UNIQUE KEY `Email` (`Email`),
  KEY `Username` (`Username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- 转存表中的数据 `users`
--

INSERT INTO `users` (`ID`, `Username`, `Password`, `RegisterDate`, `Email`) VALUES
(1, '1', '1', '2022-11-18 23:11:03', '1'),
(2, '2', '2', '2022-12-11 10:58:20', '2');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
