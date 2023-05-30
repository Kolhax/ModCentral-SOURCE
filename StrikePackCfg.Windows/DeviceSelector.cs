using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using StrikePackCfg.Backend;

namespace StrikePackCfg.Windows;

public class DeviceSelector : MetroWindow, IComponentConnector
{
	private readonly MainWindow _mainWin;

	private bool _contentLoaded;

	public ReadOnlyObservableCollection<IDeviceState> Devices => new ReadOnlyObservableCollection<IDeviceState>(_mainWin.Devices);

	public IDeviceState SelectedDevice { get; set; }

	public DeviceSelector(MainWindow mainwin)
	{
		((Window)this).Owner = (Window)(object)mainwin;
		InitializeComponent();
		((FrameworkElement)this).DataContext = this;
		_mainWin = mainwin;
	}

	private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		App.CurrentDevice = SelectedDevice;
		_mainWin.UpdateCurrentDevice();
		((Window)this).Close();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/windows/deviceselector.xaml", UriKind.Relative);
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
			((ListView)target).SelectionChanged += Selector_OnSelectionChanged;
		}
		else
		{
			_contentLoaded = true;
		}
	}
}
