using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABCSuperMarketWebApp.Models;
using ABCSuperMarketWebApp.TableHandler;
using ABCSuperMarketWebApp.BlobHandler;

namespace ABCSuperMarketWebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(string id)
        {

            //for edit
            if (!string.IsNullOrEmpty(id))
            {
                //set the name of the table
                TableManager TableManagerObj = new TableManager("Items");

                //retrieve the item to be updated
                List<Item> ItemListObj = TableManagerObj.RetrieveEntity<Item>("RowKey eq '" + id + "'");
                Item ItemObj = ItemListObj.FirstOrDefault();
                return View(ItemObj);

            }

            return View(new Item());
        }

        [HttpPost]
        public ActionResult Index(string id, HttpPostedFileBase uploadFile, FormCollection formData)
        {
            Item ItemObj = new Item();
            ItemObj.ItemName = formData["ItemName"] == "" ? null : formData["ItemName"];
            ItemObj.ItemDescription = formData["ItemDescription"] == "" ? null : formData["ItemDescription"];
            double price;
            if (double.TryParse(formData["ItemPrice"], out price))
            {
                ItemObj.ItemPrice = double.Parse(formData["ItemPrice"] == "" ? null : formData["ItemPrice"]);
            }
            else
            {
                return View(new Item());
            }

            foreach (string file in Request.Files)
            {
                uploadFile = Request.Files[file];
            }

            //container
            BlobManager BlobManagerObj = new BlobManager("images");
            string FileAblsoluteUri = BlobManagerObj.UploadFile(uploadFile);



            TableManager TableManagerObj = new TableManager("Items");

            //Insert Statement
            if (string.IsNullOrEmpty(id))
            {
                ItemObj.PartitionKey = "Item";
                ItemObj.RowKey = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(FileAblsoluteUri))
                {
                    ItemObj.FilePath = "https://abcsupermarketstorageac.blob.core.windows.net/images/NOImage.jpg";
                }
                else
                {
                    ItemObj.FilePath = FileAblsoluteUri.ToString();
                }

                TableManagerObj.InsertEntity<Item>(ItemObj, true);

            }else
            {
                ItemObj.PartitionKey = "Item";
                ItemObj.RowKey = id;
                
                if (string.IsNullOrEmpty(FileAblsoluteUri))
                {
                    ItemObj.FilePath = TableManagerObj.RetrieveEntity<Item>("RowKey eq '" + ItemObj.RowKey + "'").LastOrDefault<Item>().FilePath;
                }
                else
                {
                    ItemObj.FilePath = FileAblsoluteUri.ToString();
                }

                TableManagerObj.InsertEntity<Item>(ItemObj, false);

            }

            return RedirectToAction("Get");
        }

        //get Items
        public ActionResult Get()
        {
            TableManager TableManagerObj = new TableManager("Items");
            List<Item> ItemListObj = TableManagerObj.RetrieveEntity<Item>(null);
            return View(ItemListObj);
        }

        //Delete Item
        public ActionResult Delete(string id)
        {
            //return the item to be delete
            TableManager TableManagerObj = new TableManager("Items");
            List<Item> ItemListObj = TableManagerObj.RetrieveEntity<Item>("RowKey eq '" + id + "'");
            Item ItemObj = ItemListObj.FirstOrDefault();

            //delete the item
            TableManagerObj.DeleteEntity<Item>(ItemObj);
            BlobManager blobManagerObj = new BlobManager("images");
            blobManagerObj.DeleteBlob(ItemObj.FilePath);
            return RedirectToAction("Get");


        }


    }
}