using System.Runtime.CompilerServices;
using static System.Math;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.IO;
using SkiaSharp;

namespace Segmented
{
    public class PieChartView : SKCanvasView
    {
        private const string SvgMainImagePattern =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<svg version=""1.1"" id=""Layer_1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" x=""0px"" y=""0px""
width=""{0}px"" height=""{0}px"" viewBox=""0 0 {0} {0}"" enable-background=""new 0 0 {0} {0}"" xml:space=""preserve"">
{1}
{2}
</svg>";

        private const string SvgSegmentPatter =
@"<path fill=""{5}"" d=""M{0},{0} L{0},0 A{0},{0} 1 {4},1 {1}, {2} z"" transform=""rotate({3}, {0}, {0})"" />";

        private const string SvgCenterCircle = 
@"<circle cx=""{0}"" cy=""{0}"" r=""{1}"" fill=""{2}""/>";

        private const double Degree360 = 360;
        private const double Degree180 = 180;
        private const double Degree90 = 180;

        public static readonly BindableProperty SegmentsSourceProperty = BindableProperty.Create(nameof(SegmentsSource), typeof(List<SegmentInfo>), typeof(PieChartView), null);

        public static readonly BindableProperty SeparatorPercentageProperty = BindableProperty.Create(nameof(SeparatorPercentage), typeof(double), typeof(PieChartView), 0.005);

        public static readonly BindableProperty CenterCirclePercentageProperty = BindableProperty.Create(nameof(CenterCirclePercentage), typeof(double), typeof(PieChartView), 0.5);

        public static readonly BindableProperty SeparatorColorProperty = BindableProperty.Create(nameof(SeparatorColor), typeof(Color), typeof(PieChartView), Color.White);

        private double _lastDrawSize;

        private SkiaSharp.Extended.Svg.SKSvg _svgHolder;

        public List<SegmentInfo> SegmentsSource
        {
            get => GetValue(SegmentsSourceProperty) as List<SegmentInfo>;
            set => SetValue(SegmentsSourceProperty, value);
        }

        public double SeparatorPercentage
        {
            get => (double)GetValue(SeparatorPercentageProperty);
            set => SetValue(SeparatorPercentageProperty, value);
        }

        public double CenterCirclePercentage
        {
            get => (double)GetValue(CenterCirclePercentageProperty);
            set => SetValue(CenterCirclePercentageProperty, value);
        }

        public Color SeparatorColor
        {
            get => (Color)GetValue(SeparatorColorProperty);
            set => SetValue(SeparatorColorProperty, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(Height) ||
                propertyName == nameof(Width) ||
                propertyName == nameof(WidthRequest) ||
                propertyName == nameof(HeightRequest)
                )
            {
                Draw(true);
                return;
            }

            if (propertyName == nameof(SegmentsSource) ||
                propertyName == nameof(SeparatorPercentage) ||
                propertyName == nameof(CenterCirclePercentage) ||
                propertyName == nameof(SeparatorColor)
                )
            {
                Draw(false);
                return;
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            var surface = e.Surface;
            var canvas = surface.Canvas;

            var width = e.Info.Width;
            var height = e.Info.Height;

            canvas.Clear(SKColors.White);

            if (_svgHolder == null)
            {
                return;
            }

            var canvasMin = Min(width, height);
            var svgMax = Max(_svgHolder.Picture.CullRect.Width, _svgHolder.Picture.CullRect.Height);
            var scale = canvasMin / svgMax;
            var matrix = SKMatrix.MakeScale(scale, scale);

            canvas.DrawPicture(_svgHolder.Picture, ref matrix);
        }

        private void Draw(bool checkLastDrawSize)
        {
            var width = WidthRequest > 0 
                ? WidthRequest 
                : Width;

            var height = HeightRequest > 0
                ? HeightRequest
                : Height;

            if (width <= 0 || height <= 0 || (!(SegmentsSource?.Any() ?? false)))
            {
                return;
            }

            var halfSize = ((int)Min(Width, Height)) / 2;
            if (checkLastDrawSize && Abs(_lastDrawSize - halfSize) < double.Epsilon)
            {
                return;
            }
            _lastDrawSize = halfSize;

            var itemsCount = SegmentsSource.Count;

            var totalPercentage = itemsCount <= 1
                ? 1
                : 1 - itemsCount * SeparatorPercentage;


            var rotation = 0.0;
            var segmentsToDraw = new List<SegmentInfo>();
            foreach (var item in SegmentsSource)
            {
                segmentsToDraw.Add(new SegmentInfo
                {
                    Percentage = SeparatorPercentage,
                    Color = SeparatorColor
                });
                segmentsToDraw.Add(new SegmentInfo
                {
                    Percentage = item.Percentage * totalPercentage,
                    Color = item.Color
                });
            }

            var segmentsBuilder = new StringBuilder();

            foreach (var item in segmentsToDraw)
            {
                var angle = Degree360 * item.Percentage;
                var angleCalculated = (angle > Degree180) ? Degree360 - angle : angle;
                var angleRad = angleCalculated * PI / Degree180;

                var perpendicularDistance = halfSize * Sin(angleRad);
                if (angleCalculated > Degree90)
                {
                    perpendicularDistance = halfSize * Sin((Degree180 - angleCalculated) * PI / Degree180);
                }

                var topPointDistance = Sqrt(2 * halfSize * halfSize - (2 * halfSize * halfSize * Cos(angleRad)));
                var y1 = Sqrt(topPointDistance * topPointDistance - perpendicularDistance * perpendicularDistance);
                var x1 = halfSize + perpendicularDistance;
                var obtuseAngleFlag = 0;

                if (angle > Degree180)
                {
                    x1 = halfSize - perpendicularDistance;
                    obtuseAngleFlag = 1;
                }

                segmentsBuilder.AppendLine(string.Format(SvgSegmentPatter, halfSize, x1, y1, rotation, obtuseAngleFlag, GetHexColor(item.Color)));

                rotation = rotation + angle;
            }

            var centerCiclerSvg = string.Format(SvgCenterCircle, halfSize, halfSize * CenterCirclePercentage, GetHexColor(SeparatorColor));

            var fullSvg = string.Format(SvgMainImagePattern, halfSize * 2, segmentsBuilder, centerCiclerSvg);


            _svgHolder = new SkiaSharp.Extended.Svg.SKSvg();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fullSvg)))
            {
#pragma warning disable CS1701 // Assuming assembly reference matches identity
                _svgHolder.Load(stream);
#pragma warning restore CS1701 // Assuming assembly reference matches identity
            }

            InvalidateSurface();
        }

        private string GetHexColor(Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }
    }
}