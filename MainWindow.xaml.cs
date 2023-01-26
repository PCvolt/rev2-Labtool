using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

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
		private Labtool _labtool;

		#region FormHandling
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_labtool = new Labtool();

			try
			{
				MemoryAccessor.AttachToProcess();
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
			checkbox.IsChecked = false;
			SetForegroundWindow(MemoryAccessor.process.MainWindowHandle);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Environment.Exit(Environment.ExitCode);
			MemoryAccessor.Dispose();
		}
		#endregion

		#region FormFunctions
		private void updateLabel(System.Windows.Controls.ContentControl updatedLabel, string updatedString)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				updatedLabel.Content = updatedString;
			}));
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
		#endregion

		private void refreshInfo()
		{
			while (true)
			{
				_labtool.macroButtons();
				_labtool.updateFrameInfo();


				if (_labtool.f.updateFA)
				{
					updateLabel(this.fa1Label, _labtool.f.frameAdvantage + "F");
					_labtool.f.updateFA = false;
				}

				if (_labtool.g1.updateGap)
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						string concat = this.gaps1Textblock.Text;
						this.gaps1Textblock.Text = concat.Insert(0, _labtool.g1.rememberGap.ToString() + "F" + Environment.NewLine);
					}));
					_labtool.g1.updateGap = false;
				}

				if (_labtool.g2.updateGap)
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						string concat = this.gaps2Textblock.Text;
						this.gaps2Textblock.Text = concat.Insert(0, _labtool.g2.rememberGap.ToString() + "F" + Environment.NewLine);
					}));
					_labtool.g2.updateGap = false;
				}

				updateLabel(this.p1HPLabel, "" + _labtool._player1._HP);
				updateLabel(this.p2HPLabel, "" + _labtool._player2._HP);
				updateLabel(this.p1MeterLabel, "" + _labtool._player1._meter / 100.0);
				updateLabel(this.p2MeterLabel, "" + _labtool._player2._meter / 100.0);
				updateLabel(this.p1RISCLabel, "" + _labtool._player1._RISC / 100.0);
				updateLabel(this.p2RISCLabel, "" + _labtool._player2._RISC / 100.0);
				updateLabel(this.p1xPosLabel, "x: " + _labtool._player1._pos.x / 1000);
				updateLabel(this.p1yPosLabel, "y: " + _labtool._player1._pos.y / 1000);
				updateLabel(this.p2xPosLabel, "x: " + _labtool._player2._pos.x / 1000);
				updateLabel(this.p2yPosLabel, "y: " + _labtool._player2._pos.y / 1000);

				// Player is always #1, dummy is always #2, how to find which character is at which side ?
				updateLabel(this.p1CharLabel, "(Player)" + Player.charactersList[_labtool._player1.characterIndex]);
				updateLabel(this.p2CharLabel, Player.charactersList[_labtool._player2.characterIndex]);
				updateLabel(this.p1DefModifLabel, "[x" + Player.defmodifList[_labtool._player1.characterIndex] + "]");
				updateLabel(this.p2DefModifLabel, "[x" + Player.defmodifList[_labtool._player2.characterIndex] + "]");
				updateLabel(this.p1StunLabel, "" + _labtool._player1._stun + "/" + Player.stunList[_labtool._player1.characterIndex]);
				updateLabel(this.p2StunLabel, "" + _labtool._player2._stun + "/" + Player.stunList[_labtool._player2.characterIndex]);
				updateLabel(this.p1GutsLabel, "(x guts)");
				updateLabel(this.p2GutsLabel, "(x guts)");
			}
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			Labtool.processKeys = !Labtool.processKeys;
		}
	}
}


