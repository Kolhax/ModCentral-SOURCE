using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using MahApps.Metro.Controls;

namespace StrikePackCfg.Windows;

public class About : MetroWindow, IComponentConnector
{
	private bool _contentLoaded;

	public string AppNameVersion => $"Mod Central v{App.GetSemver()}";

	public About()
	{
		InitializeComponent();
		((FrameworkElement)this).DataContext = this;
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
		e.Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/windows/about.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			((Hyperlink)target).RequestNavigate += Hyperlink_RequestNavigate;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
