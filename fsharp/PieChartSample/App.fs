﻿namespace PieChartSample

open Xamarin.Forms

type App() =
    inherit Application()

    let BuildPieChart () =
        let pieChart = DonutChartView()
        pieChart.HorizontalOptions <- LayoutOptions.CenterAndExpand
        pieChart.VerticalOptions <- LayoutOptions.CenterAndExpand
        pieChart.WidthRequest <- 230.
        pieChart.HeightRequest <- 230.
        pieChart

    do
        let pieChart = BuildPieChart()

        pieChart.SegmentsSource <- [ { SegmentInfo.Color=Color.Brown; Percentage=0.6 };
        { SegmentInfo.Color=Color.Red; Percentage=0.2 };
        { SegmentInfo.Color=Color.Gold; Percentage=0.1 };
        { SegmentInfo.Color=Color.Black; Percentage=0.1 }
        ]
        
        let stack = StackLayout()
        stack.BackgroundColor <- Color.White
        stack.Children.Add pieChart

        base.MainPage <- ContentPage(Content = stack)