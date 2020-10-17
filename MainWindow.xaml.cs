using System;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Runtime.InteropServices;



namespace rev2_Labtool_Framework_
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 
	public struct Rect
	{
		public int Left { get; set; }
		public int Top { get; set; }
		public int Right { get; set; }
		public int Bottom { get; set; }
	}

	public partial class MainWindow : Window
	{
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);


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
			SetForegroundWindow(labtool.process.MainWindowHandle);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Environment.Exit(Environment.ExitCode);
			labtool?.Dispose();
		}


		private void updateLabel(System.Windows.Controls.ContentControl updatedLabel, string updatedString)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				updatedLabel.Content = updatedString;
			}));
		}

		private void refreshInfo()
		{
			while (true)
			{
				labtool.macroButtons();
				labtool.updateFrameInfo();


				if (labtool.f1.updateFA)
				{
					updateLabel(this.fa1Label, labtool.f1.frameAdvantage + "F");
					labtool.f1.updateFA = false;
				}

				if (labtool.f1.updateGap)
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						string concat = this.gaps1Textblock.Text;
						this.gaps1Textblock.Text = concat.Insert(0, labtool.f1.rememberGap.ToString() + "F" + Environment.NewLine);
					}));
					labtool.f1.updateGap = false;
				}

				if (labtool.f2.updateGap)
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						string concat = this.gaps2Textblock.Text;
						this.gaps2Textblock.Text = concat.Insert(0, labtool.f2.rememberGap.ToString() + "F" + Environment.NewLine);
					}));
					labtool.f2.updateGap = false;
				}
			
				updateLabel(this.p1HPLabel, "" + labtool.player1.HP);
				updateLabel(this.p2HPLabel, "" + labtool.player2.HP);
				updateLabel(this.p1MeterLabel, "" + labtool.player1.meter / 100.0);
				updateLabel(this.p2MeterLabel, "" + labtool.player2.meter / 100.0);
				updateLabel(this.p1RISCLabel, "" + labtool.player1.RISC / 100.0);
				updateLabel(this.p2RISCLabel, "" + labtool.player2.RISC / 100.0);
				updateLabel(this.p1xPosLabel, "x: " + labtool.player1.pos.x / 1000);
				updateLabel(this.p1yPosLabel, "y: " + labtool.player1.pos.y / 1000);
				updateLabel(this.p2xPosLabel, "x: " + labtool.player2.pos.x / 1000);
				updateLabel(this.p2yPosLabel, "y: " + labtool.player2.pos.y / 1000);

				// Player is always #1, dummy is always #2, how to find which character is at which side ?
				updateLabel(this.p1CharLabel, "(Player)" + Player.charactersList[labtool.player1.characterIndex]);
				updateLabel(this.p2CharLabel, Player.charactersList[labtool.player2.characterIndex]);
				updateLabel(this.p1DefModifLabel, "[x" + Player.defmodifList[labtool.player1.characterIndex] + "]");
				updateLabel(this.p2DefModifLabel, "[x" + Player.defmodifList[labtool.player2.characterIndex] + "]");
				updateLabel(this.p1StunLabel, "" + labtool.player1.stun + "/" + Player.stunList[labtool.player1.characterIndex]);
				updateLabel(this.p2StunLabel, "" + labtool.player2.stun + "/" + Player.stunList[labtool.player2.characterIndex]);
				updateLabel(this.p1GutsLabel, "(x guts)");
				updateLabel(this.p2GutsLabel, "(x guts)");
			}
		}

		private void clearGapsString(object sender, RoutedEventArgs e)
		{
			this.gaps1Textblock.Text = "";
			this.gaps2Textblock.Text = "";
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("explorer.exe", _githubPage);
		}
	}
}


