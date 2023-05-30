using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using StrikePackCfg.Backend;
using StrikePackCfg.Views;

namespace StrikePackCfg.Windows;

public class RemapperWindow : MetroWindow, IComponentConnector
{
	internal Flyout RightFlyout;

	internal TextBlock PR_txt;

	internal ComboBox PaddleRight;

	internal TextBlock PR2_txt;

	internal ComboBox PaddleRight2;

	internal StackPanel RightAdjustmentSettings;

	internal Flyout LeftFlyout;

	internal TextBlock PL_txt;

	internal ComboBox PaddleLeft;

	internal TextBlock PL2_txt;

	internal ComboBox PaddleLeft2;

	internal StackPanel LeftAdjustmentSettings;

	internal DockPanel Panel;

	private bool _contentLoaded;

	public RemapperWindow(RemapperViewBase remapperView, string title)
	{
		((Window)this).Owner = (Window)(object)App.MainWin;
		InitializeComponent();
		MainWindow mainWin = App.MainWin;
		mainWin.CloseWindow = (EventHandler)Delegate.Combine(mainWin.CloseWindow, new EventHandler(CloseWindow));
		((Window)this).Closing += delegate
		{
			MainWindow mainWin2 = App.MainWin;
			mainWin2.CloseWindow = (EventHandler)Delegate.Remove(mainWin2.CloseWindow, new EventHandler(CloseWindow));
		};
		Panel.Children.Clear();
		Panel.Children.Add(remapperView);
		((Window)this).Title = title;
		remapperView.Window = this;
		((FrameworkElement)(object)LeftFlyout).DataContext = remapperView;
		((FrameworkElement)(object)RightFlyout).DataContext = remapperView;
		if (!App.CurrentDevice.Device.Product.ProductHasPaddles())
		{
			PaddleLeft2.Visibility = Visibility.Collapsed;
			PaddleRight2.Visibility = Visibility.Collapsed;
			PaddleLeft.Visibility = Visibility.Collapsed;
			PaddleRight.Visibility = Visibility.Collapsed;
			PL2_txt.Visibility = Visibility.Collapsed;
			PR2_txt.Visibility = Visibility.Collapsed;
			PL_txt.Visibility = Visibility.Collapsed;
			PR_txt.Visibility = Visibility.Collapsed;
		}
		else if (!App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			PaddleLeft2.IsEnabled = false;
			PaddleRight2.IsEnabled = false;
			PL_txt.Text = "Paddle Left";
			PL2_txt.Text = "N.A.";
			PR_txt.Text = "Paddle Right";
			PR2_txt.Text = "N.A.";
			PaddleLeft2.Visibility = Visibility.Collapsed;
			PaddleRight2.Visibility = Visibility.Collapsed;
			PL2_txt.Visibility = Visibility.Collapsed;
			PR2_txt.Visibility = Visibility.Collapsed;
		}
		else
		{
			PaddleLeft2.IsEnabled = true;
			PaddleRight2.IsEnabled = true;
			PL_txt.Text = "Button #1";
			PL2_txt.Text = "Button #3";
			PR_txt.Text = "Button #2";
			PR2_txt.Text = "Button #4";
			PaddleLeft2.Visibility = Visibility.Visible;
			PaddleRight2.Visibility = Visibility.Visible;
			PL2_txt.Visibility = Visibility.Visible;
			PR2_txt.Visibility = Visibility.Visible;
		}
		LeftAdjustmentSettings.IsEnabled = App.CurrentDevice.Device.SupportsAdjustments;
		RightAdjustmentSettings.IsEnabled = App.CurrentDevice.Device.SupportsAdjustments;
		if (App.CurrentDevice.Device.SupportsAdjustments)
		{
			LeftAdjustmentSettings.Visibility = Visibility.Visible;
			RightAdjustmentSettings.Visibility = Visibility.Visible;
		}
		else
		{
			LeftAdjustmentSettings.Visibility = Visibility.Collapsed;
			RightAdjustmentSettings.Visibility = Visibility.Collapsed;
		}
	}

	private void CloseWindow(object sender, EventArgs eventArgs)
	{
		((Window)this).Close();
	}

	internal void ShowLeftFlyout()
	{
		LeftFlyout.IsOpen = true;
	}

	internal void ShowRightFlyout()
	{
		RightFlyout.IsOpen = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/windows/remapperwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			RightFlyout = (Flyout)target;
			break;
		case 2:
			PR_txt = (TextBlock)target;
			break;
		case 3:
			PaddleRight = (ComboBox)target;
			break;
		case 4:
			PR2_txt = (TextBlock)target;
			break;
		case 5:
			PaddleRight2 = (ComboBox)target;
			break;
		case 6:
			RightAdjustmentSettings = (StackPanel)target;
			break;
		case 7:
			LeftFlyout = (Flyout)target;
			break;
		case 8:
			PL_txt = (TextBlock)target;
			break;
		case 9:
			PaddleLeft = (ComboBox)target;
			break;
		case 10:
			PL2_txt = (TextBlock)target;
			break;
		case 11:
			PaddleLeft2 = (ComboBox)target;
			break;
		case 12:
			LeftAdjustmentSettings = (StackPanel)target;
			break;
		case 13:
			Panel = (DockPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
