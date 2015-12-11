using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace WrappingPaper
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataString = InputData.GetData();
            var formatter = new PresentBoxDataFormatter(dataString);
            var presents = formatter.CreatePresents();

            var neededSqFt = 0;

            foreach (var present in presents)
            {
                neededSqFt += present.CalculateNeededWrappingPaper();
            }

            Console.Write("The elves need {0:##,###} sq ft of wrapping paper.", neededSqFt);
            Console.ReadLine();
        }
    }

    public static class InputData
    {
        private static Uri _baseUrl = new Uri("http://ryanvogel.github.io/", UriKind.Absolute);

        private static string _inputFile = "packages.txt";

        public static string GetData()
        {
            var restClient = new RestClient(_baseUrl);
            var restRequest = new RestRequest(_inputFile, Method.GET);
            var response = restClient.Execute(restRequest);

            return response.Content;
        }
    }

    public class PresentBoxDataFormatter
    {
        private string _dataString;

        public PresentBoxDataFormatter(string dataString)
        {
            _dataString = dataString;
        }

        public IEnumerable<PresentBox> CreatePresents()
        {
            var presents = _dataString
                .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(rec =>
                    {
                        var h = 0;
                        var l = 0;
                        var w = 0;
                        var dimensions = rec.Split('x');

                        if (int.TryParse(dimensions[0], out h)
                            && int.TryParse(dimensions[1], out l)
                            && int.TryParse(dimensions[2], out w))
                        {
                            return new PresentBox(h, l, w);
                        }

                        return new NoBox();
                    });

            return presents;
        } 
    }

    public class PresentBox
    {
        public int Height { get; private set; }
        public int Length { get; private set; }
        public int Width { get; private set; }

        public PresentBox(int height, int length, int width)
        {
            Height = height;
            Length = length;
            Width = width;
        }

        public virtual int CalculateNeededWrappingPaper()
        {
            var side1SqFt = (2 * Height * Length);
            var side2SqFt = (2 * Length * Width);
            var side3SqFt = (2 * Height * Width);

            var sidesSqFt = new List<int> { side1SqFt, side2SqFt, side3SqFt };
            var extraSqFt = sidesSqFt.Min();

            var neededSqFt = (side1SqFt + side2SqFt + side3SqFt + extraSqFt);

            return neededSqFt;
        }
    }

    public class NoBox : PresentBox
    {
        public NoBox()
            : base(0, 0, 0)
        {
        }

        public override int CalculateNeededWrappingPaper()
        {
            return 0;
        }
    }
}
