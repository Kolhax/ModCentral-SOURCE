using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MahApps.Metro.Controls.Dialogs;

namespace StrikePackCfg.Views;

public class CustomProgressDialog : BaseMetroDialog, IComponentConnector
{
	internal TextBlock TextBlock;

	private bool _contentLoaded;

	public CustomProgressDialog(string msg)
	{
		InitializeComponent();
		TextBlock.Text = msg;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/views/customprogressdialog.xaml", UriKind.Relative);
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
			TextBlock = (TextBlock)target;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
