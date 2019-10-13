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
                {
                    var changed = this[e.NewStartingIndex];
                   VerifyType(changed);
                    break;
                }
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
