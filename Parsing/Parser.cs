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
        /*25.3401,182*/
        public List<string> InitBid(string str)
        {
            List<string> res = new List<string>();
            if (str == "--")
            {
                res.Add("-");
                res.Add("-");
            }
            else
            {
                str = HtmlEntity.DeEntitize(str).Replace(",", "").Replace(".", ",");
                int index = str.IndexOf(",");
                if (index != -1)
                {
                    str = str.Insert(index + 4, " ");
                }
                res = str.Split(' ').ToList();
            }
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
                str = HtmlEntity.DeEntitize(str).Replace(",", "").Replace(".", ",");
                int index = str.IndexOf(",");
                if (index != -1)
                {
                    str = str.Insert(index + 4, " ");
                }
                res = str.Split(' ').ToList();
            }
            return res;
        }
        public void Parsing()
        {
            try
            {
                XmlConfigurator.Configure(new FileInfo("../../../loggerConfig.xml"));
                _log.Info("иницилизация необходимых компонентов");
                _log.Info("иницилизация переменных perPage totalRecords totalPage countRecords baseUrl и списка etfMarketDatas");
                int perPage, totalRecords, totalPage = 0, countRecords = 0;
                string baseUrl = "https://www.wienerborse.at/en/exchange-traded-funds/";
                string tableXPath = "//table[contains(@class, 'table-horizontal')]";
                string rowXPath = ".//tr[position()>0]";
                List<EtfMarketData> etfMarketDatas = new List<EtfMarketData>();

                /*public string Name;
        public double Last;
        public double ChangeRercent;
        public double ChangeAbs;
        public DateTime Date;
        public string ISIN;
        public double? BidFirst;
        public int? BidLast;
        public double? AskFirst;
        public int? AskLast;
        public int? Total;
        public char Status; */
                EtfMarketData e = new EtfMarketData()
                {
                    Name = "Primer",
                    Last = 265,
                    ChangeRercent = 0.54,
                    ChangeAbs = 24,
                    Date = DateTime.Now,
                    ISIN = "GEWGEGW",
                    BidFirst = 15.84,
                    BidLast = 15,
                    AskFirst = 15.48,
                    AskLast = 16,
                    Total = 45000,
                    Status = 'C'
                };
                etfMarketDatas.Add(e);
                SaveToCsv saveToCsv = new SaveToCsv();
                saveToCsv.Save(etfMarketDatas);


                _log.Info("иницилизация основных элементов HtmlAgilityPack");
                _log.Info("иницилизация HtmlWeb");
                HtmlWeb web = new HtmlWeb();
                web.OverrideEncoding = Encoding.UTF8;
                _log.Info("иницилизация HtmlDocument");
                HtmlDocument page = web.Load(baseUrl);
                if (page == null)
                {
                    _log.Error($"Ошибка. Не удалось загрузить страницу: {baseUrl}");
                    throw new Exception($"Ошибка. Не удалось загрузить страницу: {baseUrl}");
                }
                _log.Info("задание значений переменным totalRecordsNode perPageNode perPage totalRecords totalPage");
                _log.Info("задание значения totalRecordsNode");
                HtmlNode totalRecordsNode = page.DocumentNode.SelectSingleNode("//*[@id=\"c8001-module\"]/div/div[2]/div[1]/div[1]/div[1]/div/div/b[1]");
                _log.Info("задание значения totalRecordsNode прошла успешно");

                _log.Info("задание значения perPageNode");
                HtmlNode perPageNode = page.DocumentNode.SelectSingleNode("//*[@id=\"c8001-module\"]/div/div[2]/div[1]/div[1]/div[1]/div/div/b[3]");
                _log.Info("задание значения perPageNode прошла успешно");

                _log.Info("задание значения perPage");
                perPage = Convert.ToInt32(perPageNode.InnerText);
                _log.Info("задание значения perPage прошла успешно");

                _log.Info("задание значения totalRecords");
                totalRecords = Convert.ToInt32(totalRecordsNode.InnerText);
                _log.Info("задание значения totalRecords прошла успешно");

                _log.Info("задание значения totalPage");
                totalPage = (int)Math.Ceiling((double)totalRecords / perPage);
                _log.Info("задание значения totalPage прошла успешно");

                _log.Info("иницилизация прошла успешно");

                _log.Info("парсинг страниц");
                for (int pageCount = 1; pageCount <= totalPage; pageCount++)
                {
                    _log.Info($"парсинг {pageCount} страницы");
                    _log.Info($"загрузка {pageCount} страницы");
                    page = web.Load($"{baseUrl}?c8001-page={pageCount}");
                    if (page == null)
                    {
                        _log.Error($"Ошибка. Не удалось загрузить страницу: {baseUrl}?c8001-page={pageCount}");

                        throw new Exception($"Ошибка. Не удалось загрузить страницу: {baseUrl}?c8001-page={pageCount}");
                    }
                    _log.Info($"успешная загрузка {pageCount} страницы");
                    _log.Info($"загрузка таблицы с данными");
                    HtmlNode table = page.DocumentNode.SelectSingleNode(tableXPath);
                    if (page == null)
                    {
                        _log.Error($"Ошибка. Не удалось загрузить таблицу:");

                        throw new Exception($"Ошибка. Не удалось загрузить таблицу:");
                    }
                    _log.Info($"успешная загрузка таблицы с данными");
                    foreach (HtmlNode row in table.SelectNodes(rowXPath))
                    {
                        if (countRecords == totalRecords) { break; }
                        var cells = row.SelectNodes(".//td");
                        if (cells != null && cells.Count >= 8)
                        {
                            _log.Info("иницилизация вспомогательных списков Change Bid Ask");
                            List<string> Change = HtmlEntity.DeEntitize(cells[2].InnerText).Replace("%", " ").Replace(".", ",").Split(' ').ToList();
                            _log.Info("иницилизация Change прошла успешно");
                            List<string> Bid = InitBid(cells[5].InnerText);
                            _log.Info("иницилизация Bid прошла успешно");
                            List<string> Ask = InitAsk(cells[6].InnerText);
                            _log.Info("иницилизация Ask прошла успешно");
                            _log.Info($"иницилизация {++countRecords} записи");
                            Console.WriteLine($"!!!{countRecords}!!!");
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
                            _log.Info($"иницилизация {countRecords} записи прошла успешно");
                            etfMarketDatas.Add(etf);
                            Console.WriteLine($"{etf.Name}  {etf.Last}  {etf.ChangeRercent}  {etf.ChangeAbs} {etf.Date} {etf.ISIN} {etf.BidFirst} {etf.BidLast} {etf.AskFirst} {etf.AskLast} {etf.Total} {etf.Status}");
                        }

                    }
                }
                //SaveToCsv saveToCsv = new SaveToCsv();
                //saveToCsv.Save(etfMarketDatas);
            }
            catch (System.Net.WebException ex)
            {
                _log.Error($"ошибка загрузки страницы: {ex.GetType().Name} {ex.Message}");
                Console.WriteLine($"ошибка {ex.GetType().Name} подробнее в логе. Текст ошибки - {ex.Message}");
                return;
            }
            catch (System.Xml.XPath.XPathException ex)
            {
                _log.Error($"ошибка, неверный XPath: {ex.GetType().Name} {ex.Message}");
                Console.WriteLine($"ошибка {ex.GetType().Name} подробнее в логе. Текст ошибки - {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                _log.Error(ex + ex.Message);
                Console.WriteLine($"ошибка!! {ex} подробнее в логе - {ex.Message}");
                return;
            }
        }
    }
}

