using Xamarin.Forms;
using System.Collections.Generic;

namespace Segmented
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Content = new StackLayout
            {
                BackgroundColor = Color.White,
                Children =
                {
                    new SegmentedCircleView
                    {
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        WidthRequest = 230,
                        HeightRequest = 230,
                        SegmentsSource = new List<SegmentInfo>
                        {
                            new SegmentInfo
                            {
                                Percentage = 0.125,
                                Color = Color.Brown
                            },
                            new SegmentInfo
                            {
                                Percentage = 0.2,
                                Color = Color.Red
                            },
                            new SegmentInfo
                            {
                                Percentage = 0.075,
                                Color = Color.Red
                            },
                            new SegmentInfo
                            {
                                Percentage = 0.6,
                                Color = Color.Black
                            }
                        }
                    }
                }
            };
        }
    }
}
       
