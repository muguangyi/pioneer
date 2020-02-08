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
    /// Actor interface.
    /// </summary>
    public interface IActor : IActorBinder, IControlContainer, IDisposable
    {
        /// <summary>
        /// Gets a value to indicate the actor creator.
        /// </summary>
        ICreator Creator { get; }

        /// <summary>
        /// Gets a value to indicate the world instance.
        /// </summary>
        IWorld World { get; }

        /// <summary>
        /// Add trait.
        /// </summary>
        /// <param name="traitType">Trait type.</param>
        /// <returns>Trait instance.</returns>
        Trait AddTrait(Type traitType);

        /// <summary>
        /// Add trait.
        /// </summary>
        /// <typeparam name="TTrait">Trait type.</typeparam>
        /// <returns>Trait instance.</returns>
        TTrait AddTrait<TTrait>() where TTrait : Trait;

        /// <summary>
        /// Add trait.
        /// </summary>
        /// <param name="traitName">Trait name.</param>
        /// <returns>Trait instance.</returns>
        Trait AddTrait(string traitName);

        /// <summary>
        /// Remove the target trait.
        /// </summary>
        /// <param name="traitType">Trait type.</param>
        /// <returns>Indicate whether the operation is succeeded or not.</returns>
        bool RemoveTrait(Type traitType);

        /// <summary>
        /// Remove the target trait.
        /// </summary>
        /// <typeparam name="TTrait">Trait name.</typeparam>
        /// <returns>Indicate whether the operation is succeeded or not.</returns>
        bool RemoveTrait<TTrait>() where TTrait : Trait;

        /// <summary>
        /// Remove the target trait.
        /// </summary>
        /// <param name="traitName">Trait name.</param>
        /// <returns>Indicate whether the operation is succeeded or not.</returns>
        bool RemoveTrait(string traitName);

        /// <summary>
        /// Replace source trait with the destined trait.
        /// </summary>
        /// <param name="srcType">Source type.</param>
        /// <param name="destType">Destined type.</param>
        /// <returns>Indicate whether the operation is succeeded or not.</returns>
        bool ReplaceTrait(Type srcType, Type destType);

        /// <summary>
        /// Replace source trait with the destined trait.
        /// </summary>
        /// <typeparam name="TSrcTrait">Source type.</typeparam>
        /// <typeparam name="TDestTrait">Destined type.</typeparam>
        /// <returns>Indicate whether the operation is succeeded or not.</returns>
        bool ReplaceTrait<TSrcTrait, TDestTrait>() where TSrcTrait : Trait where TDestTrait : Trait;

        /// <summary>
        /// Replace source trait with the destined trait.
        /// </summary>
        /// <param name="srcName"></param>
        /// <param name="destName"></param>
        /// <returns>Indicate whether the operation is succeeded or not.</returns>
        bool ReplaceTrait(string srcName, string destName);

        /// <summary>
        /// Get the target trait.
        /// </summary>
        /// <param name="traitType">Trait type.</param>
        /// <returns>Trait instance.</returns>
        Trait GetTrait(Type traitType);

        /// <summary>
        /// Get the target trait.
        /// </summary>
        /// <typeparam name="TTrait">Trait type.</typeparam>
        /// <returns>Trait instance.</returns>
        TTrait GetTrait<TTrait>() where TTrait : Trait;

        /// <summary>
        /// Get the target trait.
        /// </summary>
        /// <param name="traitName">Trait name.</param>
        /// <returns>Trait instance.</returns>
        Trait GetTrait(string traitName);

        /// <summary>
        /// Check if the container contains the target trait.
        /// </summary>
        /// <param name="traitType">Trait type.</param>
        /// <returns>Indicate whether it contains the trait or not.</returns>
        bool HasTrait(Type traitType);

        /// <summary>
        /// Check if the container contains the target trait.
        /// </summary>
        /// <typeparam name="TTrait">Trait type.</typeparam>
        /// <returns>Indicate whether it contains the trait or not.</returns>
        bool HasTrait<TTrait>() where TTrait : Trait;

        /// <summary>
        /// Check if the container contains the target trait.
        /// </summary>
        /// <param name="traitName">Trait name.</param>
        /// <returns>Indicate whether it contains the trait or not.</returns>
        bool HasTrait(string traitName);

        /// <summary>
        /// Try to get the target trait.
        /// </summary>
        /// <param name="traitType">Trait type.</param>
        /// <param name="trait">Out trait instance.</param>
        /// <returns>Indicate whether it's succeeded or not.</returns>
        bool TryGetTrait(Type traitType, out Trait trait);

        /// <summary>
        /// Try to get the target trait.
        /// </summary>
        /// <typeparam name="TTrait">Trait type.</typeparam>
        /// <param name="trait">Out trait instance.</param>
        /// <returns>Indicate whether it's succeeded or not.</returns>
        bool TryGetTrait<TTrait>(out TTrait trait) where TTrait : Trait;

        /// <summary>
        /// Try to get the target trait by name.
        /// </summary>
        /// <param name="traitName">Trait name.</param>
        /// <param name="trait">Out trait instance.</param>
        /// <returns>Indicate whether it's succeeded or not.</returns>
        bool TryGetTrait(string traitName, out Trait trait);

        /// <summary>
        /// Add tag to the container.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <returns>Indicate whether it's succeeded or not.</returns>
        bool AddTag(string tag);

        /// <summary>
        /// Remove the target tag.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <returns>Indicate whether it's succeeded or not.</returns>
        bool RemoveTag(string tag);

        /// <summary>
        /// Replace the source tag with destined tag.
        /// </summary>
        /// <param name="srcTag">Source tag.</param>
        /// <param name="destTag">Destined tag.</param>
        /// <returns>Indicate whether it's succeeded or not.</returns>
        bool ReplaceTag(string srcTag, string destTag);

        /// <summary>
        /// Check if the container contains the target tag.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <returns>Indicate whether it is exist or not.</returns>
        bool HasTag(string tag);
    }
}
