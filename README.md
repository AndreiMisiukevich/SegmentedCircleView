# SegmentedCircleView
SegmentedCircleView for xamarin forms


## Easy for use
- **Add and setup FFImageLoading plugin with SVG support!**

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
