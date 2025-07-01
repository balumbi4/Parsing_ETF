using log4net;
using log4net.Config;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.UI.WebControls;

namespace Parsing
{
    internal class Parser
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Parser));
        public DateTime Str2DateTime(string str)
        {
            try
            {
                DateTime res;
                str = str.Replace("/", " ").Replace(":", " ").Insert(10, " ");
                int[] date = new int[6];
                string segment = "";
                int i = 0;
                foreach (var item in str)
                {
                    segment += item;
                    if (item == ' ')
                    {
                        date[i++] = Convert.ToInt32(segment);
                        segment = "";
                    }
                }
                date[i++] = Convert.ToInt32(segment);
                res = new DateTime(date[2], date[0], date[1], date[3], date[4], date[5]);
                return res;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _log.Error(ex.Message);
                Console.WriteLine("ошибка! подробнее в логе - " + ex.Message);
                return DateTime.MinValue;
            }
        }
        public double? Str2Double(string str)
        {
            if (str == "-")
            {
                return null;
            }
            double res;
            str = str.Replace("%", "").Replace(".", ",");
            res = Convert.ToDouble(str);
            return res;
        }
        public int? Str2Int32(string str)
        {
            if (str == "-")
            {
                return null;
            }
            int res;
            res = Convert.ToInt32(str);
            return res;
        }
        public List<string> InitBid(string str)
        {
            List<string> res = new List<string>();
            if (str == "--")
            {
                res.Add("-");
                res.Add("-");
            }
            else { res = HtmlEntity.DeEntitize(str).Replace(",", "").Replace(".", ",").Insert(7, " ").Split(' ').ToList(); }
            return res;
        }
        public List<string> InitAsk(string str)
        {
            List<string> res = new List<string>();
            if (str == "--")
            {
                res.Add("-");
                res.Add("-");
            }
            else
            {
                foreach (var sim in str)
                {
                    if (sim == ',')
                    {
                        str.Insert(sim + 2, " ");
                    }
                }
                res = HtmlEntity.DeEntitize(str).Replace(",", "").Replace(".", ",").Insert(7, " ").Split(' ').ToList();
            }
            return res;
        }
        public void Out()
        {
            XmlConfigurator.Configure(new FileInfo("../../../loggerConfig.xml"));
            _log.Info("иницилизация необходимых компонентов");
            _log.Info("иницилизация переменных и списка");
            int perPage, totalRecords, totalPage = 0, countRecords = 0;
            string baseUrl = "https://www.wienerborse.at/en/exchange-traded-funds/";
            List<EtfMarketData> etfMarketDatas = new List<EtfMarketData>();

            _log.Info("иницилизация основных элементов HtmlAgilityPack");
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            HtmlDocument page = web.Load(baseUrl);

            _log.Info("задиния значений переменным");
            HtmlNode totalRecordsNode = page.DocumentNode.SelectSingleNode("//*[@id=\"c8001-module\"]/div/div[2]/div[1]/div[1]/div[1]/div/div/b[1]");
            HtmlNode perPageNode = page.DocumentNode.SelectSingleNode("//*[@id=\"c8001-module\"]/div/div[2]/div[1]/div[1]/div[1]/div/div/b[3]");
            perPage = Convert.ToInt32(perPageNode.InnerText);
            totalRecords = Convert.ToInt32(totalRecordsNode.InnerText);
            totalPage = (int)Math.Ceiling((double)totalRecords / perPage);
            _log.Info("иницилизация прошла успешно");
            try
            {
                _log.Info("парсинг страниц");
                for (int pageCount = 1; pageCount <= totalPage; pageCount++)
                {
                    _log.Info($"парсинг {pageCount} страницы");
                    page = web.Load($"{baseUrl}?c8001-page={pageCount}");

                    HtmlNode table = page.DocumentNode.SelectSingleNode("//table[contains(@class, 'table-horizontal')]");

                    foreach (HtmlNode row in table.SelectNodes(".//tr[position()>0]"))
                    {
                        if (countRecords == totalRecords) { break; }
                        var cells = row.SelectNodes(".//td");
                        if (cells != null && cells.Count >= 8)
                        {
                            _log.Info("иницилизация List");
                            List<string> Change = HtmlEntity.DeEntitize(cells[2].InnerText).Replace("%", " ").Replace(".", ",").Split(' ').ToList();
                            _log.Info("иницилизация Change прошла успешно");
                            List<string> Bid = InitBid(cells[5].InnerText);
                            _log.Info("иницилизация Bid прошла успешно");
                            List<string> Ask = InitAsk(cells[6].InnerText);
                            _log.Info("иницилизация Ask прошла успешно");
                            Console.WriteLine($"!!!{++countRecords}!!!");
                            _log.Info("иницилизация etf");
                            EtfMarketData etf = new EtfMarketData
                            {
                                Name = cells[0].InnerText,
                                Last = Convert.ToDouble(HtmlEntity.DeEntitize(cells[1].InnerText).Replace(",", "").Replace(".", ",")),
                                ChangeRercent = Convert.ToDouble(Change.First()),
                                ChangeAbs = Convert.ToDouble(Change.Last()),
                                Date = Str2DateTime(cells[3].InnerText),
                                ISIN = cells[4].InnerText,
                                BidFirst = Str2Double(Bid.First()),
                                BidLast = Str2Int32(Bid.Last()),
                                AskFirst = Str2Double(Ask.First()),
                                AskLast = Str2Int32(Ask.Last()),
                                Total = Str2Int32(cells[7].InnerText.Replace(",", "")),
                                Status = Convert.ToChar(cells[8].InnerText)
                            };
                            _log.Info("иницилизация etf прошла успешно");
                            etfMarketDatas.Add(etf);
                            Console.WriteLine($"{etf.Name}  {etf.Last}  {etf.ChangeRercent}  {etf.ChangeAbs} {etf.Date} {etf.ISIN} {etf.BidFirst} {etf.BidLast} {etf.AskFirst} {etf.AskLast} {etf.Total} {etf.Status}");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                Console.WriteLine("ошибка!! подробнее в логе - " + ex.Message);
                return;
            }
            SaveToCsv saveToCsv = new SaveToCsv();
            saveToCsv.Save(etfMarketDatas);
        }
    }
}

