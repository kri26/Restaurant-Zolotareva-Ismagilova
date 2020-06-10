﻿using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using RestaurantBusinessLogic.BindingModels;
using RestaurantBusinessLogic.BusinessLogics;
using RestaurantBusinessLogic.HelperModels;
using RestaurantBusinessLogic.Interfaces;
using RestaurantBusinessLogic.ViewModels;
using RestaurantWebSupplier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantWebSupplier.Controllers
{
    public class RequestController : Controller
    {
        private readonly IRequestLogic requestLogic;
        private readonly IFridgeLogic fridgeLogic;
        private readonly IFoodLogic foodLogic;
        private readonly SupplierBusinessLogic supplierLogic;
        private readonly SupplierReportLogic reportLogic;
        public RequestController(IRequestLogic requestLogic, IFridgeLogic fridgeLogic, SupplierBusinessLogic supplierLogic, SupplierReportLogic reportLogic, IFoodLogic foodLogic)
        {
            this.requestLogic = requestLogic;
            this.fridgeLogic = fridgeLogic;
            this.supplierLogic = supplierLogic;
            this.reportLogic = reportLogic;
            this.foodLogic = foodLogic;
        }

        public IActionResult Request()
        {
            if (Program.Supplier == null)
            {
                return new UnauthorizedResult();
            }
            var requests = requestLogic.Read(new RequestBindingModel
            {
                SupplierId = Program.Supplier.Id
            });
            return View(requests);
        }

        public IActionResult Report(ReportModel model)
        {
            var requests = new List<RequestViewModel>();
            requests = requestLogic.Read(new RequestBindingModel
            {
                SupplierId = Program.Supplier.Id,
                DateFrom = model.From,
                DateTo = model.To
            });
            ViewBag.Requests = requests;
            string fileName = "D:\\data\\Reportpdf.pdf";
            if (model.SendMail)
            {
                reportLogic.SaveFoodsToPdfFile(fileName, new RequestBindingModel
                {
                    SupplierId = Program.Supplier.Id,
                    DateFrom = model.From,
                    DateTo = model.To
                }, Program.Supplier.Login);
            }
            return View();
        }

        public IActionResult RequestView(int ID)
        {
            if (Program.Supplier == null)
            {
                return new UnauthorizedResult();
            }
            if (TempData["ErrorFoodReserve"] != null)
            {
                ModelState.AddModelError("", TempData["ErrorFoodReserve"].ToString());
            }
            ViewBag.RequestID = ID;
            var foods = requestLogic.Read(new RequestBindingModel
            {
                Id = ID
            })?[0].Foods;
            return View(foods);
        }

        public IActionResult Reserve(int requestId, int foodId)
        {
            if (Program.Supplier == null)
            {
                return new UnauthorizedResult();
            }
            supplierLogic.ReserveFoods(new ReserveFoodsBindingModel
            {
                RequestId = requestId,
                FoodId = foodId
            });
            return RedirectToAction("RequestView", new { id = requestId });
        }

        public IActionResult AcceptRequest(int id)
        {
            if (Program.Supplier == null)
            {
                return new UnauthorizedResult();
            }
            supplierLogic.AcceptRequest(new ChangeRequestStatusBindingModel
            {
                RequestId = id
            });
            return RedirectToAction("Request");
        }

        public IActionResult CompleteRequest(int id)
        {
            if (Program.Supplier == null)
            {
                return new UnauthorizedResult();
            }
            supplierLogic.CompleteRequest(new ChangeRequestStatusBindingModel
            {
                RequestId = id
            });
            return RedirectToAction("Request");
        }

        public IActionResult ListFoodAvailable(int id, int count, string name, int requestId)
        {
            if (Program.Supplier == null)
            {
                return new UnauthorizedResult();
            }
            ViewBag.FoodName = name;
            ViewBag.Count = count;
            ViewBag.FoodId = id;
            ViewBag.RequestId = requestId;
            var fridges = fridgeLogic.GetFridgeAvailable(new RequestFoodBindingModel
            {
                FoodId = id,
                Count = count
            });
            return View(fridges);
        }

        private int CalculateSum(List<RequestFoodBindingModel> requestfoods)
        {
            decimal sum = 0;
            foreach (var food in requestfoods)
            {
                var foodData = foodLogic.Read(new FoodBindingModel { Id = food.FoodId }).FirstOrDefault();
                if (foodData != null)
                {
                    for (int i = 0; i < food.Count; i++)
                        sum += foodData.Price;
                }
            }
            return Convert.ToInt32(sum);
        }

        public IActionResult SendWordReport(int id)
        {
            string fileName = "D:\\data\\" + id + ".docx";
            reportLogic.SaveNeedFoodToWordFile(new WordInfo
            {
                FileName = fileName,
                RequestId = id,
                SupplierFIO = Program.Supplier.SupplierFIO
            }, Program.Supplier.Login);
            return RedirectToAction("Request");
        }
        public IActionResult SendExcelReport(int id)
        {
            string fileName = "D:\\data\\" + id + ".xlsx";
            reportLogic.SaveNeedFoodToExcelFile(new ExcelInfo
            {
                FileName = fileName,
                RequestId = id,
                SupplierFIO = Program.Supplier.SupplierFIO
            }, Program.Supplier.Login);
            return RedirectToAction("Request");
        }
    }
}