# Pioneer

[![Build status](https://ci.appveyor.com/api/projects/status/s49oscdiivqkl5x7?svg=true)](https://ci.appveyor.com/project/muguangyi/pioneer)[![codecov](https://codecov.io/gh/muguangyi/pioneer/branch/master/graph/badge.svg)](https://codecov.io/gh/muguangyi/pioneer)

***

> 纯C#版的ATCS及网络同步框架。

**Pioneer**包含两大概念：`ATCS开发框架`以及`网络同步框架`。

## ATCS开发框架

`ATCS`结合了现在比较流行的**ECS**以及传统的**MVC**模式，抽取其中最核心的思想：

> 行为者（`Actor`）为容器，数据（`Trait`）与逻辑（`Control`，`System`）分离。

* 行为者（`Actor`）只作为容器。
* 数据（`Trait`）只挂接在行为者上。
* 逻辑组件不应存在数据。
* `Control`为单行为者逻辑组件，而`System`为行为者集合逻辑组件。

ATCS (`Actor` + `Trait` + `Control` + `System`)可以组合出不同的开发方案：

* `Actor` + `Trait` + `System`相当于**ECS**模式。
* `Actor` + `Trait` + `Control`相当于Unity中的**GameObject** + **Component**模式。

当然你也可以在一个项目中同时混合使用这两种模式。

## 网络同步框架

`网络同步框架`基于一套代码到处运行的思想，隐藏底层网络传输，在应用层进行跨网络的数据同步定义。

> 可参考**UE4**的游戏开发框架。

## 快速开始

一个简单的例子展示如何实现`ECS`模式。

### 创建 IWorld

```csharp
IWorld world = Pioneer.Pioneer.New()
...
// 更新 world。
world.Update(deltaTime);
...
// 销毁 world。
world.Dispose();
```

### 创建 IActor

```csharp
IActor actor = world.CreateActor();
```

### 添加 Trait

```csharp
public class MyTrait : Pioneer.Trait
{
    public string Name;
}

actor.AddTrait<MyTrait>().Name = "User";
```

### 添加 System

```csharp
pubic class MySystem : Pioneer.System
{
    private IGroupFilter filter = null;

    public override void OnInit(IWorld world)
    {
        // 设定过滤条件
        IMatcher matcher = world.NewMatcher().HasTrait<MyTrait>();
        this.filter = world.GetFilter(this, TupleType.Job, matcher);
    }

    public override void OnUpdate(IWorld world, float deltaTime)
    {
        foreach (var a in this.filter.Actors)
        {
            // 处理满足条件的Actor。
        }
    }
}

world.AddSystem<MySystem>();
```