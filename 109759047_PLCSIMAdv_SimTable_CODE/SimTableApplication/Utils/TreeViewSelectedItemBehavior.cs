using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SimTableApplication.Utils
{
    public class TreeViewSelectedItemBehavior : Behavior<TreeView>
    {
        //dependency property
        public static readonly DependencyProperty SelectedItemProperty = 
            DependencyProperty.Register("SelectedItem", typeof(object), 
                typeof(TreeViewSelectedItemBehavior), 
                new FrameworkPropertyMetadata(null) { BindsTwoWayByDefault = true });

        //property wrapper
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }
    }
}
