using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    internal sealed class MethodDescriptor
    {
        public MethodInfo MethodInfo { get; }

        public ParameterInfo[] Parameters { get; }

        public int ParameterCount => Parameters.Length;

        public TypeInfo SecondParameterTypeInfo => ParameterCount < 2 ? null : Parameters[1].ParameterType.GetTypeInfo();

        public MethodDescriptor(MethodInfo methodInfo, ParameterInfo[] parameters)
        {
            MethodInfo = methodInfo;
            Parameters = parameters;
        }
    }
}
