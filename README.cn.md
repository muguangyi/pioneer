# Pioneer

[![Build status](https://ci.appveyor.com/api/projects/status/s49oscdiivqkl5x7?svg=true)](https://ci.appveyor.com/project/muguangyi/pioneer)[![codecov](https://codecov.io/gh/muguangyi/pioneer/branch/master/graph/badge.svg)](https://codecov.io/gh/muguangyi/pioneer)

***

> 纯C#版的ETCS及网络同步框架。

**Pioneer**包含两大概念：`ETCS开发框架`以及`网络同步框架`。

## ETCS开发框架

`ETCS`结合了现在比较流行的**ECS**以及传统的**MVC**模式，抽取其中最核心的思想：

> 实体（`Entity`）为容器，数据（`Trait`）与逻辑（`Control`，`System`）分离。

* 实体（`Entity`）只作为容器。
* 数据（`Trait`）只挂接在实体上。
* 逻辑组件不应存在数据。
* `Control`为单实体逻辑组件，而`System`为实体集合逻辑组件。

ETCS (`Entity` + `Trait` + `Control` + `System`)可以组合出不同的开发方案：

* `Entity` + `Trait` + `System`相当于**ECS**模式。
* `Entity` + `Trait` + `Control`相当于Unity中的**GameObject** + **Component**模式。

当然你也可以在一个项目中同时混合使用这两种模式。

## 网络同步框架

`网络同步框架`基于一套代码到处运行的思想，隐藏底层网络传输，在应用层进行跨网络的数据同步定义。

> 可参考**UE4**的游戏开发框架。
