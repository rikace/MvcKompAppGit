using oDataService.Models;
//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Web;
using System.Web;

namespace oDataService
{
    public class PresidentsService : DataService<PresidentContext>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {

            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Presidents", EntitySetRights.AllRead);

            config.SetServiceOperationAccessRule("PresidentsByParty", ServiceOperationRights.AllRead);

            config.SetEntitySetPageSize("Presidents", 10);

            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }

        [QueryInterceptor("Presidents")]
        public Expression<Func<President, bool>> OnQueryPresidents()
        {
            if (HttpContext.Current.User.IsInRole("Admin"))
                return p => true;
            return p => false;
        }

        [ChangeInterceptor("Presidents")]
        public void OnChangePresidents(President president, UpdateOperations operations)
        {
            // update here
            // if (operations == UpdateOperations.Change)
            //{
            //    System.Data.Objects.ObjectStateEntry entry;

            //    if (this.CurrentDataSource.Presidents
            //        .TryGetObjectStateEntry(product, out entry))
            //    {
            //        // Reject changes to a discontinued Product.
            //        // Because the update is already made to the entity by the time the 
            //        // change interceptor in invoked, check the original value of the Discontinued
            //        // property in the state entry and reject the change if 'true'.
            //        if ((bool)entry.OriginalValues["Discontinued"])
            //        {
            //            throw new DataServiceException(400, string.Format(
            //                        "A discontinued {0} cannot be modified.", product.ToString()));
            //        }
            //    }
            //    else
            //    {
            //        throw new DataServiceException(string.Format(
            //            "The requested {0} could not be found in the data source.", product.ToString()));
            //    }
            //    }
            //        else if (operations == UpdateOperations.Delete)
            //        {
            //            // Block the delete and instead set the Discontinued flag.
            //            throw new DataServiceException(400,
            //                "Products cannot be deleted; instead set the Discontinued flag to 'true'");
            //        }
            //    }
        }

        [WebGet]
        public IQueryable<President> PresidentsByParty(string party)
        {
            return (from p in this.CurrentDataSource.Presidents
                    where p.Party == party
                    select p);
        }

        protected override void HandleException(HandleExceptionArgs args)
        {
            // log error
            base.HandleException(args);
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            base.OnStartProcessingRequest(args);
        }
    }
}
