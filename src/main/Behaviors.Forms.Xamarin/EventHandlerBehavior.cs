using System;
using System.Reflection;
using Behaviors.Base;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public sealed class EventHandlerBehavior : BehaviorPropertiesBase
    {
        private Delegate _eventHandler;
        private object _resolvedSource;

        public static readonly BindableProperty EventNameProperty = BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventHandlerBehavior), propertyChanged: OnEventNameChanged);
        public static readonly BindableProperty SourceObjectProperty = BindableProperty.Create(nameof(SourceObject), typeof(object), typeof(EventHandlerBehavior), null, propertyChanged: OnSourceObjectChanged);

        public string EventName
        {
            get => (string)GetValue(EventNameProperty);
            set => SetValue(EventNameProperty, value);
        }

        public object SourceObject
        {
            get => GetValue(SourceObjectProperty);
            set => SetValue(SourceObjectProperty, value);
        }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
            SetResolvedSource(ComputeResolvedSource());
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            base.OnDetachingFrom(bindable);
            SetResolvedSource(null);
        }

        private void SetResolvedSource(object newSource)
        {
            if (AssociatedObject is null || _resolvedSource == newSource)
            {
                return;
            }

            if (!(_resolvedSource is null))
            {
                DeregisterEvent(EventName);
            }
            _resolvedSource = newSource;

            if (_resolvedSource != null)
            {
                RegisterEvent(EventName);
            }
        }

        private object ComputeResolvedSource()
        {
            return SourceObject ?? AssociatedObject;
        }

        private void RegisterEvent(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return;
            }

            var sourceObjectType = _resolvedSource.GetType();

            var eventInfo = sourceObjectType.GetRuntimeEvent(eventName);

            if (eventInfo is null)
            {
                return;
            }

            var methodInfo = typeof(EventHandlerBehavior).GetTypeInfo().GetDeclaredMethod("OnEvent");

            _eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);

            eventInfo.AddEventHandler(_resolvedSource, _eventHandler);
        }

        private void DeregisterEvent(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                return;
            }

            if (_eventHandler is null)
            {
                return;
            }

            var eventInfo = _resolvedSource.GetType().GetRuntimeEvent(eventName);

            if (eventInfo == null)
            {
                throw new ArgumentException($"EventHandlerBehavior: Can't de-register the '{EventName}' event.", EventName);
            }

            eventInfo.RemoveEventHandler(_resolvedSource, _eventHandler);

            _eventHandler = null;
        }

        private static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is EventHandlerBehavior behavior))
            {
                return;
            }


            if (behavior.AssociatedObject is null || behavior._resolvedSource is null)
            {
                return;
            }

            var oldEventName = (string)oldValue;
            var newEventName = (string)newValue;

            behavior.DeregisterEvent(oldEventName);
            behavior.RegisterEvent(newEventName);
        }

        private static void OnSourceObjectChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EventHandlerBehavior behavior)
            {
                behavior.SetResolvedSource(behavior.ComputeResolvedSource());
            }
        }
    }
}
