namespace DotnetCourseowork.Models;


public class ChartData
{
    public List<string> Labels { get; set; }
    public List<ChartDataset> Datasets { get; set; }
}

public class ChartDataset
{
    public string Label { get; set; }
    public List<double> Data { get; set; }
    public List<string> BackgroundColor { get; set; }
    public List<string> BorderColor { get; set; }
    public int BorderWidth { get; set; }
}

public class ChartOptions
{
    public bool Responsive { get; set; }
    public Scales Scales { get; set; }
}

public class Scales
{
    public List<ChartAxis> YAxes { get; set; }
}

public class ChartAxis
{
    public Ticks Ticks { get; set; }
}

public class Ticks
{
    public bool BeginAtZero { get; set; }
}

