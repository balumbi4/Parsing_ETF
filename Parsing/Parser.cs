using log4net;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parsing
{
    internal class Parser
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Parser));
        private List<string> InitData(string str)
        {
            List<string> res = new List<string>();
            if (str == "--")
            {
                res.Add("-");
                res.Add("-");
            }
            else
            {
                str = HtmlEntity.DeEntitize(str).Replace("%", " ").Replace(",", "");
                int index = str.IndexOf(".");
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
                _log.Info("иницилизания переменных");
                int perPage, totalRecords, totalPage = 0, countRecords = 0;
                string baseUrl = "https://www.wienerborse.at/en/exchange-traded-funds/";
                string tableXPath = "//table[contains(@class, 'table-horizontal')]";
                string rowXPath = ".//tr[position()>0]";
                List<EtfData> etfMarketDatas = new List<EtfData>();
                HtmlWeb web = new HtmlWeb();
                web.OverrideEncoding = Encoding.UTF8;
                HtmlDocument page = web.Load(baseUrl) ?? throw new Exception($"Ошибка. Не удалось загрузить страницу по адресу: {baseUrl}");
                HtmlNode totalRecordsNode = page.DocumentNode.SelectSingleNode("//*[@id=\"c8001-module\"]/div/div[2]/div[1]/div[1]/div[1]/div/div/b[1]");
                HtmlNode perPageNode = page.DocumentNode.SelectSingleNode("//*[@id=\"c8001-module\"]/div/div[2]/div[1]/div[1]/div[1]/div/div/b[3]");
                perPage = Convert.ToInt32(perPageNode.InnerText);
                totalRecords = Convert.ToInt32(totalRecordsNode.InnerText);
                totalPage = (int)Math.Ceiling((double)totalRecords / perPage);
                _log.Info("иницилизация прошла успешно");
                _log.Info("парсинг страниц");
                for (int pageCount = 1; pageCount <= totalPage; pageCount++)
                {
                    _log.Info($"парсинг {pageCount} страницы по адресу: {baseUrl}?c8001-page={pageCount}");
                    page = web.Load($"{baseUrl}?c8001-page={pageCount}") ?? throw new Exception($"Ошибка. Не удалось загрузить страницу по адресу: {baseUrl}?c8001-page={pageCount}");
                    _log.Info($"успешная загрузка {pageCount} страницы");
                    _log.Info($"загрузка таблицы с данными");
                    HtmlNode table = page.DocumentNode.SelectSingleNode(tableXPath) ?? throw new Exception($"Ошибка. Не удалось загрузить таблицу:");
                    _log.Info($"успешная загрузка таблицы с данными");
                    foreach (HtmlNode row in table.SelectNodes(rowXPath))
                    {
                        if (countRecords == totalRecords) { break; }
                        var cells = row.SelectNodes(".//td");
                        if (cells != null && cells.Count >= 8)
                        {
                            _log.Info($"иницилизация {++countRecords} записи из {totalRecords} записей");
                            Console.WriteLine($"!!!{countRecords}!!!");
                            try
                            {
                                EtfData etf = new EtfData
                                {
                                    Name = cells[0].InnerText,
                                    Last = cells[1].InnerText,
                                    ChangePercent = InitData(cells[2].InnerText).First(),
                                    ChangeAbs = InitData(cells[2].InnerText).Last(),
                                    Date = cells[3].InnerText,
                                    ISIN = cells[4].InnerText,
                                    Bid = InitData(cells[5].InnerText).First(),
                                    BidVolume = InitData(cells[5].InnerText).Last(),
                                    Ask = InitData(cells[6].InnerText).First(),
                                    AskVolume = InitData(cells[6].InnerText).Last(),
                                    Total = cells[7].InnerText,
                                    Status = cells[8].InnerText
                                };
                                _log.Info($"иницилизация {countRecords} записи из {totalRecords} записей прошла успешно");
                                etfMarketDatas.Add(etf);
                                Console.WriteLine($"{etf.Name}  {etf.Last}  {etf.ChangePercent}  {etf.ChangeAbs} {etf.Date} {etf.ISIN} {etf.Bid} {etf.BidVolume} {etf.Ask} {etf.AskVolume} {etf.Total} {etf.Status}");
                            }
                            catch (Exception ex)
                            {
                                _log.Error($"ошибка. не удалось иницилизировать {countRecords} запись. причина: {ex.Message}");
                            }

                        }
                        else if (cells != null) { throw new Exception("Ошибка. Количество столбцов в таблице меньше 8"); }
                    }
                }
                _log.Info($"{countRecords} записей из {totalRecords} записей успешно спарсились");
                SaveToCsv saveToCsv = new SaveToCsv();
                saveToCsv.Save(etfMarketDatas);
            }
            catch (System.Net.WebException ex)
            {
                _log.Error($"ошибка загрузки страницы: {ex.Message}");
                Console.WriteLine($"ошибка {ex.GetType().Name} подробнее в логе. Текст ошибки - {ex.Message}");
                return;
            }
            catch (System.Xml.XPath.XPathException ex)
            {
                _log.Error($"ошибка, неверный XPath: {ex.Message}");
                Console.WriteLine($"ошибка {ex.GetType().Name} подробнее в логе. Текст ошибки - {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                Console.WriteLine($"ошибка!! {ex} подробнее в логе - {ex.Message}");
                return;
            }
        }
    }
}

