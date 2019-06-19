using System;
using System.Threading.Tasks;
using Behaviors.Interface;
using Xamarin.Forms;

#pragma warning disable 1998

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public sealed class GoToStateAction : BindableObject, IAction
    {
        public static readonly BindableProperty StateNameProperty = BindableProperty.Create(nameof(StateNameProperty), typeof(string), typeof(GoToStateAction), null);
        public static readonly BindableProperty TargetObjectProperty = BindableProperty.Create(nameof(TargetObject), typeof(VisualElement), typeof(GoToStateAction), null);

        public string StateName
        {
            get => (string)GetValue(StateNameProperty);
            set => SetValue(StateNameProperty, value);
        }

        public object TargetObject
        {
            get => (VisualElement)GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

        public async Task<bool> Execute(object sender, object parameter)
        {
            if (string.IsNullOrWhiteSpace(StateName))
            {
                return false;
            }

            if (TargetObject is VisualElement element)
            {
                return GoToState(element, StateName);
            }

            return false;
        }

        private bool GoToState(VisualElement visualElement, string stateName)
        {
            if (visualElement is null)
            {
                throw new ArgumentNullException(nameof(visualElement));
            }


            if (string.IsNullOrWhiteSpace(stateName))
            {
                throw new ArgumentNullException(nameof(stateName));
            }

            return VisualStateManager.GoToState(visualElement, stateName);
        }
    }
}
