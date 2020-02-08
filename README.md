# Pioneer

[![Build status](https://ci.appveyor.com/api/projects/status/s49oscdiivqkl5x7?svg=true)](https://ci.appveyor.com/project/muguangyi/pioneer)[![codecov](https://codecov.io/gh/muguangyi/pioneer/branch/master/graph/badge.svg)](https://codecov.io/gh/muguangyi/pioneer)

***

> ATCS with network sync framework in pure C#.

**Pioneer** includes two concepts: `ATCS development framework` and `Network synchronize framework`.

## ATCS Development Framework

`ATCS` composes the public **ECS** and the classic **MVC** develop pattern, and abstract the kernal thought:

> `Actor` is a container; Data (`Trait`) and Logic (`Control`, `System`) are separated.

* Actor is only a container.
* Data (`Trait`) is attached to Actor.
* Logic component can't contains Data.
* `Control` is single Actor's logic component, and `System` is a group of Actors' logic component.

ATCS (`Actor` + `Trait` + `Control` + `System`) could compose different development solutions:

* `Actor` + `Trait` + `System` equals **ECS** pattern.
* `Actor` + `Trait` + `Control` equals **GameObject** + **Component** pattern in Unity.

Of cause you could compose two type patterns in one project as well.

## Network Synchronize Framework

`Network Synchronize Framework` is based on the purpose that same code run anywhere. It hides the low level network transfer and could define the data synchronize over network in application level.

> Please refer to **UE4** replication server framework.

## Quick Start

A quick sample show how to implement `ECS` pattern.

### Create IWorld

```csharp
IWorld world = Pioneer.Pioneer.New()
...
// Update world with frame delta time.
world.Update(deltaTime);
...
// Dispose the world.
world.Dispose();
```

### Create IActor

```csharp
IActor actor = world.CreateActor();
```

### Add Trait

```csharp
public class MyTrait : Pioneer.Trait
{
    public string Name;
}

actor.AddTrait<MyTrait>().Name = "User";
```

### Add System

```csharp
pubic class MySystem : Pioneer.System
{
    private IGroupFilter filter = null;

    public override void OnInit(IWorld world)
    {
        // Define filter conditions.
        IMatcher matcher = world.NewMatcher().HasTrait<MyTrait>();
        this.filter = world.GetFilter(this, TupleType.Job, matcher);
    }

    public override void OnUpdate(IWorld world, float deltaTime)
    {
        foreach (var a in this.filter.Actors)
        {
            // Handle actors that satisfy the conditions.
        }
    }
}

world.AddSystem<MySystem>();
```
