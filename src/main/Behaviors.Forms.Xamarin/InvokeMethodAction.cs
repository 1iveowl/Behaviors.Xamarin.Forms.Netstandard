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
        private Type _targetObjectType;
        private MethodDescriptor _cachedMethodDescriptor;
        private readonly List<MethodDescriptor> _methodDescriptors = new List<MethodDescriptor>();

        public static readonly BindableProperty MethodNameProperty = BindableProperty.Create(nameof(MethodName), typeof(string), typeof(InvokeMethodAction), null, propertyChanged: OnMethodNameChanged);
        public static readonly BindableProperty TargetObjectProperty = BindableProperty.Create(nameof(TargetObject), typeof(object), typeof(InvokeMethodAction), null, propertyChanged: OnTargetObjectChanged);

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


        public async Task<bool> Execute(object sender, object parameter)
        {
            var target = GetValue(TargetObjectProperty) != null ? TargetObject : sender;

            if (target is null || string.IsNullOrWhiteSpace(MethodName))
            {
                return false;
            }

            UpdateTargetType(target.GetType());

            var methodDescriptor = FindBestMethod(parameter);

            if (methodDescriptor is null)
            {
                if (TargetObject != null)
                {
                    throw new ArgumentException("Valid method not found.");
                }
                return false;
            }

            var parameters = methodDescriptor.Parameters;

            if (parameters.Length == 0)
            {
                methodDescriptor.MethodInfo.Invoke(target, parameters: null);
                return true;
            }
            else if (parameters.Length == 2)
            {
                methodDescriptor.MethodInfo.Invoke(target, new object[] {target, parameter});
                return true;
            }

            await Task.CompletedTask;

            return false;
        }

        private MethodDescriptor FindBestMethod(object parameter)
        {
            var parameterTypeInfo = parameter?.GetType().GetTypeInfo();

            if (parameterTypeInfo is null)
            {
                return _cachedMethodDescriptor;
            }

            MethodDescriptor mostDerivedMethod = null;

            foreach (var currentMethod in _methodDescriptors)
            {
                var currentTypeInfo = currentMethod.SecondParameterTypeInfo;

                if (currentTypeInfo.IsAssignableFrom(parameterTypeInfo))
                {
                    if (mostDerivedMethod is null || !currentTypeInfo.IsAssignableFrom(mostDerivedMethod.SecondParameterTypeInfo))
                    {
                        mostDerivedMethod = currentMethod;
                    }
                }
            }

            return mostDerivedMethod ?? _cachedMethodDescriptor;
        }

        private void UpdateTargetType(Type newTargetType)
        {
            if (newTargetType == _targetObjectType)
            {
                return;
            }

            _targetObjectType = newTargetType;

            UpdateMethodDescriptors();
        }

        private void UpdateMethodDescriptors()
        {
            _methodDescriptors.Clear();

            _cachedMethodDescriptor = null;

            if (string.IsNullOrWhiteSpace(MethodName) || _targetObjectType is null)
            {
                return;
            }

            foreach (var method in _targetObjectType.GetRuntimeMethods())
            {
                if (string.Equals(method.Name, MethodName, StringComparison.Ordinal) 
                    && method.ReturnType == typeof(void)
                    && method.IsPublic)
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length == 0)
                    {
                        _cachedMethodDescriptor = new MethodDescriptor(method, parameters);
                    }
                        
                    else if (parameters.Length == 2 && parameters[0].ParameterType == typeof(object))
                    {
                        _methodDescriptors.Add(new MethodDescriptor(method, parameters));
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
