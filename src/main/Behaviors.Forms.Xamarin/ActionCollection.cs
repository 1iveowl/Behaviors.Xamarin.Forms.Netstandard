using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Behaviors.Interface;
using Xamarin.Forms;

namespace Behaviors
{
    public class ActionCollection : BindableObjectCollection
    {
        public ActionCollection()
        {
            CollectionChanged += ActionCollection_CollectionChanged;
        }

        void ActionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collectionChange = e.Action;

            if (collectionChange == NotifyCollectionChangedAction.Reset)
            {
                foreach (BindableObject bindable in this)
                {
                    ActionCollection.VerifyType(bindable);
                }
            }
            else if (collectionChange == NotifyCollectionChangedAction.Replace)
            {
                BindableObject changed = this[(int)e.NewStartingIndex];
                ActionCollection.VerifyType(changed);
            }
        }

        static void VerifyType(BindableObject bindable)
        {
            if (!(bindable is IAction))
            {
                throw new InvalidOperationException("Non-IAction added to IAction collection");
            }
        }
    }
}
