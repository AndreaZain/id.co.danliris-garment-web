﻿// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExtCore.Data.Abstractions;
using Infrastructure;
using Infrastructure.External.DanLirisClient.Microservice;
using Infrastructure.External.DanLirisClient.Microservice.HttpClientService;
using Infrastructure.External.DanLirisClient.Microservice.MasterResult;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Barebone.Controllers
{
    public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.Controller
    {
        protected IStorage Storage { get; private set; }

        public ControllerBase(IStorage storage)
        {
            this.Storage = storage;
        }
    }

    [ApiController]
    public abstract class ControllerApiBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        protected IStorage Storage { get; }
        protected IWebApiContext WorkContext { get; }
        protected IMediator Mediator { get; }
        protected readonly string GARMENT_POLICY = "GarmentPolicy";
        protected readonly IHttpClientService _http;
        //protected readonly string Token;

        public ControllerApiBase(IServiceProvider serviceProvider)
        {
            _http = serviceProvider.GetService<IHttpClientService>();
            this.Storage = serviceProvider.GetService<IStorage>();
            this.WorkContext = serviceProvider.GetService<IWebApiContext>();
            this.Mediator = serviceProvider.GetService<IMediator>();
            //Token = GetTokenAsync();
        }

        //protected string GetTokenAsync()
        //{

        //    var response = _http.PostAsync(PurchasingDataSettings.TokenEndpoint,
        //        new StringContent(JsonConvert.SerializeObject(new { username = PurchasingDataSettings.Username, password = PurchasingDataSettings.Password }), Encoding.UTF8, "application/json")).Result;
        //    TokenResult tokenResult = new TokenResult();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        tokenResult = JsonConvert.DeserializeObject<TokenResult>(response.Content.ReadAsStringAsync().Result);

        //    }
        //    else
        //    {
        //        GetTokenAsync();
        //    }
        //    return tokenResult.data;
        //}

        protected SingleProductResult GetProduct(int id, string token)
        {
            var masterProductUri = MasterDataSettings.Endpoint + $"master/products/{id}";
            var productResponse = _http.GetAsync(masterProductUri).Result;

            if (productResponse.IsSuccessStatusCode)
            {
                SingleProductResult productResult = JsonConvert.DeserializeObject<SingleProductResult>(productResponse.Content.ReadAsStringAsync().Result);
                return productResult;
            }
            else
            {
                return new SingleProductResult();
            }
        }

        protected SingleUnitResult GetUnit(int id, string token)
        {
            try
            {
                var masterUnitUri = MasterDataSettings.Endpoint + $"master/units/{id}";
                var unitResponse = _http.GetAsync(masterUnitUri).Result;
                if (unitResponse.IsSuccessStatusCode)
                {
                    SingleUnitResult unitResult = JsonConvert.DeserializeObject<SingleUnitResult>(unitResponse.Content.ReadAsStringAsync().Result);
                    return unitResult;
                }
                else
                {
                    return new SingleUnitResult();
                }
            }
            catch (Exception ex)
            {
                SingleUnitResult unitResult = new SingleUnitResult();
                unitResult.data.Name = MasterDataSettings.Endpoint;
                throw new Exception(MasterDataSettings.Endpoint);
            }
            
        }

        //protected SingleMaterialTypeResult GetMaterialType(string id, string token)
        //{
        //    var materialTypeUri = WeavingDataSettings.Endpoint + $"weaving/material-types/{id}";
        //    var materialTypeResponse = _http.GetAsync(materialTypeUri, token).Result;

        //    if (materialTypeResponse.IsSuccessStatusCode)
        //    {
        //        SingleMaterialTypeResult materialTypeResult = JsonConvert.DeserializeObject<SingleMaterialTypeResult>(materialTypeResponse.Content.ReadAsStringAsync().Result);
        //        return materialTypeResult;
        //    }
        //    else
        //    {
        //        return new SingleMaterialTypeResult();
        //    }

        //}

        //protected SingleMachineSpinningResult GetMachine(int id, string token)
        //{
        //    var masterMachineSpinningUri = MasterDataSettings.Endpoint + $"machine-spinnings/{id}";
        //    var machineSpinningResponse = _http.GetAsync(masterMachineSpinningUri, Token).Result;

        //    if (machineSpinningResponse.IsSuccessStatusCode)
        //    {
        //        SingleMachineSpinningResult machineSpinningResult = JsonConvert.DeserializeObject<SingleMachineSpinningResult>(machineSpinningResponse.Content.ReadAsStringAsync().Result);
        //        return machineSpinningResult;
        //    }
        //    else
        //    {
        //        return new SingleMachineSpinningResult();
        //    }

        //}

        protected SingleUomResult GetUom(int id, string token)
        {
            var masterUomUri = MasterDataSettings.Endpoint + $"master/uoms/{id}";
            var masterUomResponse = _http.GetAsync(masterUomUri, token).Result;

            if (masterUomResponse.IsSuccessStatusCode)
            {
                SingleUomResult uomResult = JsonConvert.DeserializeObject<SingleUomResult>(masterUomResponse.Content.ReadAsStringAsync().Result);
                return uomResult;
            }
            else
            {
                return new SingleUomResult();
            }

        }

        protected ProductResult GetProducts(string keyword = null)
        {
            var masterProductUri = MasterDataSettings.Endpoint + $"master/products?size={int.MaxValue}&keyword={keyword}";
            //var masterUnitUri = $"https://com-danliris-service-core-dev.azurewebsites.net/v1/master/products/simple";
            var productResponse = _http.GetAsync(masterProductUri).Result;

            var productResult = new ProductResult();
            if (productResponse.IsSuccessStatusCode)
            {
                productResult = JsonConvert.DeserializeObject<ProductResult>(productResponse.Content.ReadAsStringAsync().Result);
            }
            else
            {
                GetProducts(keyword);
            }
            return productResult;
        }

        protected UnitResult GetUnitDepartments(string keyword = null)
        {
            var masterUnitUri = MasterDataSettings.Endpoint + $"master/units?size={int.MaxValue}" + "&filter={}" + $"&keyword={keyword}";
            //var masterUnitUri = $"https://com-danliris-service-core-dev.azurewebsites.net/v1/master/units/simple";
            var unitResponse = _http.GetAsync(masterUnitUri).Result;

            var unitResult = new UnitResult();
            if (unitResponse.IsSuccessStatusCode)
            {
                unitResult = JsonConvert.DeserializeObject<UnitResult>(unitResponse.Content.ReadAsStringAsync().Result);
            }
            else
            {
                GetUnitDepartments(keyword);
            }
            return unitResult;
        }

        //protected MaterialTypeResult GetMaterialTypes(string token, string keyword = null)
        //{
        //    var materialTypeUri = WeavingDataSettings.Endpoint + $"weaving/material-types?size={int.MaxValue}&keyword={keyword}";
        //    var materialTypeResponse = _http.GetAsync(materialTypeUri, token).Result;

        //    var materialTypeResult = new MaterialTypeResult();
        //    if (materialTypeResponse.IsSuccessStatusCode)
        //    {
        //        materialTypeResult = JsonConvert.DeserializeObject<MaterialTypeResult>(materialTypeResponse.Content.ReadAsStringAsync().Result);
        //    }
        //    else
        //    {
        //        GetMaterialTypes(keyword, token);
        //    }

        //    return materialTypeResult;
        //}

        //protected MachineSpinningResult GetMachineSpinnings(string token, string keyword = null)
        //{
        //    var masterMachineSpinningUri = MasterDataSettings.Endpoint + $"machine-spinnings?size={int.MaxValue}&keyword={keyword}";
        //    //var masterUnitUri = $"https://com-danliris-service-core-dev.azurewebsites.net/v1/master/units/simple";
        //    var machineSpinningResponse = _http.GetAsync(masterMachineSpinningUri, Token).Result;

        //    var machineSpinningResult = new MachineSpinningResult();
        //    if (machineSpinningResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
        //    {
        //        machineSpinningResult = JsonConvert.DeserializeObject<MachineSpinningResult>(machineSpinningResponse.Content.ReadAsStringAsync().Result);
        //    }
        //    else
        //    {
        //        GetMachineSpinnings(keyword, token);
        //    }

        //    return machineSpinningResult;
        //}

        protected UomResult GetUoms(string token, string keyword = null)
        {
            var masterUomUri = MasterDataSettings.Endpoint + $"master/uoms?size{int.MaxValue}&keyword={keyword}";
            //var masterUnitUri = $"https://com-danliris-service-core-dev.azurewebsites.net/v1/master/units/simple";
            var uomResponse = _http.GetAsync(masterUomUri, token).Result;

            var uomResult = new UomResult();
            if (uomResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                uomResult = JsonConvert.DeserializeObject<UomResult>(uomResponse.Content.ReadAsStringAsync().Result);
            }
            else
            {
                GetUoms(keyword, token);
            }
            return uomResult;
        }

        protected async Task<string> PutGarmentUnitExpenditureNoteCreate(int id)
        {
            var garmentUnitExpenditureNoteUri = PurchasingDataSettings.Endpoint + $"garment-unit-expenditure-notes/isPreparingTrue/{id}";
            var garmentUnitExpenditureNoteResponse = await _http.PutAsync(garmentUnitExpenditureNoteUri, WorkContext.Token, new StringContent(JsonConvert.SerializeObject(new { username = PurchasingDataSettings.Username, password = PurchasingDataSettings.Password }), Encoding.UTF8, "application/json"));

            //TokenResult tokenResult = new TokenResult();
            //if (garmentUnitExpenditureNoteResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
            //{
            //    return garmentUnitExpenditureNoteResponse.EnsureSuccessStatusCode().ToString();
            //}
            return garmentUnitExpenditureNoteResponse.EnsureSuccessStatusCode().ToString();
        }

        protected async Task<string> PutGarmentUnitExpenditureNoteDelete(int id)
        {
            var garmentUnitExpenditureNoteUri = PurchasingDataSettings.Endpoint + $"garment-unit-expenditure-notes/isPreparingFalse/{id}";
            var garmentUnitExpenditureNoteResponse = await _http.PutAsync(garmentUnitExpenditureNoteUri, WorkContext.Token, new StringContent(JsonConvert.SerializeObject(new { username = PurchasingDataSettings.Username, password = PurchasingDataSettings.Password }), Encoding.UTF8, "application/json"));

            //TokenResult tokenResult = new TokenResult();
            //if (garmentUnitExpenditureNoteResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
            //{
            //    tokenResult = JsonConvert.DeserializeObject<TokenResult>(await garmentUnitExpenditureNoteResponse.Content.ReadAsStringAsync());

            //}
            return garmentUnitExpenditureNoteResponse.EnsureSuccessStatusCode().ToString();
        }

        protected ProductResult GetGarmentProducts(string keyword = null)
        {
            var masterProductUri = MasterDataSettings.Endpoint + $"master/garmentProducts?size={int.MaxValue}&keyword={keyword}";
            //var masterUnitUri = $"https://com-danliris-service-core-dev.azurewebsites.net/v1/master/products/simple";
            var productResponse = _http.GetAsync(masterProductUri).Result;

            var productResult = new ProductResult();
            if (productResponse.IsSuccessStatusCode)
            {
                productResult = JsonConvert.DeserializeObject<ProductResult>(productResponse.Content.ReadAsStringAsync().Result);
            }
            else
            {
                GetProducts(keyword);
            }
            return productResult;
        }

        protected SingleProductResult GetGarmentProduct(int id, string token)
        {
            var masterProductUri = MasterDataSettings.Endpoint + $"master/garmentProducts/{id}";
            var productResponse = _http.GetAsync(masterProductUri).Result;

            if (productResponse.IsSuccessStatusCode)
            {
                SingleProductResult productResult = JsonConvert.DeserializeObject<SingleProductResult>(productResponse.Content.ReadAsStringAsync().Result);
                return productResult;
            }
            else
            {
                return new SingleProductResult();
            }
        }

        protected void VerifyUser()
        {
            WorkContext.UserName = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
            WorkContext.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
            WorkContext.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
        }

        protected IActionResult Ok<T>(T data, object info = null, string message = null)
        {
            return base.Ok(new
            {
                apiVersion = this.WorkContext.ApiVersion,
                success = true,
                data,
                info,
                message,
                statusCode = HttpStatusCode.OK
            });
        }
    }
}