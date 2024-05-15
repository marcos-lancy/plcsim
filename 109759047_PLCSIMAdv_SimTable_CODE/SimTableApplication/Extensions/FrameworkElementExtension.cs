using System.Windows;

namespace SimTableApplication.Extensions
{
    public static class FrameworkElementExtension
    {
        public static void UpdateBindingTarget(this FrameworkElement element, DependencyProperty property)
        {
            var bindingExpr = element.GetBindingExpression(property);
            bindingExpr?.UpdateTarget();
        }

        public static void UpdateBindingSource(this FrameworkElement element, DependencyProperty property)
        {
            var bindingExpr = element.GetBindingExpression(property);
            bindingExpr?.UpdateSource();
        }
    }
}
