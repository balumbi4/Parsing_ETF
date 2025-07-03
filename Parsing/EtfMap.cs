using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing
{
    internal class EtfMap : ClassMap<EtfData>
    {
        public EtfMap()
        {
            Map(m => m.Name)
                .Name("Name");
            Map(m => m.Last)
                .Name("Last")
                .Convert(args => args.Value.Last?.ToString() ?? "-");
            Map(m => m.ChangePercent)
                .Name("ChangePercent")
                .Convert(args => args.Value.ChangePercent?.ToString() ?? "-");
            Map(m => m.ChangeAbs)
                .Name("ChangeAbs")
                .Convert(args => args.Value.ChangeAbs?.ToString() ?? "-");
            Map(m => m.Date)
                .Name("Date");
            Map(m => m.ISIN)
                .Name("ISIN");
            Map(m => m.BidFirst)
                .Name("BidFirst")
                .Convert(args => args.Value.BidFirst?.ToString() ?? "-");
            Map(m => m.BidLast)
                .Name("BidLast")
                .Convert(args => args.Value.BidLast?.ToString() ?? "-");
            Map(m => m.AskFirst)
                .Name("AskFirst")
                .Convert(args => args.Value.AskFirst?.ToString() ?? "-");
            Map(m => m.AskLast)
                .Name("AskLast")
                .Convert(args => args.Value.AskLast?.ToString() ?? "-");
            Map(m => m.Total)
                .Name("Total")
                .Convert(args => args.Value.Total?.ToString() ?? "-");
            Map(m => m.Status)
                .Name("Status");
        }
    }
}