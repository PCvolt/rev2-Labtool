using System;
using System.Configuration;
using System.Threading;
using System.Windows;


namespace rev2_LabTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly string _githubPage = ConfigurationManager.AppSettings.Get("GithubPage");
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

		bool removeHint = false;
		private void refreshInfo()
		{
			while (true)
			{
				Thread.Sleep(15);

				if (labtool.ReadMenuLabel().Equals("PSM_TRP_GuardTypeT")) //Workaround, it is best to find the actual mode of the game rather than relying on selecting a label
				{
					labtool.updateFrameInfo();

					if (removeHint)
					{
						Dispatcher.BeginInvoke(new Action(() =>
						{
							this.faLabel.Content = "";
						}));
						removeHint = false;
					}
					
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
					//TODO: Johnny stance (?)
				}
				else
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						this.faLabel.Content = "Select 'Block Type'";
					}));
					removeHint = true;

				}
			}
		}

		private void clearGapsString(object sender, RoutedEventArgs e)
		{
			this.gapsTextblock.Text = "";
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("explorer.exe", _githubPage);
		}
	}
}
