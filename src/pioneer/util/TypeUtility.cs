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
using System.Linq;

namespace Pioneer
{
    static class TypeUtility
    {
        private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        public static Type GetType(string typeName)
        {
            if (!typeCache.TryGetValue(typeName, out Type type))
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                typeCache.Add(typeName, type = assemblies.SelectMany(a => a.GetTypes()).First(t => (t.Name == typeName || t.FullName == typeName)));
            }

            return type;
        }
    }
}
