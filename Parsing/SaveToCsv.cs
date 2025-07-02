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
        string path = "ETF.csv";
        public void Save(List<EtfMarketData> etfMarketDatas)
        {
            using (var writer = new StreamWriter(path, false))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
                {
                    //HasHeaderRecord = true,
                    Delimiter = "\t"
                };
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.WriteRecords(etfMarketDatas);
                }
            }
        }
    }
}

