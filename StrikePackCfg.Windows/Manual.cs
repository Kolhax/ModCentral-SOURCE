using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MahApps.Metro.Controls;

namespace StrikePackCfg.Windows;

public class Manual : MetroWindow, IComponentConnector
{
	internal WebBrowser ManualUrl;

	private bool _contentLoaded;

	public Manual()
	{
		InitializeComponent();
		((FrameworkElement)this).DataContext = this;
		if (App.CurrentDevice != null)
		{
			ManualUrl.Source = (App.CurrentDevice.Device.Product.DisplayName.Contains("PS4") ? new Uri("https://collectiveminds.ca/wp-content/uploads/cm_strikepack_fps_ps4_manual_eng_v3_1804.pdf") : new Uri("https://collectiveminds.ca/wp-content/uploads/cm_strikepack_fps_xb1_manual_eng_v2_1804.pdf"));
		}
		else
		{
			MessageBox.Show("Please connect a Strikepack.", "No device found");
		}
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/windows/manual.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			ManualUrl = (WebBrowser)target;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
