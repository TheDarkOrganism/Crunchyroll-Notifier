using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using WinUIApp.Controls;

namespace WinUIApp.Validation
{
	internal static class VisualTreeExtensions
	{
		public static TElement? FindFirstVisualChild<TElement>(this DependencyObject parentDependency, string name)
			where TElement : notnull, FrameworkElement
		{
			ArgumentNullException.ThrowIfNull(parentDependency, nameof(parentDependency));
			ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentDependency); i++)
			{
				DependencyObject childDependency = VisualTreeHelper.GetChild(parentDependency, i);

				if (childDependency is null)
				{
					continue;
				}

				if (childDependency is TElement element && element.Name == name)
				{
					return element;
				}

				if (childDependency.FindFirstVisualChild<TElement>(name) is TElement result)
				{
					return result;
				}
			}

			return null;
		}

		private static string? GetBindingPath(Control control, DependencyProperty dependencyProperty)
		{
			ArgumentNullException.ThrowIfNull(control, nameof(control));
			ArgumentNullException.ThrowIfNull(dependencyProperty, nameof(dependencyProperty));

			return control.GetBindingExpression(dependencyProperty)?.ParentBinding.Path.Path;
		}

		public static string? GetBindingPath(this Control control)
		{
			ArgumentNullException.ThrowIfNull(control, nameof(control));

			return control switch
			{
				TextBox => GetBindingPath(control, TextBox.TextProperty),
				NumberBox => GetBindingPath(control, NumberBox.ValueProperty) ?? GetBindingPath(control, NumberBox.TextProperty),
				Selector => GetBindingPath(control, Selector.SelectedValueProperty) ?? GetBindingPath(control, Selector.SelectedItemProperty),
				CustomTimePicker => GetBindingPath(control, CustomTimePicker.TimeProperty),
				_ => null
			};
		}
	}
}
