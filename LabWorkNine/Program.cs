using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;

namespace LabWorkNine
{
    class Finance
    {
        public List<Finance> Records { get; set; } = new List<Finance>();

        static public List<string> ReadFromFile()
        {
            List<string> tickers = File.ReadAllLines(@"C:\Users\gkras\OneDrive\Рабочий стол\ticker.txt").ToList();
            return tickers;
        }

        static public async Task<double> GetAverageAsync(string ticker)
        {
            string apiUrl = $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1=1665223608&period2=1696759608&interval=1d&events=history&includeAdjustedClose=true";
           // Console.WriteLine(apiUrl);
            using (HttpClient client = new HttpClient())
            {
                string responseBody = await client.GetStringAsync(apiUrl);
                string[] lines = responseBody.Split('\n');
                lines = lines.Skip(1).ToArray();
                double totalAveragePrice = 0;
                int totalDays = 0;

                foreach (string line in lines)
                {
                    string[] values = line.Split(',');

                    double high = Convert.ToDouble(values[2], CultureInfo.InvariantCulture);
                    double low = Convert.ToDouble(values[3], CultureInfo.InvariantCulture);
                    double averagePrice = (high + low) / 2;

                    totalAveragePrice += averagePrice;
                    totalDays++;
                }

                double averagePriceForYear = totalAveragePrice / totalDays;
                return averagePriceForYear;
            }
        }

    }


    class Program
    {
        static async Task Main()
        {
            List<string> tickers = Finance.ReadFromFile();
            Dictionary<string, double> data = new Dictionary<string, double>();
            foreach (var ticker in tickers)
            {
                await Task.Delay(TimeSpan.FromSeconds(1)); 
                double averagePrice = await Finance.GetAverageAsync(ticker);
                data.Add(ticker, averagePrice);
            }

            string filePath = @"C:\Users\gkras\OneDrive\Рабочий стол\Result.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var dt in data)
                {
                    writer.WriteLine(dt.Key + ":" + dt.Value);
                }
            }

            Console.WriteLine("Строки успешно записаны в файл.");
        }
    }

}

