namespace PieChartSample

open System.Runtime.CompilerServices
open System.Text
open System.Collections.Generic
open System.Linq
open Xamarin.Forms
open SkiaSharp.Views.Forms
open System.IO
open SkiaSharp

type PieChartView () =
    inherit SKCanvasView () 

    let SvgMainImagePattern = @"<?xml version=""1.0"" encoding=""utf-8""?>
    <svg version=""1.1"" id=""Layer_1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" x=""0px"" y=""0px""
    width=""{0}px"" height=""{0}px"" viewBox=""0 0 {0} {0}"" enable-background=""new 0 0 {0} {0}"" xml:space=""preserve"">
    {1}
    {2}
    </svg>"
    let SvgSegmentPatter = @"<path fill=""{5}"" d=""M{0},{0} L{0},0 A{0},{0} 1 {4},1 {1}, {2} z"" transform=""rotate({3}, {0}, {0})"" />"
    let SvgCenterCircle = @"<circle cx=""{0}"" cy=""{0}"" r=""{1}"" fill=""{2}""/>"
    let Degree360 = 360
    let Degree180 = 180
    let Degree90 = 90

    static let SegmentsSourceProperty = BindableProperty.Create("SegmentsSource", typeof<List<SegmentInfo>>, typeof<PieChartView>, null)
    static let SeparatorPercentageProperty = BindableProperty.Create("SeparatorPercentage", typeof<float>, typeof<PieChartView>, 0.005)
    static let CenterCirclePercentageProperty = BindableProperty.Create("CenterCirclePercentage", typeof<float>, typeof<PieChartView>, 0.5)
    static let SeparatorColorProperty = BindableProperty.Create("SeparatorColor", typeof<Color>, typeof<PieChartView>, Color.White)

    member this.SegmentsSource
        with get () = this.GetValue SegmentsSourceProperty :?> Color
        and set (value:Color) = this.SetValue(SegmentsSourceProperty, value)
        
