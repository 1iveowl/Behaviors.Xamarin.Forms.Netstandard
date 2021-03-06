﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Behaviors.Base;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public class TranslateAction : AnimationBase, IAction
    {
        public static readonly BindableProperty XProperty = BindableProperty.Create(nameof(X), typeof(double), typeof(TranslateAction), 1.0);
        public static readonly BindableProperty YProperty = BindableProperty.Create(nameof(Y), typeof(double), typeof(TranslateAction), 1.0);

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public async Task<bool> Execute(object sender, object parameter)
        {
            if (!(TargetObject is VisualElement element))
            {
                element = sender as VisualElement;

                if (element is null)
                {
                    return false;
                }
            }

            if (Await)
            {
                await element.TranslateTo(X, Y, (uint)Duration, GetEasingFunction());
            }
            else
            {
#pragma warning disable 4014
                element.TranslateTo(X, Y, (uint)Duration, GetEasingFunction());
#pragma warning restore 4014
            }

            return true;
        }
    }
}
