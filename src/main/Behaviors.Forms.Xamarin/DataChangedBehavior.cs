using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Behaviors.Base;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    public sealed class DataChangedBehavior : BehaviorBase<VisualElement>
    {
        public static readonly BindableProperty ActionsProperty = BindableProperty.Create("Actions", typeof(ActionCollection), typeof(DataChangedBehavior), null);
        public static readonly BindableProperty BindingProperty = BindableProperty.Create("Binding", typeof(object), typeof(DataChangedBehavior), null, propertyChanged: OnValueChanged);
        public static readonly BindableProperty ComparisonProperty = BindableProperty.Create("Comparison", typeof(ComparisonCondition), typeof(DataChangedBehavior), ComparisonCondition.Equal, propertyChanged: OnValueChanged);
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(object), typeof(DataChangedBehavior), null, propertyChanged: OnValueChanged);

        public ActionCollection Actions
        {
            get
            {
                var actionCollection = (ActionCollection)GetValue(ActionsProperty);
                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();
                    SetValue(ActionsProperty, actionCollection);
                }
                return actionCollection;
            }
        }

        public object Binding
        {
            get => (object)GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }

        public ComparisonCondition ComparisonCondition
        {
            get => (ComparisonCondition)GetValue(ComparisonProperty);
            set => SetValue(ComparisonProperty, value);
        }

        public object Value
        {
            get => (object)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            base.OnDetachingFrom(bindable);
        }

        private static bool Compare(object leftOperand, ComparisonCondition operatorType, object rightOperand)
        {
            if (leftOperand != null && rightOperand != null)
            {
                rightOperand = TypeConverterHelper.Convert(rightOperand.ToString(), leftOperand.GetType().FullName);
            }

            var leftComparableOperand = leftOperand as IComparable;
            var rightComparableOperand = rightOperand as IComparable;

            if ((leftComparableOperand != null) && (rightComparableOperand != null))
            {
                return EvaluateComparable(leftComparableOperand, operatorType, rightComparableOperand);
            }

            switch (operatorType)
            {
                case ComparisonCondition.Equal:
                    return object.Equals(leftOperand, rightOperand);
                case ComparisonCondition.NotEqual:
                    return !object.Equals(leftOperand, rightOperand);
                case ComparisonCondition.LessThan:
                case ComparisonCondition.LessThanOrEqual:
                case ComparisonCondition.GreaterThan:
                case ComparisonCondition.GreaterThanOrEqual:
                {
                    switch (leftComparableOperand)
                    {
                        case null when rightComparableOperand == null:
                            throw new ArgumentException("Invalid operands");
                        case null:
                            throw new ArgumentException("Invalid left operand");
                        default:
                            throw new ArgumentException("Invalid right operand");
                    }
                }
                default:
                    return false;
            }
        }

        private static bool EvaluateComparable(IComparable leftOperand, ComparisonCondition operatorType, IComparable rightOperand)
        {
            object convertedOperand = null;
            try
            {
                convertedOperand = Convert.ChangeType(rightOperand, leftOperand.GetType(), CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }

            if (convertedOperand == null)
            {
                return operatorType == ComparisonCondition.NotEqual;
            }

            var comparison = leftOperand.CompareTo((IComparable)convertedOperand);
            switch (operatorType)
            {
                case ComparisonCondition.Equal:
                    return comparison == 0;
                case ComparisonCondition.NotEqual:
                    return comparison != 0;
                case ComparisonCondition.LessThan:
                    return comparison < 0;
                case ComparisonCondition.LessThanOrEqual:
                    return comparison <= 0;
                case ComparisonCondition.GreaterThan:
                    return comparison > 0;
                case ComparisonCondition.GreaterThanOrEqual:
                    return comparison >= 0;
                default:
                    return false;
            }
        }

        private static async void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (DataChangedBehavior)bindable;
            if (behavior.AssociatedObject == null)
            {
                return;
            }

            if (Compare(behavior.Binding, behavior.ComparisonCondition, behavior.Value))
            {
                foreach (BindableObject item in behavior.Actions)
                {
                    item.BindingContext = behavior.BindingContext;
                    IAction action = (IAction)item;
                    await action.Execute(bindable, newValue);
                }
            }
        }
    }
}
