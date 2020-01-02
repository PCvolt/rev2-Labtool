using System;
using System.Threading;
using System.Windows;


namespace rev2_LabTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Labtool labtool;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			labtool = new Labtool();

			try
			{
				labtool.AttachToProcess();
			}
			catch (Exception exception)
			{
				MessageBox.Show($"{exception.Message}");
				Application.Current.Shutdown();
				return;
			}

			Thread t1 = new Thread(refreshInfo);
			t1.IsBackground = true;
			t1.Start();

		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Environment.Exit(Environment.ExitCode);
			labtool?.Dispose();
		}

		private void refreshInfo()
		{
			while (true)
			{
				labtool.frameAdvantageLoop();//System.ArgumentException

				if (labtool.updateFA)
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						this.faLabel.Content = labtool.frameAdvantage + "F";
					}));
				}

				if (labtool.updateGap)
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						string concat = this.gapsTextblock.Text;
						this.gapsTextblock.Text = concat.Insert(0, labtool.rememberGap.ToString() + "F" + Environment.NewLine);
					}));
					labtool.updateGap = false;
				}
				//TODO: Johnny stance
			}
		}

		private void clearGapsString(object sender, RoutedEventArgs e)
		{
			this.gapsTextblock.Text = "";
		}
	}
}
