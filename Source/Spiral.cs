namespace Monotone
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using System.Windows.Shapes;
	using System.Windows.Threading;

	internal class Spiral : Canvas
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
}