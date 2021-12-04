
using MealManagementSoftwareDataLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace MealManagementSoftwareBusinessLayer
{
    public interface IBasketsService
    {
        List<Basket> GetDetail();
    }

}
