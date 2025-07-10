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
        private EtfMap()
        {
            Map(m => m.Name)
                .Name("Name");
            Map(m => m.Last)
                .Name("Last")
                .Convert(args => args.Value.Last?.ToString() ?? "-");
            Map(m => m.ChangePercent)
                .Name("Chg. % 1D")
                .Convert(args => args.Value.ChangePercent?.ToString() ?? "-");
            Map(m => m.ChangeAbs)
                .Name("Chg. Abs.")
                .Convert(args => args.Value.ChangeAbs?.ToString() ?? "-");
            Map(m => m.Date)
                .Name("Date Time")
                .Convert(args => args.Value.Date?.ToString() ?? "-");
            Map(m => m.ISIN)
                .Name("ISIN");
            Map(m => m.Bid)
                .Name("Bid")
                .Convert(args => args.Value.Bid?.ToString() ?? "-");
            Map(m => m.BidVolume)
                .Name("Bid volume")
                .Convert(args => args.Value.BidVolume?.ToString() ?? "-");
            Map(m => m.Ask)
                .Name("Ask")
                .Convert(args => args.Value.Ask?.ToString() ?? "-");
            Map(m => m.AskVolume)
                .Name("Ask volume")
                .Convert(args => args.Value.AskVolume?.ToString() ?? "-");
            Map(m => m.Total)
                .Name("Total volume")
                .Convert(args => args.Value.Total?.ToString() ?? "-");
            Map(m => m.Status)
                .Name("Status");
        }
    }
}