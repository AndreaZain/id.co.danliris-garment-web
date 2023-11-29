using ExtCore.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Domain.Queries;
using Manufactures.Domain.GarmentSample.SampleExpenditureGoods.Repositories;

namespace Manufactures.Application.GarmentExpenditureGoods.Queries.GetSampleExpenditureGoodsForOmzet
{
    public class GarmentSampleExpenditureGoodForOmzetQueryHandler : IQueryHandler<GetSampleExpenditureGoodsForOmzetQuery, GarmentSampleExpenditureGoodForOmzetListViewModel>
    {
        private readonly IStorage _storage;
        private readonly IGarmentSampleExpenditureGoodRepository garmentExpenditureGoodRepository;
        private readonly IGarmentSampleExpenditureGoodItemRepository garmentExpenditureGoodItemRepository;
 
        public GarmentSampleExpenditureGoodForOmzetQueryHandler(IStorage storage, IServiceProvider serviceProvider)
        {
            _storage = storage;
            
            garmentExpenditureGoodRepository = storage.GetRepository<IGarmentSampleExpenditureGoodRepository>();
            garmentExpenditureGoodItemRepository = storage.GetRepository<IGarmentSampleExpenditureGoodItemRepository>();
        }

        public async Task<GarmentSampleExpenditureGoodForOmzetListViewModel> Handle(GetSampleExpenditureGoodsForOmzetQuery request, CancellationToken cancellationToken)
        {
            GarmentSampleExpenditureGoodForOmzetListViewModel expenditureGoodListViewModel = new GarmentSampleExpenditureGoodForOmzetListViewModel();
    
            //DateTimeOffset dateFrom = new DateTimeOffset(request.dateFrom, new TimeSpan(7, 0, 0));
            //DateTimeOffset dateTo = new DateTimeOffset(request.dateTo, new TimeSpan(7, 0, 0));

            var offset = 7;

            var factexpend = (from a in (from aa in garmentExpenditureGoodRepository.Query
                                        where aa.ExpenditureDate.AddHours(offset).Date >= request.dateFrom && aa.ExpenditureDate.AddHours(offset).Date <= request.dateTo && aa.ExpenditureType == "EXPORT"
                                         select aa)
                             join b in garmentExpenditureGoodItemRepository.Query on a.Identity equals b.ExpenditureGoodId

                             group new { Qty = b.Quantity, Prc = b.Price } by new
                             {
                                 a.PackingListId,
                                 a.ExpenditureGoodNo,
                                 a.Invoice,
                                 a.RONo,
                                 a.Article,
                                 a.BuyerCode,
                                 a.BuyerName,
                                 a.ComodityCode,
                                 a.ComodityName,
                                 a.UnitCode,
                             } into G

                             select new GarmentSampleExpenditureGoodForOmzetDto
                             {
                                 PackingListId = G.Key.PackingListId,
                                 ExpenditureGoodNo = G.Key.ExpenditureGoodNo,
                                 InvoiceNo = G.Key.Invoice,
                                 RONumber = G.Key.RONo,
                                 Article = G.Key.Article,
                                 UnitCode = G.Key.UnitCode,
                                 BuyerName = G.Key.BuyerCode + " - " + G.Key.BuyerName,
                                 ComodityName = G.Key.ComodityCode + " - " + G.Key.ComodityName.TrimEnd(),
                                 Quantity = Math.Round(G.Sum(m => m.Qty), 0),
                                 Price = Math.Round(G.Average(m => m.Prc), 4),
                             });

            expenditureGoodListViewModel.garmentReports = factexpend.OrderBy(x => x.ExpenditureGoodNo).ThenBy(w => w.RONumber).ThenBy(w => w.InvoiceNo).ToList();

            return expenditureGoodListViewModel;
        }
    }
}
