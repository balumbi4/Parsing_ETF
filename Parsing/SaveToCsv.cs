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
            string pathFile = $"{DateTime.Now.ToString().Remove(0, 11).Replace(":", "_")}" + ".csv";
            string pathDir = "CSV";
            if (!Directory.Exists(pathDir))
            {
                Directory.CreateDirectory(pathDir);
            }
            pathDir = Path.Combine(Directory.GetCurrentDirectory(), pathDir, DateTime.Now.ToString().Remove(10).Replace(".", "_"));
            if (!Directory.Exists(pathDir))
            {
                Directory.CreateDirectory(pathDir);
            }
            string pathCsv = Path.Combine(Directory.GetCurrentDirectory() , pathDir, pathFile);
            
            if (!File.Exists(pathCsv))
            {
                File.Create(pathCsv).Close();
            }
            _log.Info($"запись данных в файл {pathCsv}");
            try
            {
                using (var writer = new StreamWriter(pathCsv))
                {
                    var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Encoding = new UTF8Encoding(true),
                        Delimiter = "\t",
                        HasHeaderRecord = true,
                    };
                    using (var csv = new CsvWriter(writer, csvConfig))
                    {
                        csv.Context.RegisterClassMap<EtfMap>();
                        csv.WriteRecords(etfMarketDatas);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"ошибка при записи. текст ошибки: {ex.Message}");
            }
            _log.Info($"все данные записаны успешно");
        }
    }
}