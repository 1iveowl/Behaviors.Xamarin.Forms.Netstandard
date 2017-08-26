﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Behaviors.Base;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    public class TranslateAction : AnimationBase, IAction
    {
        public static readonly BindableProperty XProperty = BindableProperty.Create("X", typeof(double), typeof(TranslateAction), 1.0);
        public static readonly BindableProperty YProperty = BindableProperty.Create("Y", typeof(double), typeof(TranslateAction), 1.0);

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
                await element.TranslateTo(X, Y, (uint)Duration, GetEasingFunction());
            }
            else
            {
                element.TranslateTo(X, Y, (uint)Duration, GetEasingFunction());
            }

            return true;
        }
    }
}
