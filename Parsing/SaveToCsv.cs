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
        public void Save(List<EtfData> etfMarketDatas)
        {
            string pathFile = $"{DateTime.Now.ToString().Remove(0, 11).Replace(":", "_")}" + ".tsv";
            string pathDir = DateTime.Now.ToString().Remove(10).Replace(".", "_");
            if (!Directory.Exists(pathDir))
            {
                Directory.CreateDirectory(pathDir);
            }
            string pathCsv = Path.Combine(Directory.GetCurrentDirectory(), pathDir, pathFile);
            if (!File.Exists(pathCsv))
            {
                File.Create(pathCsv).Close();
            }
            using (var writer = new StreamWriter(pathCsv))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Encoding = new UTF8Encoding(true),
                    Delimiter = "\t",
                    HasHeaderRecord = true,
                    //Mode = CsvMode.RFC4180, // стандартный режим CSV
                };
                using (var csv = new CsvWriter(writer, csvConfig))
                {
                    csv.Context.RegisterClassMap<EtfMap>();
                    csv.WriteRecords(etfMarketDatas);
                }
            }
        }
    }
}

/*    if (!Directory.Exists(csvFilePathStocks))
      {
        Directory.CreateDirectory(csvFilePathStocks);
      }
      string fileName = $"Stock_{dateGetParsing}.csv";
      csvFilePathStocks = Path.Combine(csvFilePathStocks, fileName);
      if (!File.Exists(csvFilePathStocks))
      {
        File.Create(csvFilePathStocks).Close();
      }
      if (!Directory.Exists(csvFilePathBonds))
      {
        Directory.CreateDirectory(csvFilePathBonds);
      }
      string _fileName = $"Bond_{dateGetParsing}.csv";
      csvFilePathBonds = Path.Combine(csvFilePathBonds, _fileName);
      if (!File.Exists(csvFilePathBonds))
      {
        File.Create(csvFilePathBonds).Close();
      }*/