using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Globalization;

namespace Parsing
{
    internal class SaveToCsv
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SaveToCsv));
        string path = "etfs.tsv";
        public void Save(List<EtfData> etfMarketDatas)
        {

            using (var writer = new StreamWriter(path))
            {

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    //Encoding = Encoding.UTF8,
                    Encoding = new UTF8Encoding(true),
                    Delimiter = "\t",
                    HasHeaderRecord = true, // включаем заголовки
                    Mode = CsvMode.RFC4180, // стандартный режим CSV
                };
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.Context.RegisterClassMap<EtfMap>();
                    csv.WriteRecords(etfMarketDatas);
                }
            }

            //using (var writer = new StreamWriter("ETF.csv"))
            //{
            //    foreach (var row in etfMarketDatas)
            //    {
            //        string csvLine = string.Join("\t", row);
            //        writer.WriteLine(csvLine);
            //    }
            //}
            //using (var writer = new StreamWriter("etfs.csv"))
            //using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //{
            //    csv.WriteRecords(etfMarketDatas);
            //}

            //using (var writer = new StreamWriter(path, false))
            //{
            //    var csvConfig = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            //    {
            //        //HasHeaderRecord = true,
            //        Delimiter = "\t"
            //    };
            //    using (var csv = new CsvWriter(writer, csvConfig))
            //    {
            //        csv.WriteRecords(etfMarketDatas);
            //    }
            //}
        }
    }
}

