using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Application.GarmentExpenditureGoods.Queries.GetSampleExpenditureGoodsForOmzet
{
    public class GarmentSampleExpenditureGoodForOmzetDto
    {
        public GarmentSampleExpenditureGoodForOmzetDto()
        {
        }
        public int PackingListId { get; set; }
        public string ExpenditureGoodNo { get; set; }
        public string RONumber { get; set; }
        public string Article { get; set; }
        public string UnitCode { get; set; }
        public string BuyerName { get; set; }
        public string InvoiceNo { get; set; }
        public string ComodityName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }


        public GarmentSampleExpenditureGoodForOmzetDto(GarmentSampleExpenditureGoodForOmzetDto getReportExpenditure)
        {

            PackingListId = getReportExpenditure.PackingListId;
            ExpenditureGoodNo = getReportExpenditure.ExpenditureGoodNo;
            RONumber = getReportExpenditure.RONumber;
            Article = getReportExpenditure.Article;
            UnitCode = getReportExpenditure.UnitCode;
            BuyerName = getReportExpenditure.BuyerName;
            InvoiceNo = getReportExpenditure.InvoiceNo;
            ComodityName = getReportExpenditure.ComodityName;
            Quantity = getReportExpenditure.Quantity;
            Price = getReportExpenditure.Price;
        }

    }
}
