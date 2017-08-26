using System;
using System.Collections.Generic;
using System.Text;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public enum EasingFunction
    {
        BounceIn,
        BounceOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        Linear,
        SinIn,
        SinOut,
        SinInOut,
        SpringIn,
        SpringOut
    }
}
