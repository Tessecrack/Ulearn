using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delegates.Reports
{
	public abstract class StatisticMaker
	{
		public abstract string Caption { get; }
		public abstract StatisticCreator StatisticCreator { get; }
	}

	public delegate object StatisticCreator(IEnumerable<double> data);
	public abstract class FormatMaker
	{
		public abstract Func<string, string> CaptionMake { get; }
		public abstract Func<string, string, string> ItemMake { get; }
		public abstract Func<string> ListBegin { get; }
		public abstract Func<string> ListEnd { get; }
    }

    public class FormatHTML : FormatMaker
    {
        public override Func<string, string> CaptionMake => (cap) => $"<h1>{cap}</h1>"; 
        public override Func<string, string, string> ItemMake
			=> (valueType, entry) => $"<li><b>{valueType}</b>: {entry}"; 
        public override Func<string> ListBegin  => () => "<ul>"; 
        public override Func<string> ListEnd => () => "</ul>"; 
    }

    public class FormatMarkdown : FormatMaker
    {
        public override Func<string, string> CaptionMake => (cap) => $"## {cap}\n\n";
		public override Func<string, string, string> ItemMake
			=> (value, entry) => $" * **{value}**: {entry}\n\n";
		public override Func<string> ListBegin => () => "";
		public override Func<string> ListEnd => () => "";
    }

    public class MeanAndStdStatisticMaker : StatisticMaker
	{
		public override string Caption => "Mean and Std";
		public override StatisticCreator StatisticCreator => (dataSource) =>
		{
			var data = dataSource.ToList();
			var mean = data.Average();
			var std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (data.Count - 1));

			return new MeanAndStd
			{
				Mean = mean,
				Std = std
			};
		};
    }

	public class MedianStatisticMaker : StatisticMaker
	{
		public override string Caption => "Median";
		public override StatisticCreator StatisticCreator => (dataSource) =>
		{
			var list = dataSource.OrderBy(z => z).ToList();
			if (list.Count % 2 == 0)
				return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
			return list[list.Count / 2];
		};
    }

	public static class ReportMakerHelper
	{
		public static string MakeReport(IEnumerable<Measurement> dataSource , 
			FormatMaker formatMaker, StatisticMaker statisticMaker)
		{
			var data = dataSource.ToList();
			var result = new StringBuilder();
			result.Append(formatMaker.CaptionMake(statisticMaker.Caption));
			result.Append(formatMaker.ListBegin());
			result.Append(formatMaker
				.ItemMake("Temperature", 
				statisticMaker.StatisticCreator(data.Select(z => z.Temperature)).ToString()));
			result.Append(formatMaker.ItemMake("Humidity",
				statisticMaker.StatisticCreator(data.Select(z => z.Humidity)).ToString()));
			result.Append(formatMaker.ListEnd());
			return result.ToString();
		}

		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data) 
			=> MakeReport(data, new FormatHTML(), new MeanAndStdStatisticMaker());
		public static string MedianMarkdownReport(IEnumerable<Measurement> data) 
			=> MakeReport(data, new FormatMarkdown(), new MedianStatisticMaker());
		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements) 
			=> MakeReport(measurements, new FormatMarkdown(), new MeanAndStdStatisticMaker());
		public static string MedianHtmlReport(IEnumerable<Measurement> measurements) 
			=> MakeReport(measurements, new FormatHTML(), new MedianStatisticMaker());
	}
}
