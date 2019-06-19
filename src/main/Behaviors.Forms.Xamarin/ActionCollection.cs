using System;
using System.Collections.Specialized;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    [Preserve(AllMembers = true)]
    public class ActionCollection : BindableObjectCollection
    {
        public ActionCollection()
        {
            CollectionChanged += ActionCollection_CollectionChanged;
        }

        private void ActionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collectionChange = e.Action;

            var changed = this[(int)e.NewStartingIndex];

            switch (collectionChange)
            {
                case NotifyCollectionChangedAction.Reset:
                {
                    foreach (var bindable in this)
                    {
                        VerifyType(bindable);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Replace:
                    VerifyType(changed);
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Remove:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void VerifyType(BindableObject bindable)
        {
            if (!(bindable is IAction))
            {
                throw new InvalidOperationException("Non-IAction added to IAction collection");
            }
        }
    }
}
