namespace Monotone
{
	using System;
	using System.IO;
	using System.Net;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Markup;
	using System.Windows.Media;
	using System.Windows.Resources;
	using System.Windows.Threading;

	public class Application : System.Windows.Application
	{
		private DispatcherTimer timer = new DispatcherTimer();
		private WebClient webClient;
		private int currentPart = 0;

		public Application()
		{
			this.Startup += this.Application_Startup;
		}

		private void Application_Startup(object sender, StartupEventArgs args)
		{
			this.RootVisual = new Canvas();
			this.Root.Loaded += this.RootVisual_Loaded;
		}

		private Canvas Root
		{
			get { return (Canvas) this.RootVisual; }
		}

		private void RootVisual_Loaded(object sender, EventArgs e)
		{
			Stream audioStream = this.GetType().Assembly.GetManifestResourceStream("Monotone.Monotone.mp3");

			this.Root.Children.Clear();

			MediaElement audio = new MediaElement();
			audio.SetSource(audioStream);
			this.Root.Children.Add(audio);
			audio.Pause();

			this.Root.Children.Add(new Canvas());
			this.NextPart();

			audio.Position = new TimeSpan(0);
			audio.Play();

			this.timer.Interval = new TimeSpan(0, 0, 0, 0, (int)((60f / 140f) * 4 * 4 * 2 * 1000f));
			this.timer.Tick += new EventHandler(this.Part_Tick);
			this.timer.Start();
		}

		private void Part_Tick(object sender, EventArgs e)
		{
			this.timer.Stop();
			this.NextPart();
			this.timer.Start();
		}

		private void NextPart()
		{
			switch (this.currentPart)
			{
				case 0:
					this.Root.Children.RemoveAt(1);
					this.Root.Children.Add((Canvas) this.LoadResource("Monotone.Title.xaml"));
					break;

				case 1:
					this.Root.Children.RemoveAt(1);
					Spiral spiral = new Spiral();
					this.Root.Children.Add(spiral);
					spiral.Start();
					break;

				case 2:
					((Spiral)this.Root.Children[1]).Stop();
					this.Root.Children.RemoveAt(1);
					Stroposcope stroposcope = new Stroposcope();
					this.Root.Children.Add(stroposcope);
					stroposcope.Start();
					break;

				case 3:
					((Stroposcope)this.Root.Children[1]).Stop();
					this.Root.Children.RemoveAt(1);
					this.Root.Children.Add((Canvas) this.LoadResource("Monotone.End.xaml"));
					break;
			}

			this.currentPart++;
		}

		private DependencyObject LoadResource(string resourceName)
		{
			using (StreamReader reader = new StreamReader(this.GetType().Assembly.GetManifestResourceStream(resourceName)))
			{
				return (DependencyObject) XamlReader.Load(reader.ReadToEnd());
			}
		}
	}
}
