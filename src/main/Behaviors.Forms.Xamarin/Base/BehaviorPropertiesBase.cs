using Xamarin.Forms;

namespace Behaviors.Base
{
    [Preserve(AllMembers = true)]
    [ContentProperty("Actions")]
    public class BehaviorPropertiesBase : BehaviorBase<VisualElement>
    {
        public static readonly BindableProperty ActionsProperty = BindableProperty.Create(nameof(Actions), typeof(ActionCollection), typeof(BehaviorPropertiesBase), null);

        public ActionCollection Actions => (ActionCollection)GetValue(ActionsProperty);

        public BehaviorPropertiesBase()
        {
            SetValue(ActionsProperty, new ActionCollection());
        }
    }
}
