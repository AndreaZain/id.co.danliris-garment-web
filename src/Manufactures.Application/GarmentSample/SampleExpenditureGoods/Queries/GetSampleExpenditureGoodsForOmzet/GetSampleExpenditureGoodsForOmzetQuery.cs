using Infrastructure.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Application.GarmentExpenditureGoods.Queries.GetSampleExpenditureGoodsForOmzet
{
    public class GetSampleExpenditureGoodsForOmzetQuery : IQuery<GarmentSampleExpenditureGoodForOmzetListViewModel>
    {
        public string token { get; private set; }
        public string unitcode { get; private set; }
        public int offset { get; private set; }
        public DateTime dateFrom { get; private set; }
        public DateTime dateTo { get; private set; }

        public GetSampleExpenditureGoodsForOmzetQuery(DateTime dateFrom, DateTime dateTo, string unitcode, int offset, string token)
        {
            this.unitcode = unitcode;
            this.offset = offset;
            this.dateFrom = dateFrom;
            this.dateTo = dateTo;
            this.token = token;
        }
    }
}
