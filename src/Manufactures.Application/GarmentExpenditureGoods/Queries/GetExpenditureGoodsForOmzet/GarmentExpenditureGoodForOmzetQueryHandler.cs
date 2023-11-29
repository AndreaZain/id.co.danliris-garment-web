using ExtCore.Data.Abstractions;
using Manufactures.Domain.GarmentExpenditureGoods.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Domain.Queries;

namespace Manufactures.Application.GarmentExpenditureGoods.Queries.GetExpenditureGoodsForOmzet
{
    public class GarmentExpenditureGoodForOmzetQueryHandler : IQueryHandler<GetExpenditureGoodsForOmzetQuery, GarmentExpenditureGoodForOmzetListViewModel>
    {
        private readonly IStorage _storage;
        private readonly IGarmentExpenditureGoodRepository garmentExpenditureGoodRepository;
        private readonly IGarmentExpenditureGoodItemRepository garmentExpenditureGoodItemRepository;

        public GarmentExpenditureGoodForOmzetQueryHandler(IStorage storage, IServiceProvider serviceProvider)
        {
            _storage = storage;
            
            garmentExpenditureGoodRepository = storage.GetRepository<IGarmentExpenditureGoodRepository>();
            garmentExpenditureGoodItemRepository = storage.GetRepository<IGarmentExpenditureGoodItemRepository>();
        }

        public async Task<GarmentExpenditureGoodForOmzetListViewModel> Handle(GetExpenditureGoodsForOmzetQuery request, CancellationToken cancellationToken)
        {
            GarmentExpenditureGoodForOmzetListViewModel expenditureGoodListViewModel = new GarmentExpenditureGoodForOmzetListViewModel();
    
            //DateTimeOffset dateFrom = new DateTimeOffset(request.dateFrom, new TimeSpan(7, 0, 0));
            //DateTimeOffset dateTo = new DateTimeOffset(request.dateTo, new TimeSpan(7, 0, 0));

            var offset = 7;

            var factexpend = (from a in (from aa in garmentExpenditureGoodRepository.Query
                                        where aa.ExpenditureDate.AddHours(offset).Date >= request.dateFrom && aa.ExpenditureDate.AddHours(offset).Date <= request.dateTo && aa.ExpenditureType == "EXPORT"
                                         select aa)
                             join b in garmentExpenditureGoodItemRepository.Query on a.Identity equals b.ExpenditureGoodId

                             group new { Qty = b.Quantity, Prc = b.BasicPrice } by new
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

                             select new GarmentExpenditureGoodForOmzetDto
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
