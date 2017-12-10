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
    public class ScaleAction : AnimationBase, IAction
    {
        public static readonly BindableProperty FinalScaleProperty = BindableProperty.Create("FinalScale", typeof(double), typeof(ScaleAction), 1.0);
        public static readonly BindableProperty IsRelativeProperty = BindableProperty.Create("IsRelative", typeof(bool), typeof(ScaleAction), false);

        public double FinalScale
        {
            get => (double)GetValue(FinalScaleProperty);
            set => SetValue(FinalScaleProperty, value);
        }

        public bool IsRelative
        {
            get => (bool)GetValue(IsRelativeProperty);
            set => SetValue(IsRelativeProperty, value);
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

            if (IsRelative)
            {
                if (Await)
                {
                    await element.RelScaleTo(FinalScale, (uint)Duration, GetEasingFunction());
                }
                else
                {
#pragma warning disable 4014
                    element.RelScaleTo(FinalScale, (uint)Duration, GetEasingFunction());
#pragma warning restore 4014
                }
            }
            else
            {
                if (Await)
                {
                    await element.ScaleTo(FinalScale, (uint)Duration, GetEasingFunction());
                }
                else
                {
#pragma warning disable 4014
                    element.ScaleTo(FinalScale, (uint)Duration, GetEasingFunction());
#pragma warning restore 4014
                }
            }

            return true;
        }
    }
}
