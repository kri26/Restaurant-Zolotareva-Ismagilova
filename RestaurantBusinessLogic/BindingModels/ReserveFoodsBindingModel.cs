﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantBusinessLogic.BindingModels
{
    public class ReserveFoodsBindingModel
    {
        public int RequestId { get; set; }
        public int FoodId { get; set; }
    }
}