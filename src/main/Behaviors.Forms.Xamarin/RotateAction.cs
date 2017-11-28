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
    public class RotateAction : AnimationBase, IAction
    {
        public static readonly BindableProperty FinalAngleProperty = BindableProperty.Create("FinalAngle", typeof(double), typeof(RotateAction), 0.0);
        public static readonly BindableProperty IsRelativeProperty = BindableProperty.Create("IsRelative", typeof(bool), typeof(RotateAction), false);
        public static readonly BindableProperty AxisProperty = BindableProperty.Create("Axis", typeof(RotationAxis), typeof(RotateAction), RotationAxis.Z);

        public double FinalAngle
        {
            get => (double)GetValue(FinalAngleProperty);
            set => SetValue(FinalAngleProperty, value);
        }

        public bool IsRelative
        {
            get => (bool)GetValue(IsRelativeProperty);
            set => SetValue(IsRelativeProperty, value);
        }

        public RotationAxis Axis
        {
            get => (RotationAxis)GetValue(AxisProperty);
            set => SetValue(AxisProperty, value);
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

            switch (Axis)
            {
                case RotationAxis.X:
                    if (Await)
                    {
                        await element.RotateXTo(FinalAngle, (uint)Duration, GetEasingFunction());
                    }
                    else
                    {
#pragma warning disable 4014
                        element.RotateXTo(FinalAngle, (uint)Duration, GetEasingFunction());
#pragma warning restore 4014
                    }
                    break;
                case RotationAxis.Y:
                    if (Await)
                    {
                        await element.RotateYTo(FinalAngle, (uint)Duration, GetEasingFunction());
                    }
                    else
                    {
#pragma warning disable 4014
                        element.RotateYTo(FinalAngle, (uint)Duration, GetEasingFunction());
#pragma warning restore 4014
                    }
                    break;
                case RotationAxis.Z:
                    if (IsRelative)
                    {
                        if (Await)
                        {
                            await element.RelRotateTo(FinalAngle, (uint)Duration, GetEasingFunction());
                        }
                        else
                        {
#pragma warning disable 4014
                            element.RelRotateTo(FinalAngle, (uint)Duration, GetEasingFunction());
#pragma warning restore 4014
                        }
                    }
                    else
                    {
                        if (Await)
                        {
                            await element.RotateTo(FinalAngle, (uint)Duration, GetEasingFunction());
                        }
                        else
                        {
#pragma warning disable 4014
                            element.RotateTo(FinalAngle, (uint)Duration, GetEasingFunction());
#pragma warning restore 4014
                        }
                    }
                    break;
                default:
                    break;
            }

            return true;
        }
    }
}
