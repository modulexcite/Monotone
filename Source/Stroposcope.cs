namespace Monotone
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using System.Windows.Shapes;
	using System.Windows.Threading;

	internal class Stroposcope : Canvas
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