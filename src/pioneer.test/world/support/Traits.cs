/*
 * This file is part of the Pioneer package.
 *
 * (c) MuGuangYi <muguangyi@hotmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */

namespace Pioneer.Test.World.Support
{
    public class BaseTrait : Trait
    {
        public override void Dispose()
        {
            --TraitCount;
        }

        public override void OnInit()
        {
            ++TraitCount;
        }

        public void ChangeValue()
        {
            NotifyChanged();
        }

        public static int TraitCount = 0;
    }

    public class ATrait : BaseTrait
    {
    }

    public class BTrait : BaseTrait
    {
    }

    public class CTrait : BaseTrait
    {
    }

    public class DTrait : BaseTrait
    {
    }
}
