using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Behaviors.Base;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public class FadeAction : AnimationBase, IAction
    {
        public static readonly BindableProperty FinalOpacityProperty = BindableProperty.Create("FinalOpacity", typeof(double), typeof(FadeAction), 1.0);

        public double FinalOpacity
        {
            get => (double)GetValue(FinalOpacityProperty);
            set => SetValue(FinalOpacityProperty, value);
        }

        public async Task<bool> Execute(object sender, object parameter)
        {
            VisualElement element;
            if (TargetObject != null)
            {
                element = TargetObject as VisualElement;
            }
            else
            {
                element = sender as VisualElement;
            }

            if (element == null)
            {
                return false;
            }

            if (Await)
            {
                await element.FadeTo(FinalOpacity, (uint)Duration, GetEasingFunction());
            }
            else
            {
#pragma warning disable 4014
                element.FadeTo(FinalOpacity, (uint)Duration, GetEasingFunction());
#pragma warning restore 4014
            }

            return true;
        }
    }
}
