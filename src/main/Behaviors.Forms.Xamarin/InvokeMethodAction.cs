using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public class InvokeMethodAction : BindableObject, IAction
    {
        Type targetObjectType;
        MethodDescriptor cachedMethodDescriptor;
        List<MethodDescriptor> methodDescriptors = new List<MethodDescriptor>();

        public static readonly BindableProperty MethodNameProperty = BindableProperty.Create("MethodName", typeof(string), typeof(InvokeMethodAction), null, propertyChanged: OnMethodNameChanged);
        public static readonly BindableProperty TargetObjectProperty = BindableProperty.Create("TargetObject", typeof(object), typeof(InvokeMethodAction), null, propertyChanged: OnTargetObjectChanged);

        public string MethodName
        {
            get => (string)GetValue(MethodNameProperty);
            set => SetValue(MethodNameProperty, value);
        }

        public object TargetObject
        {
            get => (object)GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> Execute(object sender, object parameter)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var target = GetValue(TargetObjectProperty) != null ? TargetObject : sender;

            if (target == null || string.IsNullOrWhiteSpace(MethodName))
            {
                return false;
            }

            UpdateTargetType(target.GetType());

            var methodDescriptor = FindBestMethod(parameter);
            if (methodDescriptor == null)
            {
                if (TargetObject != null)
                {
                    throw new ArgumentException("Valid method not found.");
                }
                return false;
            }

            var parameters = methodDescriptor.Parameters;
            switch (parameters.Length)
            {
                case 0:
                    methodDescriptor.MethodInfo.Invoke(target, parameters: null);
                    return true;
                case 2:
                    methodDescriptor.MethodInfo.Invoke(target, new object[] { target, parameter });
                    return true;
            }

            return false;
        }

        private MethodDescriptor FindBestMethod(object parameter)
        {
            var parameterTypeInfo = parameter?.GetType().GetTypeInfo();

            if (parameterTypeInfo == null)
            {
                return cachedMethodDescriptor;
            }

            MethodDescriptor mostDerivedMethod = null;

            foreach (MethodDescriptor currentMethod in methodDescriptors)
            {
                var currentTypeInfo = currentMethod.SecondParameterTypeInfo;

                if (currentTypeInfo.IsAssignableFrom(parameterTypeInfo))
                {
                    if (mostDerivedMethod == null || !currentTypeInfo.IsAssignableFrom(mostDerivedMethod.SecondParameterTypeInfo))
                    {
                        mostDerivedMethod = currentMethod;
                    }
                }
            }

            return mostDerivedMethod ?? cachedMethodDescriptor;
        }

        private void UpdateTargetType(Type newTargetType)
        {
            if (newTargetType == targetObjectType)
            {
                return;
            }

            targetObjectType = newTargetType;
            UpdateMethodDescriptors();
        }

        private void UpdateMethodDescriptors()
        {
            methodDescriptors.Clear();
            cachedMethodDescriptor = null;

            if (string.IsNullOrWhiteSpace(MethodName) || targetObjectType == null)
            {
                return;
            }

            foreach (MethodInfo method in targetObjectType.GetRuntimeMethods())
            {
                if (string.Equals(method.Name, MethodName, StringComparison.Ordinal) && method.ReturnType == typeof(void) && method.IsPublic)
                {
                    var parameters = method.GetParameters();

                    switch (parameters.Length)
                    {
                        case 0:
                            cachedMethodDescriptor = new MethodDescriptor(method, parameters);
                            break;
                        case 2 when parameters[0].ParameterType == typeof(object):
                            methodDescriptors.Add(new MethodDescriptor(method, parameters));
                            break;
                    }
                }
            }

        }

        private static void OnMethodNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var action = (InvokeMethodAction)bindable;
            var newType = newValue?.GetType();
            action.UpdateTargetType(newType);
        }

        private static void OnTargetObjectChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var action = (InvokeMethodAction)bindable;
            action.UpdateMethodDescriptors();
        }
    }
}
