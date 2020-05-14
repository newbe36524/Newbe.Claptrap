# Newbe.Claptrap
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

[![build](https://github.com/newbe36524/Newbe.Claptrap/workflows/Claptrap/badge.svg)](https://github.com/newbe36524/Newbe.Claptrap/actions)
[![Codecov](https://img.shields.io/codecov/c/github/newbe36524/Newbe.Claptrap)](https://codecov.io/gh/newbe36524/Newbe.Claptrap)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=newbe36524_Newbe.Claptrap&metric=coverage)](https://sonarcloud.io/dashboard?id=newbe36524_Newbe.Claptrap)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=newbe36524_Newbe.Claptrap&metric=alert_status)](https://sonarcloud.io/dashboard?id=newbe36524_Newbe.Claptrap)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/1fd0e7443364414ca0003dab27f9f9b8)](https://www.codacy.com/manual/472158246/Newbe.Claptrap?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=newbe36524/Newbe.Claptrap&amp;utm_campaign=Badge_Grade)

以`事件溯源`和`Actor模式`作为基本理论的一套服务端开发框架。于此之上，开发者可以更为简单的开发出“分布式”、“可水平扩展”、“可测试性高”的应用系统。

## 当前项目状态

项目规划中，现所有开发在 develop 分支上进行，如对项目感兴趣或愿意参与项目开发，欢迎通过issue与项目组联系。以下是当前的主要情况

Claptrap and it`s Minions are coming.

- 核心功能
  - 时间处理与状态管理
    - [X] 定期保存快照
    - [X] 事件处理异常时恢复状态
    - [ ] 支持事件UID的单独检查
  - 事件存储支持
    - [x] 内存
    - [x] SQLite
    - [ ] Mysql
    - [ ] MSSQL
    - [ ] Postgresql
  - 状态存储支持
    - [x] 内存
    - [x] SQLite
    - [ ] Mysql
    - [ ] MSSQL
    - [ ] Postgresql
  - 全球化与本地化
    - [X] 框架
    - [ ] 替换现有所有文本
    - [X] 支持启动时配置
  - [ ] Minions
  - [ ] 事件处理广播器与接收器
  - 事件与状态序列化
    - [X] Json
  - Claptrap Design
    - [X] 全局 Design
    - [X] 特定 Identity 自定义 Design
    - [X] 启动时变更 Design
    - [X] 导出
    - [ ] 配置文件式设计 Design
  - [ ] 组件化
  - 单元测试
    - [ ] 覆盖率80%以上
    - [ ] 覆盖主要的异常抛出情况
- 开发工具
  - [ ] 项目模板
  - 代码样例
    - [X] [单体式运行](https://github.com/newbe36524/Newbe.Claptrap.Examples/tree/master/src/Newbe.Claptrap.OutofOrleans)
    - [X] [Orleans 结合](https://github.com/newbe36524/Newbe.Claptrap.Examples/tree/master/src/Newbe.Claptrap.ArticleManager)
    - [ ] Minions

## 项目样例

您可以通过<https://github.com/newbe36524/Newbe.Claptrap.Examples>来获取关于该项目的样例代码，以便您了解如何使用该项目。

如果您无法正常的获取 github 上的代码，也可以通过<https://gitee.com/yks/Newbe.Claptrap.Examples>来获取样例代码。

## 参与讨论

- 如果你对该项目感兴趣，你可以通过 [github issues](https://github.com/newbe36524/Newbe.Claptrap/issues) 提交您的看法
- 如果您无法正常访问 github issue，您也可以发送邮件到 newbe-claptrap@googlegroups.com 来参与我们的讨论
- 点击链接QQ交流【Newbe.Claptrap】：<https://jq.qq.com/?_wv=1027&k=5uJGXf5>

## 参考资料

该项目受启发于众多开源项目与博客文章：

- [基于Actor框架Orleans构建的分布式、事件溯源、事件驱动、最终一致性的高性能框架——Ray](https://github.com/RayTale/Ray)
- [Event Sourcing Pattern](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/dn589792%28v%3dpandp.10%29)
- [Event Sourcing Pattern 中文译文](https://www.infoq.cn/article/event-sourcing)
- [Orleans - Distributed Virtual Actor Model](https://github.com/dotnet/orleans)
- [Service Fabric](https://docs.microsoft.com/zh-cn/azure/service-fabric/)
- [ENode 1.0 - Saga的思想与实现](http://www.cnblogs.com/netfocus/p/3149156.html)

## Stargazers over time

[![Stargazers over time](https://starchart.cc/newbe36524/Newbe.Claptrap.svg)](https://starchart.cc/newbe36524/Newbe.Claptrap)
## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://www.newbe.pro"><img src="https://avatars1.githubusercontent.com/u/7685462?v=4" width="100px;" alt=""/><br /><sub><b>Newbe36524</b></sub></a><br /><a href="#infra-newbe36524" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/newbe36524/Newbe.Claptrap/commits?author=newbe36524" title="Tests">⚠️</a> <a href="https://github.com/newbe36524/Newbe.Claptrap/commits?author=newbe36524" title="Code">💻</a></td>
  </tr>
</table>

<!-- markdownlint-enable -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!