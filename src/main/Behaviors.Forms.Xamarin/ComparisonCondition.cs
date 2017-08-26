using System;
using System.Collections.Generic;
using System.Text;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public enum ComparisonCondition
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }
}
