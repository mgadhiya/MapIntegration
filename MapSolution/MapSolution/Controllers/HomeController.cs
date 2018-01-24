using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Text;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using System.Drawing;
using System.Web.UI.WebControls;


namespace MapSolution.Controllers
{
    public class HomeController : Controller
    {
        MyReleaseDBEntities dbContext = new MyReleaseDBEntities();
        public ActionResult Index()
        {
            var model = new TestViewModel();
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            List<string> xValues = new List<string>();
            List<string> yValues = new List<string>();

            //var list = dbContext.GetCityFamilyCount();

            //if (list != null)
            //{
            //    foreach (var item in list)
            //    {
            //        xValues.Add(item.City);
            //        yValues.Add(item.FamilyCount.ToString());
            //    }
            //}

            var list = dbContext.GetLatlongData();
            var mapList = list.Select(a => new MapModel
            {
                Lat = a.latitude.Value,
                Lng = a.longtitude.Value,
                count = a.FamilyCount.Value
            }).ToList();

           


            model.GraphModel = new GraphModel();
            model.GraphModel.XValues = xValues;
            model.GraphModel.YValues = yValues;

            model.Name = "Gitesh Patel";
            var serializer = new JavaScriptSerializer();
            model.JsonData = serializer.Serialize(mapList);
            model.MapList = mapList;
            return View(model);
        }

        public ActionResult Chart()
        {
            var list = dbContext.GetCityFamilyCount().ToList();
            var xvalues = list.Select(a => a.City + " - " + a.zip_code).ToArray();
            var yvalues = list.Select(a => a.FamilyCount).ToArray();

            Chart chart = new Chart();
            chart.ChartAreas.Add(new ChartArea());

            chart.BackColor = Color.Transparent;
            chart.Width = Unit.Pixel(600);
            chart.Height = Unit.Pixel(400);

            chart.Series.Add(new Series("Data"));
            chart.Series["Data"].ChartType = SeriesChartType.Pie;

            chart.Series["Data"]["PieLabelStyle"] = "Disabled";
            chart.Series["Data"].Label = string.Empty;
            chart.ChartAreas[0].Area3DStyle.Enable3D = true;
            chart.ChartAreas[0].Area3DStyle.Inclination = 10;
            chart.Series["Data"]["PieLineColor"] = "Black";
            chart.Series["Data"].Points.DataBindXY(
                xvalues, string.Empty,
                yvalues, string.Empty);

            
            chart.Legends.Add(new Legend("legned"));
            chart.Legends["legned"].LegendStyle  = LegendStyle.Column;

            chart.Series["Data"].Label = "#VALY";
            chart.Series["Data"].LegendText = "#VALY-#VALX";

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms, ChartImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }

        public ActionResult test()
        {
            var model = new TestViewModel();
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            var list = dbContext.GetLatlongData();
            var mapList = list.Select(a => new MapModel
            {
                Lat = a.latitude.Value,
                Lng = a.longtitude.Value,
                count = a.FamilyCount.Value
            }).ToList();

            model.Name = "Gitesh Patel";
            var serializer = new JavaScriptSerializer();
            model.JsonData = serializer.Serialize(mapList);
            model.MapList = mapList;
            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }
    }

    public class TestViewModel
    {
        public string Name { get; set; }
        public string JsonData { get; set; }
        public GraphModel GraphModel { get; set; }
        public List<MapModel> MapList { get; set; }
    }
    public class MapModel
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public int count { get; set; }
    }
    public class GraphModel
    {
        public List<string> XValues { get; set; }
        public List<string> YValues { get; set; }
    }

    //public static class HtmlCharthelper
    //{
    //    public static object  Generate(this HtmlHelper helper, GraphModel model)
    //    {
    //        var myChart = new Chart(width: 600, height: 400)
    //        .AddTitle("Chart Title")
    //        .AddSeries(
    //        name: "ChartTitle",
    //        chartType: "Pie",
    //        xValue: model.YValues.ToArray(),
    //        yValues: model.YValues.ToArray())
    //        .AddSeries(
    //        name: "legend",
    //        chartType: "Pie",
    //        xValue: model.XValues.ToArray(),
    //        yValues: model.XValues.ToArray()).AddLegend()
    //        .Write();
    //        return myChart;
    //    }
    //}
}
