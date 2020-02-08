/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;
using System.Collections.Generic;

namespace Pioneer
{
    /// <summary>
    /// World interface.
    /// </summary>
    public interface IWorld : ICreator, IActorsBinder, IDisposable
    {
        /// <summary>
        /// Event when the world is loading.
        /// </summary>
        event Action OnLoading;

        /// <summary>
        /// Event when the world is closed.
        /// </summary>
        event Action<Exception> OnClosed;

        /// <summary>
        /// Event when a player actor enters the world.
        /// </summary>
        event Action<IActor> OnPlayerEntered;

        /// <summary>
        /// Event when a player actor exits the world.
        /// </summary>
        event Action<IActor> OnPlayerExited;

        /// <summary>
        /// Gets a value to indicate the actor collection in the world.
        /// </summary>
        IEnumerable<IActor> Actors { get; }

        /// <summary>
        /// Get actor by actor's ID.
        /// </summary>
        /// <param name="actorId">Actor ID.</param>
        /// <returns>Actor instance if exists, otherwise return null.</returns>
        IActor GetActorById(ulong actorId);

        /// <summary>
        /// Add a system.
        /// </summary>
        /// <param name="systemType">System type.</param>
        /// <returns>System instance.</returns>
        System AddSystem(Type systemType);

        /// <summary>
        /// Add a system.
        /// </summary>
        /// <typeparam name="TSystem">System type.</typeparam>
        /// <returns>System instance.</returns>
        TSystem AddSystem<TSystem>() where TSystem : System;

        /// <summary>
        /// Add a system.
        /// </summary>
        /// <param name="systemName">System name.</param>
        /// <returns>System instance.</returns>
        System AddSystem(string systemName);

        /// <summary>
        /// Add actor template with an unique name.
        /// </summary>
        /// <param name="template">Template name.</param>
        /// <param name="decorator">Decorator func for actor.</param>
        /// <returns>Indicate if the operation is succeeded or not.</returns>
        bool TrySetActorTemplate(string template, Action<IActor> decorator);

        /// <summary>
        /// Start the world with network service provider.
        /// </summary>
        /// <param name="nsp">Network service provider.</param>
        void Start(string nsp = null);

        /// <summary>
        /// Stop the world.
        /// </summary>
        void Stop();

        /// <summary>
        /// Update the world.
        /// </summary>
        /// <param name="deltaTime">Delta time from the last frame.</param>
        void Update(float deltaTime);

        /// <summary>
        /// Notify the world has been loaded.
        /// </summary>
        void NotifyLoaded();
    }
}
