namespace Monotone
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Markup;
	using System.Windows.Media;
	using System.Windows.Resources;
	using System.Windows.Shapes;
	using System.Windows.Threading;

	public class Application : System.Windows.Application
	{
		private DispatcherTimer timer = new DispatcherTimer();
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

		private class Spiral : Canvas
		{
			private DispatcherTimer timer = new DispatcherTimer();
			private int count;
			private double angle = 0;
			private double radius = 15;
			private RotateTransform rotateTransform;
			private ScaleTransform scaleTransform;
	
			public Spiral()
			{
				this.Width = 600;
				this.Height = 400;
				this.RenderTransformOrigin = new Point(0.5, 0.5);
	
				this.rotateTransform = new RotateTransform();
				this.rotateTransform.Angle = 0;
	
				this.scaleTransform = new ScaleTransform();
				this.scaleTransform.ScaleX = 1;
				this.scaleTransform.ScaleY = 1;
	
				TransformGroup renderTransform = new TransformGroup();
				renderTransform.Children.Add(this.rotateTransform);
				renderTransform.Children.Add(this.scaleTransform);
	
				this.RenderTransform = renderTransform;
			}
	
			public void Start()
			{
				this.timer.Interval = new TimeSpan(0,0,0,0,(int)((60f / 140f) * 50f));
				this.timer.Tick += new EventHandler(this.Timer_Tick);
				this.timer.Start();
			}
	
			public void Stop()
			{
				// this.timer.Tick -= new EventHandler(this.Timer_Tick);
				this.timer.Stop();
				this.timer = null;
				this.Children.Clear();
			}
	
			private void Timer_Tick(object sender, EventArgs e)
			{
				if ((this.timer != null) && (this.timer.IsEnabled))
				{
					int zoom = 330;
					int fade = 410;
	
					if (count < zoom)
					{
						Ellipse ellipse = new Ellipse();
						ellipse.Width = 16;
						ellipse.Height = 16;
						ellipse.StrokeThickness = 3;
						ellipse.Stroke = new SolidColorBrush(Colors.White);
	
						double rad = (this.angle * Math.PI) / 180f;
						double y = 200 - 6 + (Math.Sin(rad) * this.radius);
						double x = 300 - 6 + (Math.Cos(rad) * this.radius);
						Canvas.SetTop(ellipse, y);
						Canvas.SetLeft(ellipse, x);
	
						if ((this.count % 5) == 0)
						{
							ellipse.Fill = new SolidColorBrush(Colors.White);
						}
	
						this.Children.Add(ellipse);
	
						double length = 2 * Math.PI * this.radius;
	
						int distance = 20;
	
						this.angle += 360 * distance / length;
						this.radius += distance / (length / distance);
					}
	
					if (this.count > zoom)
					{
						this.rotateTransform.Angle -= (count - zoom) / 2;
						this.scaleTransform.ScaleX *= 1.025f;
						this.scaleTransform.ScaleY *= 1.025f;
					}
	
					if (this.count > fade)
					{
						int color = (int)((count - fade) * 11.0);
						if (color > 255)
						{
							color = 255;
						}
	
						this.Background = new SolidColorBrush(Color.FromArgb(255, (byte)color, (byte)color, (byte)color));
					}
	
					this.rotateTransform.Angle -= 9;
	
					this.count++;
				}
			}
		}

		private class Stroposcope : Canvas
		{
			private DispatcherTimer timer = new DispatcherTimer();
			private List<Ring> rings = new List<Ring>();
			private int count = 0;
			private int ringSize = 50;
			private int ringCount = 10;
	
			public void Start()
			{
				this.Background = new SolidColorBrush(Colors.Black);
				this.Width = 600;
				this.Height = 400;
				this.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, this.Width, this.Height) };
	
				this.timer.Interval = new TimeSpan(0,0,0,0,(int)((60f / 140f) * 50f));
				this.timer.Tick += new EventHandler(this.Timer_Tick);
				this.timer.Start();
	
				this.rings.Add(new Ring(20 * 8, 25 * 8, 1.2f, 0.8f, 0.30, 60 * 8, 0, 6, ringSize, ringCount));
				this.rings.Add(new Ring(35 * 8, 10 * 8, 1.0f, 0.7f, 0.35, 80 * 8, 0, -6, ringSize, ringCount));
				this.rings.Add(new Ring(40 * 8, 12 * 8, 1.0f, 1.0f, 0.20, 70 * 8, 0, 4, ringSize, ringCount));
	
				for (int i = 0; i < this.rings.Count; i++)
				{
					this.Children.Add(this.rings[i]);
				}
			}
	
			public void Stop()
			{
				// this.timer.Tick -= new EventHandler(this.Timer_Tick);
				this.timer.Stop();
				this.timer = null;
			}
	
			private void Timer_Tick(object sender, EventArgs e)
			{
				if ((this.timer != null) && (this.timer.IsEnabled))
				{
					int index = this.rings.Count;
	
					if ((this.count > (219 - 26)) && (this.count < 219))
					{
						int color = (int)((this.count - 219 - 26) * 10f);
						if (color > 255)
						{
							color = 255;
						}
	
						this.Background = new SolidColorBrush(Color.FromArgb(255, (byte)color, (byte)color, (byte)color));
					}
	
					if ((this.count >= (219)) && (this.count < (219 + 26)))
					{
						int color = (int) ((219 + 26 - this.count) * 10f);
						color = (color < 0) ? 0 : color;
						color = (color > 255) ? 255 : color;
						this.Background = new SolidColorBrush(Color.FromArgb(255, (byte)color, (byte)color, (byte)color));
					}
	
					if ((this.count > (438 - 26)) && (this.count < 438))
					{
						double x = Canvas.GetLeft(this);
						x -= 30;
						Canvas.SetLeft(this, x);
					}
	
					if (this.count == 219)
					{
						this.rings.Add(new Ring(20 * 8, 25 * 8, 1.2f, 0.8f, 0.30, 60 * 8, 40, 6, ringSize, ringCount));
						this.rings.Add(new Ring(35 * 8, 10 * 8, 1.0f, 0.7f, 0.35, 80 * 8, -150, -6, ringSize, ringCount));
						this.rings.Add(new Ring(40 * 8, 12 * 8, 1.0f, 1.0f, 0.20, 70 * 8, 123, 4, ringSize, ringCount));
					}
	
					foreach (Ring ring in this.rings)
					{
						ring.Tick();
					}
	
					if (this.count == 219)
					{
						for (int i = index; i < this.rings.Count; i++)
						{
							this.Children.Add(this.rings[i]);
						}
					}
	
					this.count++;
				}
			}
	
			private class Ring : Canvas
			{
				private double angle = 0;
				private double radius = 0;
				private double increment = 0;
				private float x;
				private float y;
				private float dx;
				private float dy;
	
				public Ring(float x, float y, float dx, float dy, double opacity, double radius, double angle, double increment, int size, int count)
				{
					this.Opacity = opacity;
	
					this.radius = radius;
					this.angle = angle;
					this.increment = increment;
					this.x = x;
					this.y = y;
					this.dx = dx;
					this.dy = dy;
	
					for (int i = 0; i < count; i++)
					{
						double distance = i * size * 4;
	
						Ellipse ellipse = new Ellipse()
						{
							Stroke = new SolidColorBrush(Colors.White),
							StrokeThickness = size,
							Width = distance,
							Height = distance,
						};
	
						Canvas.SetTop(ellipse, -(distance / 2));
						Canvas.SetLeft(ellipse, -(distance / 2));
		
						this.Children.Add(ellipse);
					}
				}
	
				public void Tick()
				{
					double rad = (this.angle * Math.PI) / 180f;
					double x = this.x + (Math.Cos(rad) * this.radius * this.dx);
					double y = this.y + (Math.Sin(rad) * this.radius * this.dy);
					Canvas.SetTop(this, y);
					Canvas.SetLeft(this, x);
					this.angle += this.increment;
				}
			}
		}
	}
}
