﻿using RestaurantBusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RestaurantBusinessLogic.ViewModels
{
    public class RequestViewModel
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        [DisplayName("ФИО")]
        public string SupplierFIO { get; set; }
        [DisplayName("Сумма")]
        public decimal Sum { get; set; }
        [DisplayName("Дата создания")]
        public DateTime DateCreate { get; set; }
        [DisplayName("Дата выполнения")]
        public DateTime? DateImplement { get; set; }
        [DisplayName("Статус")]
        public Status Status { get; set; }
        public List<RequestFoodViewModel> RequestFoods { get; set; }
    }
}