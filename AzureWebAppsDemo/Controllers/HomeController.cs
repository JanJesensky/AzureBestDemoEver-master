using AzureWebAppsDemo.Models;
using DAL;
using DataTables.Mvc;
using Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureWebAppsDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubscription(SubscriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("subscription");
                queue.CreateIfNotExists();

                Subscription subscription = new Subscription()
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CreatedDate = DateTime.UtcNow
                };

                queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(subscription)));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("api/queue")]
        public JsonResult GetSubscriptionsInQueue([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest model)
        {
            var subscriptions = new List<Subscription>();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("subscription");
            queue.CreateIfNotExists();

            var messages = queue.PeekMessages(5);

            foreach (var message in messages)
            {
                var subscription = JsonConvert.DeserializeObject<Subscription>(message.AsString);
                subscription.CreatedDate = message.InsertionTime.HasValue ? message.InsertionTime.Value.DateTime : DateTime.Now;
                subscriptions.Add(subscription);
            }

            var sortedColumns = model.Columns.GetSortedColumns().ToList();

            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.Name)
                {
                    case "First Name":
                        subscriptions = sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? subscriptions.OrderBy(s => s.FirstName).ToList() : subscriptions.OrderByDescending(s => s.FirstName).ToList();
                        break;
                    case "Last Name":
                        subscriptions = sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? subscriptions.OrderBy(s => s.LastName).ToList() : subscriptions.OrderByDescending(s => s.LastName).ToList();
                        break;
                    case "Email":
                        subscriptions = sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? subscriptions.OrderBy(s => s.Email).ToList() : subscriptions.OrderByDescending(s => s.Email).ToList();
                        break;
                    case "Created Date":
                        subscriptions = sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? subscriptions.OrderBy(s => s.CreatedDate).ToList() : subscriptions.OrderByDescending(s => s.CreatedDate).ToList();
                        break;
                }
            }

            if (model.Length != -1)
            {
                subscriptions = subscriptions
                    .Skip(model.Start)
                    .Take(model.Length)
                    .ToList();
            }

            return Json(new DataTablesResponse(model.Draw, subscriptions, 0, 0));
        }

        [HttpPost]
        [Route("api/subscriptions")]
        public JsonResult GetSubscriptions([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest model)
        {
            DataContext context = new DataContext();
            var subscriptions = context.Subscriptions.ToList();
            var totalRecordsCount = subscriptions.Count;

            var sortedColumns = model.Columns.GetSortedColumns().ToList();

            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.Name)
                {
                    case "First Name":
                        subscriptions = sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? subscriptions.OrderBy(s => s.FirstName).ToList() : subscriptions.OrderByDescending(s => s.FirstName).ToList();
                        break;
                    case "Last Name":
                        subscriptions = sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? subscriptions.OrderBy(s => s.LastName).ToList() : subscriptions.OrderByDescending(s => s.LastName).ToList();
                        break;
                    case "Email":
                        subscriptions = sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? subscriptions.OrderBy(s => s.Email).ToList() : subscriptions.OrderByDescending(s => s.Email).ToList();
                        break;
                    case "Created Date":
                        subscriptions = sortedColumn.SortDirection == Column.OrderDirection.Ascendant ? subscriptions.OrderBy(s => s.CreatedDate).ToList() : subscriptions.OrderByDescending(s => s.CreatedDate).ToList();
                        break;
                }
            }

            if (model.Length != -1)
            {
                subscriptions = subscriptions
                    .Skip(model.Start)
                    .Take(model.Length)
                    .ToList();
            }

            return Json(new DataTablesResponse(model.Draw, subscriptions, totalRecordsCount, totalRecordsCount));
        }
    }
}