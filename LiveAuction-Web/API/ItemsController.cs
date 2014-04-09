using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LiveAuction.APIs
{
    public class ItemsController : ApiController
    {
        public static List<Models.ItemModel> Items { get; set; }

        public ItemsController()
        {
            if (Items == null)
            {
                Items = new List<Models.ItemModel>();
                LoadItems();
            }
        }

        private void LoadItems()
        {
            if (Items == null)
            {
                LoadItems();
            }

            Items.Add(new Models.ItemModel()
            {
                id = 1,
                name = "Dell Laptop",
                desc = "A brand new Dell Laptop",
                reserve = 750.00M,
                price = 0.00M,
                status = "Started",
                bidder = "",
                start = DateTime.Now,
                end = DateTime.Now.AddMinutes(10),
                duration = 5
            });

            Items.Add(new Models.ItemModel()
            {
                id = 2,
                name = "Arc Touch Mouse",
                desc = "new in box",
                reserve = 10.00M,
                price = 0.00M,
                status = "Started",
                bidder = "",
                start = DateTime.Now,
                end = DateTime.Now.AddMinutes(10),
                duration = 5
            });

            Items.Add(new Models.ItemModel()
            {
                id = 3,
                name = "Surface 2",
                desc = "new in box",
                reserve = 299.00M,
                price = 0.00M,
                status = "Ended",
                bidder = "",
                start = DateTime.Now,
                end = DateTime.Now.AddMinutes(0),
                duration = 10
            });
        }

        [HttpGet]
        [Route("api/items/{status}")]
        public List<Models.ItemModel> GetItems(string status)
        {
            if (status.ToLower() == "all")
            {
                return Items;
            }
            else
            {
                return Items.Where(x => x.status.ToLower() == status.ToLower()).ToList();
            }         
        }

        [HttpGet]
        [Route("api/update/{id:int}/{price:int}")]
        public bool UpdatePrice(int id, decimal price)
        {
            var _item = Items.Where(x => x.id == id).SingleOrDefault();
            if (_item == null)
            {
                return false;
            }
            _item.price = price;
            return true;
        }
    }
}
