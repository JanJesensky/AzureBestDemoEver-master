//----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Entities;
using DAL;
using System.Net;
using System.Net.Mail;
using SendGrid;
using System.Configuration;
using SendGrid.Helpers.Mail;
using System.Net.Mime;
using System.Threading;

namespace ComputeWebJobsSDKStorageQueue1
{
    public class Functions
    {
        /// <summary>
        /// Reads an Order object from the "initialorder" queue
        /// Creates a blob for the specified order which contains the order details
        /// The message in "orders" will be picked up by "QueueToBlob"
        /// </summary>
        public static void MultipleOutput([QueueTrigger("subscription")] Subscription subscription)
        {
            try
            {
                Console.WriteLine("Web Job trigerred - new subscription has been found");

                Thread.Sleep(5000);

                DataContext context = new DataContext();

                context.Subscriptions.Add(subscription);

                context.SaveChanges();

                Console.WriteLine("Web Job - subscription Saved To Database");

                MailMessage mailMsg = new MailMessage();

                // To
                mailMsg.To.Add(new MailAddress(subscription.Email, String.Format("{0} {1}", subscription.FirstName, subscription.LastName)));

                // From
                mailMsg.From = new MailAddress("subscriptions@azurewebappsdemo.azurewebsites.net", "Best Demo Ever");

                // Subject and multipart/alternative Body
                mailMsg.Subject = "New Subscription Created";
                string text = "Thanks for subscribing. Enjoy!";
                string html = @"<p>Thanks for subscribing. Enjoy!</p>";
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("azure_76e48044306e4805b61010eb61703f4b@azure.com", "Heslo12!");
                smtpClient.Credentials = credentials;

                Console.WriteLine("Web Job - sending email to " + subscription.Email);
                smtpClient.Send(mailMsg);
                Console.WriteLine("Web Job - Email sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}
