using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations;

namespace ABCSuperMarketWebApp.Models
{
    public class Item : TableEntity
    {
        public Item() { }


        public string ItemName { get; set; } = "";
        public string ItemDescription { get; set; } = "";
        public double ItemPrice { get; set; } = 0;
        public string FilePath { get; set; } = "";
    }
}