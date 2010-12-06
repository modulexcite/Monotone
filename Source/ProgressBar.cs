namespace Monotone
{
	using System.Windows.Controls;
	using System.Windows.Media;
	using System.Windows.Shapes;

	internal class ProgressBar : Canvas
	{
		public ProgressBar()
		{
			Rectangle stroke = new Rectangle() 
			{
				Stroke = new SolidColorBrush(Colors.White),
				StrokeThickness = 1,
				Width = 100,
				Height = 16 
			};

			Rectangle fill = new Rectangle()
			{
				Fill = new SolidColorBrush(Colors.White),
				Stroke = new SolidColorBrush(Colors.White),
				StrokeThickness = 1,				
				Width = 1,
				Height = 16
			};

			this.Children.Add(stroke);
			this.Children.Add(fill);
		}

		public int Value
		{
			get
			{
				Rectangle fill = (Rectangle) this.Children[1];
				return (int) fill.Width;
			}

			set
			{
				Rectangle fill = (Rectangle) this.Children[1];
				fill.Width = value;
			}
		}
	}
}
