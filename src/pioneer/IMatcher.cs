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
    /// Matcher interface.
    /// </summary>
    public interface IMatcher
    {
        /// <summary>
        /// Ensure the actor contains the target trait.
        /// </summary>
        /// <param name="traitType">Trait type.</param>
        /// <returns>Matcher instance.</returns>
        IMatcher HasTrait(Type traitType);

        /// <summary>
        /// Ensure the actor contains the target trait.
        /// </summary>
        /// <typeparam name="TTrait">Trait type.</typeparam>
        /// <returns>Matcher instance.</returns>
        IMatcher HasTrait<TTrait>() where TTrait : ITrait;

        /// <summary>
        /// Ensure the actor contains the target trait.
        /// </summary>
        /// <param name="traitName">Trait name.</param>
        /// <returns>Matcher instance.</returns>
        IMatcher HasTrait(string traitName);

        /// <summary>
        /// Ensure the actor contains the target tag.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <returns>Matcher instance.</returns>
        IMatcher HasTag(string tag);

        /// <summary>
        /// Ensure the actor doesn't include the target trait.
        /// </summary>
        /// <param name="traitType">Trait type.</param>
        /// <returns>Matcher instance.</returns>
        IMatcher ExceptTrait(Type traitType);

        /// <summary>
        /// Ensure the actor doesn't include the target trait.
        /// </summary>
        /// <typeparam name="TTrait">Trait type.</typeparam>
        /// <returns>Matcher instance.</returns>
        IMatcher ExceptTrait<TTrait>() where TTrait : ITrait;

        /// <summary>
        /// Ensure the actor doesn't include the target trait.
        /// </summary>
        /// <param name="traitName">Trait name.</param>
        /// <returns>Matcher instance.</returns>
        IMatcher ExceptTrait(string traitName);

        /// <summary>
        /// Ensure the actor doesn't include the target tag.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <returns>Matcher instance.</returns>
        IMatcher ExceptTag(string tag);
    }
}
