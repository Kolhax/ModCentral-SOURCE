using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using MahApps.Metro.Controls;

namespace StrikePackCfg.Windows;

public class News : MetroWindow, IComponentConnector
{
	private static bool willNavigate;

	internal WebBrowser WebBrowser;

	private bool _contentLoaded;

	public News()
	{
		InitializeComponent();
		((FrameworkElement)this).DataContext = this;
	}

	public void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
	{
		if (!e.Uri.ToString().Contains("gp.ctrlmaxplus.ca"))
		{
			e.Cancel = true;
			Process.Start(new ProcessStartInfo
			{
				FileName = e.Uri.ToString()
			});
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/windows/news.xaml", UriKind.Relative);
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
			WebBrowser = (WebBrowser)target;
			WebBrowser.Navigating += WebBrowser_Navigating;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
