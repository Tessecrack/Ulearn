using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delegates.Reports
{
    public delegate object MakeStatistics(IEnumerable<double> data);
    public class Format
    {
        public Func<string, string> MakeCaption;
        public Func<string> BeginList;
        public Func<string> EndList;
        public Func<string, string, string> MakeItem;
    }

    public class Statistic
    {
        public string Caption;
        public MakeStatistics MakeStatistics;
    }

    public class HtmlFormat : Format
    {
        public HtmlFormat()
        {
            MakeCaption = (caption) => $"<h1>{caption}</h1>";
            BeginList = () => "<ul>";
            EndList = () => "</ul>";
            MakeItem = (valueType, entry) => $"<li><b>{valueType}</b>: {entry}";
        }
    }
    public class MarkdownFormat : Format
    {
        public MarkdownFormat()
        {
            MakeCaption = (caption) => $"## {caption}\n\n";
            BeginList = () => "";
            EndList = () => "";
            MakeItem = (valueType, entry) => $" * **{valueType}**: {entry}\n\n";
        }
    }
    public class MeanAndStdStatistics : Statistic
    {
        public MeanAndStdStatistics()
        {
            Caption = "Mean and Std";
            MakeStatistics = (data) =>
            {
                var listOfData = data.ToList();
                var meanValue = data.Average();
                var stdValue = Math.Sqrt(data.Select(z => Math.Pow(z - meanValue, 2)).Sum() / (listOfData.Count - 1));
                return new MeanAndStd
                {
                    Mean = meanValue,
                    Std = stdValue
                };
            };  
        }
    }
    public class MedianStatistics : Statistic
    {
        public MedianStatistics()
        {
            Caption = "Median";
            MakeStatistics = (data) =>
            {
                var list = data.OrderBy(z => z).ToList();
                if (list.Count % 2 == 0)
                    return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
                return list[list.Count / 2];
            };
        }
    } 

    public class ReportMaker
    {
        public string ReportCreator(IEnumerable<Measurement> measurements, Format format, Statistic statistic)
        {
            var data = measurements.ToList();
            var result = new StringBuilder();
            result.Append(format.MakeCaption(statistic.Caption));
            result.Append(format.BeginList());
            result.Append(format.MakeItem("Temperature", statistic.MakeStatistics(data.Select(z => z.Temperature)).ToString()));
            result.Append(format.MakeItem("Humidity", statistic.MakeStatistics(data.Select(z => z.Humidity)).ToString()));
            result.Append(format.EndList());
            return result.ToString();
        }
    }
	public static class ReportMakerHelper
	{
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data) => 
            new ReportMaker().ReportCreator(data, new HtmlFormat(), new MeanAndStdStatistics());
		public static string MedianMarkdownReport(IEnumerable<Measurement> data) =>
            new ReportMaker().ReportCreator(data, new MarkdownFormat(), new MedianStatistics());
		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements) =>
            new ReportMaker().ReportCreator(measurements, new MarkdownFormat(), new MeanAndStdStatistics());
        public static string MedianHtmlReport(IEnumerable<Measurement> measurements) =>
            new ReportMaker().ReportCreator(measurements, new HtmlFormat(), new MedianStatistics());
	}
}
