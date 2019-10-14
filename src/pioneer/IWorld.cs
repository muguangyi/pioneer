/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

using System;

namespace Pioneer
{
    /// <summary>
    /// World interface.
    /// </summary>
    public interface IWorld : IEntityContainer, IDisposable
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
        /// Add entity template with an unique name.
        /// </summary>
        /// <param name="template">Template name.</param>
        /// <param name="decorator">Decorator func for entity.</param>
        /// <returns>Indicate if the operation is succeeded or not.</returns>
        bool AddEntityTemplate(string template, Action<IEntity> decorator);

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
