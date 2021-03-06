﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public sealed class SetPropertyAction : BindableObject, IAction
    {
        public static readonly BindableProperty PropertyNameProperty = BindableProperty.Create(nameof(PropertyName), typeof(string), typeof(SetPropertyAction), null);
        public static readonly BindableProperty TargetObjectProperty = BindableProperty.Create(nameof(TargetObject), typeof(object), typeof(SetPropertyAction), null);
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(object), typeof(SetPropertyAction), null);

        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        public object TargetObject
        {
            get => (object)GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

        public object Value
        {
            get => (object)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> Execute(object sender, object parameter)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var targetObject = TargetObject ?? sender;

            if (targetObject == null || PropertyName == null)
            {
                return false;
            }

            UpdatePropertyValue(targetObject);
            return true;
        }

        private void UpdatePropertyValue(object targetObject)
        {
            var targetType = targetObject.GetType();
            var propertyInfo = targetType.GetRuntimeProperty(PropertyName);
            ValidateProperty(targetType.Name, propertyInfo);

            Exception innerException = null;
            try
            {
                object result;

                var propertyType = propertyInfo.PropertyType;
                var propertyTypeInfo = propertyType.GetTypeInfo();

                if (Value is null)
                {
                    result = propertyTypeInfo.IsValueType ? Activator.CreateInstance(propertyType) : null;
                }
                else if (propertyTypeInfo.IsAssignableFrom(Value.GetType().GetTypeInfo()))
                {
                    result = Value;
                }
                else
                {
                    var valueAsString = Value.ToString();
                    result = propertyTypeInfo.IsEnum ? Enum.Parse(propertyType, valueAsString, false) : TypeConverterHelper.Convert(valueAsString, propertyType.FullName);
                }
                propertyInfo.SetValue(targetObject, result, new object[0]);
            }
            catch (FormatException ex)
            {
                innerException = ex;
            }
            catch (ArgumentException ex)
            {
                innerException = ex;
            }

            if (innerException != null)
            {
                throw new ArgumentException("Cannot set value.", innerException);
            }
        }

        private static void ValidateProperty(string targetTypeName, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentException("Cannot find property name.");
            }
            else if (!propertyInfo.CanWrite)
            {
                throw new ArgumentException("Property is read-only.");
            }
        }
    }
}
