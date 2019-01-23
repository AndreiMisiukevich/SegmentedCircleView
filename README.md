# SegmentedCircleView
SegmentedCircleView for xamarin forms

![img](https://github.com/AndreiMisiukevich/SegmentedCircleView/blob/master/1.png?raw=true)

## Easy for use
- **Add and setup FFImageLoading plugin with SVG support!**
- Copy next classes to your NETSTANDARD proj
https://github.com/AndreiMisiukevich/SegmentedCircleView/blob/master/Segmented/SegmentedCircleView.cs
https://github.com/AndreiMisiukevich/SegmentedCircleView/blob/master/Segmented/SegmentInfo.cs

And use lik:
```csharp
new SegmentedCircleView
{
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
```
