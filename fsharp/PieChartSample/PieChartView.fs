namespace PieChartSample

open System.Text
open System.Collections.Generic
open System.Linq
open Xamarin.Forms
open System.IO
open SkiaSharp
open System

type PieChartView () =
    inherit Image () 

    let SvgMainImagePattern = @"<?xml version=""1.0"" encoding=""utf-8""?>
    <svg version=""1.1"" id=""Layer_1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" x=""0px"" y=""0px""
    width=""{0}px"" height=""{0}px"" viewBox=""0 0 {0} {0}"" enable-background=""new 0 0 {0} {0}"" xml:space=""preserve"">
    {1}
    {2}
    </svg>"
    let SvgSegmentPatter = @"<path fill=""{5}"" d=""M{0},{0} L{0},0 A{0},{0} 1 {4},1 {1}, {2} z"" transform=""rotate({3}, {0}, {0})"" />"
    let SvgCenterCircle = @"<circle cx=""{0}"" cy=""{0}"" r=""{1}"" fill=""{2}""/>"
    let Degree360 = 360.
    let Degree180 = 180.
    let Degree90 = 90.

    static let SegmentsSourceProperty = BindableProperty.Create("SegmentsSource", typeof<seq<SegmentInfo>>, typeof<PieChartView>, null)
    static let SeparatorPercentageProperty = BindableProperty.Create("SeparatorPercentage", typeof<float>, typeof<PieChartView>, 0.005)
    static let CenterCirclePercentageProperty = BindableProperty.Create("CenterCirclePercentage", typeof<float>, typeof<PieChartView>, 0.5)
    static let SeparatorColorProperty = BindableProperty.Create("SeparatorColor", typeof<Color>, typeof<PieChartView>, Color.White)

    let mutable lastDrawableSize : int = 0 

    let BuildColorPart(part : float) : int =
        int(part * 255.)

    let GetHexColor(color : Color) : string =
        let red =  BuildColorPart color.R
        let green = BuildColorPart color.G
        let blue = BuildColorPart color.B
        let alpha = BuildColorPart color.A
        String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", alpha, red, green, blue)
       
    member this.SegmentsSource
        with get () = this.GetValue SegmentsSourceProperty :?> seq<SegmentInfo>
        and set (value:seq<SegmentInfo>) = this.SetValue(SegmentsSourceProperty, value)

    member this.SeparatorPercentage
        with get () = this.GetValue SeparatorPercentageProperty :?> float
        and set (value:float) = this.SetValue(SeparatorPercentageProperty, value)

    member this.CenterCirclePercentage
        with get () = this.GetValue CenterCirclePercentageProperty :?> float
        and set (value:float) = this.SetValue(CenterCirclePercentageProperty, value)

    member this.SeparatorColor
        with get () = this.GetValue SeparatorColorProperty :?> Color
        and set (value:Color) = this.SetValue(SeparatorColorProperty, value)

    member this.Draw (checkLastDrawSize : bool) =
        let width = if base.WidthRequest > 0. then base.WidthRequest else base.Width
        let height = if base.HeightRequest > 0. then base.HeightRequest else base.Height
        
        if width <= 0. || 
            height <= 0. || 
            this.SegmentsSource = null || 
            not(this.SegmentsSource.Any()) 
        then
             () // exit
            
        else

        let halfSize = (int(Math.Min(width, height))) / 2
        if checkLastDrawSize && Math.Abs(lastDrawableSize - halfSize) <= 0 
        then
            () // exit

        else

        lastDrawableSize <- halfSize

        let itemsCount = this.SegmentsSource.Count()
        let totalPercentage = if itemsCount > 1 then 1. - float(itemsCount) * this.SeparatorPercentage else 1.
        
        let segmentsToDraw = List<SegmentInfo>()
        for item in this.SegmentsSource do
            segmentsToDraw.Add { SegmentInfo.Color=this.SeparatorColor; Percentage=this.SeparatorPercentage }
            segmentsToDraw.Add { SegmentInfo.Color=item.Color; Percentage=item.Percentage * totalPercentage }

        let mutable rotation = 0.
        let segmentsBuilder = StringBuilder();
        
        for item in segmentsToDraw do 
            let angle = Degree360 * item.Percentage
            let angleCalculated = if angle > Degree180 then Degree360 - angle else angle
            let angleRad = angleCalculated * Math.PI / Degree180
            let perpendicularDistance = if angleCalculated > Degree90 then float(halfSize) * Math.Sin((Degree180 - angleCalculated) * Math.PI / Degree180) else float(halfSize) * Math.Sin(angleRad)
            let topPointDistance = Math.Sqrt(float(2 * halfSize * halfSize) - (float (2 * halfSize * halfSize) * Math.Cos(angleRad)))
            let y1 = Math.Sqrt(topPointDistance * topPointDistance - perpendicularDistance * perpendicularDistance)
            let x1 = if angle > Degree180 then float(halfSize) - perpendicularDistance else float(halfSize) + perpendicularDistance;
            let obtuseAngleFlag = if angle > Degree180 then 1 else 0
            segmentsBuilder.AppendLine(String.Format(SvgSegmentPatter, halfSize, x1, y1, rotation, obtuseAngleFlag, GetHexColor(item.Color))) |> ignore
            rotation <- rotation + angle
            ()


        let centerCiclerSvg = String.Format(SvgCenterCircle, halfSize, float(halfSize) * this.CenterCirclePercentage, GetHexColor(this.SeparatorColor))
        let fullSvg = String.Format(SvgMainImagePattern, halfSize * 2, segmentsBuilder, centerCiclerSvg);
        let svgHolder = SkiaSharp.Extended.Svg.SKSvg();

        use stream = new MemoryStream(Encoding.UTF8.GetBytes(fullSvg))
        svgHolder.Load(stream) |> ignore

        let canvasSize = svgHolder.CanvasSize
        let cullRect = svgHolder.Picture.CullRect

        use bitmap = new SKBitmap(int(canvasSize.Width), int(canvasSize.Height))       
        use canvas = new SKCanvas(bitmap)
        let canvasMin = Math.Min(canvasSize.Width, canvasSize.Height)
        let svgMax = Math.Max(cullRect.Width, cullRect.Height)
        let scale = canvasMin / svgMax
        let matrix = SKMatrix.MakeScale(scale, scale)
        canvas.Clear(SKColor.Empty)
        canvas.DrawPicture(svgHolder.Picture, ref matrix)
        canvas.Flush()
        canvas.Save() |> ignore
        use image = SKImage.FromBitmap(bitmap)
        let data = image.Encode(SKEncodedImageFormat.Png, Int32.MaxValue)       
        this.Source <- ImageSource.FromStream(fun _ -> data.AsStream())
        ()

    override this.OnPropertyChanged(propertyName : string) =
        if  propertyName = "Height" ||
            propertyName = "Width" ||
            propertyName = "WidthRequest" || 
            propertyName = "HeightRequest" then this.Draw(true)
        elif propertyName = "SegmentsSource" ||
            propertyName = "SeparatorPercentage" ||
            propertyName = "CenterCirclePercentage" || 
            propertyName = "SeparatorColor" then this.Draw(false)
        ()