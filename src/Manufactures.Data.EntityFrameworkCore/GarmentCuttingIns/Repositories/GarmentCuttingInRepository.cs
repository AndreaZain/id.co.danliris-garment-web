﻿using Infrastructure.Data.EntityFrameworkCore;
using Infrastructure.Data.EntityFrameworkCore.Utilities;
using Manufactures.Domain.GarmentCuttingIns;
using Manufactures.Domain.GarmentCuttingIns.ReadModels;
using Manufactures.Domain.GarmentCuttingIns.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufactures.Data.EntityFrameworkCore.GarmentCuttingIns.Repositories
{
    public class GarmentCuttingInRepository : AggregateRepostory<GarmentCuttingIn, GarmentCuttingInReadModel>, IGarmentCuttingInRepository
    {
        public IQueryable<GarmentCuttingInReadModel> Read(int page, int size, string order, string keyword, string filter)
        {
            var data = Query;

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            data = QueryHelper<GarmentCuttingInReadModel>.Filter(data, FilterDictionary);

            List<string> SearchAttributes = new List<string>
            {
                "CutInNo",
                "CuttingType",
                "Article",
                "RONo",
                "UnitCode",
                "UnitName",
            };
            data = QueryHelper<GarmentCuttingInReadModel>.Search(data, SearchAttributes, keyword);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            data = OrderDictionary.Count == 0 ? data.OrderByDescending(o => o.ModifiedDate) : QueryHelper<GarmentCuttingInReadModel>.Order(data, OrderDictionary);

            data = data.Skip((page - 1) * size).Take(size);

            return data;
        }

        protected override GarmentCuttingIn Map(GarmentCuttingInReadModel readModel)
        {
            return new GarmentCuttingIn(readModel);
        }
    }
}
