using System;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Entities
{
    public partial class NorthwindEntitiesSaveCHanges : NorthwindEntities
    {
        /*partial*/ void OnContextCreated()
        {
            this.Connection.StateChange += new StateChangeEventHandler(Connection_StateChange);
        }

        void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            var conn = ((EntityConnection)sender).StoreConnection;
            Console.WriteLine("{0}: Database {1}, State: {2}, was {3}", DateTime.Now.ToShortDateString(), conn.Database, e.CurrentState, e.OriginalState);
        }

        public override int SaveChanges(SaveOptions options)
        {
            var allDeletedEntities = this.ObjectStateManager
                                        .GetObjectStateEntries(EntityState.Deleted)
                                        .Select(s => s.Entity);

            // do something 
            //return base.SaveChanges(options);

            //var pos = this.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified).Select(entry => entry.Entity).OfType<PurchaseOrder>().ToList();
            //foreach (var order in pos)
            //{
            //    if (order.EntityState == EntityState.Added)
            //    {
            //        order.PurchaseOrderId = Guid.NewGuid();
            //        order.CreateDate = DateTime.Now;
            //        order.ModifiedDate = DateTime.Now;
            //    }
            //    else if (order.EntityState == EntityState.Modified)
            //    {
            //        order.ModifiedDate = DateTime.Now;
            //    }
            //}

            // Validate
            //var entries = this.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Where(entry => entry.Entity is Employee);
            //foreach (var entry in entries)
            //{
            //    var salaryProp = entry.GetModifiedProperties().FirstOrDefault(p => p == "Salary");
            //    if (salaryProp != null)
            //    {
            //        var originalSalary = Convert.ToDecimal(entry.OriginalValues[salaryProp]);
            //        var currentSalary = Convert.ToDecimal(entry.CurrentValues[salaryProp]);
            //        if (originalSalary != currentSalary)
            //        {
            //            if (currentSalary > originalSalary * 1.1M)
            //                throw new ApplicationException("Can't increase salary more than 10%");
            //        }
            //    }
            //}

            var affected = base.SaveChanges(options);
            if (SaveOptions.AcceptAllChangesAfterSave == (SaveOptions.AcceptAllChangesAfterSave & options))
            {
                //var entities = this.ObjectStateManager.GetObjectStateEntries(EntityState.Unchanged).Where(e => !e.IsRelationship).Select(e => e.Entity).OfType<IObjectWithChangeTracker>();
                //foreach (var entity in entities)
                //{
                //    entity.AcceptChanges();
                //}
            }
            return affected;
        }

        public void StartSelfTracking()
        {
            //var entities = this.ObjectStateManager.GetObjectStateEntries(~EntityState.Detached).Where(e => !e.IsRelationship).Select(e => e.Entity).OfType<IObjectWithChangeTracker>();
            //foreach (var entity in entities)
            //{
            //    entity.StartTracking();
            //}
        }
    }

    class Program
    {

        static void DeleteRelatedEntities<T>(T entity, NorthwindEntities context) where T : EntityObject
        {
            var entities = ((IEntityWithRelationships)entity).RelationshipManager.GetAllRelatedEnds().SelectMany(e => e.CreateSourceQuery().OfType<EntityObject>()).ToList();
            foreach (var child in entities)
            {
                context.DeleteObject(child);
            }
            context.SaveChanges();
        }

        public void SqlEntity_Test(NorthwindEntities context)
        {
            string eSql = "SELECT VALUE c FROM NorthwindEntities.Categories AS c";
            ObjectQuery<Category> query = context.CreateQuery<Category>(eSql);

            var result = query.ToList();

            using (var conn = new EntityConnection("name=NorthwindEntities"))
            {
                conn.Open();
                EntityCommand cmd = conn.CreateCommand();
                cmd.CommandText = eSql;
                EntityDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            }
        }

        public void TestSaveChanges()
        {
            using (var ctx = new Entities.NorthwindEntities())
            {
                ctx.SavingChanges += new EventHandler(ctx_SavingChanges);
                var region = ctx.Regions.CreateObject();
                region.RegionDescription = "Sample Region";
                ctx.Regions.AddObject(region);
                ctx.SaveChanges();

                var regionAdded = ctx.Regions.FirstOrDefault(r => r.RegionID.Equals(region.RegionID));
                ctx.Regions.DeleteObject(region);

                var regionState = ctx.ObjectStateManager.GetObjectStateEntry(region).State; // get the state of the entity

                var upState = ctx.ObjectStateManager.GetObjectStateEntry(region);
                //upState.ApplyCurrentValues(region);
                var originalValues = upState.GetUpdatableOriginalValues();


                //ctx.ContextOptions.

                ctx.SaveChanges();



                var context = new Entities.NorthwindEntities();


                //regionAdded.RegionDescription = "Updated Region";

                ctx.Regions.ApplyCurrentValues(regionAdded);
                var entry = ctx.ObjectStateManager.GetObjectStateEntry(regionAdded);
                var origValues = entry.GetUpdatableOriginalValues();

                //entry.CurrentValues[]
                //var entry = ctx.ObjectStateManager.GetObjectStateEntry(regionAdded);
                //var origValues = entry.GetUpdatableOriginalValues();
                //var properties = entry.GetModifiedProperties();
                //ctx.DetectChanges();
                ctx.SaveChanges();
            }
        }

        // Update only changed properties, deatach scenario
        public void UpdateOnlyChangedPropertiesProject(Category category, Category originalCategory)
        {
            using (var context = new NorthwindEntities())
            {
                context.Categories.Attach(category);
                context.Categories.ApplyOriginalValues(originalCategory);
                context.SaveChanges();
            }
        }

        public Category SubmitCategory(Category category)
        {
            using (var context = new NorthwindEntities())
            {
                context.Categories.Attach(category);
                if (category.CategoryID == 0)
                {
                    // this is an insert
                    context.ObjectStateManager.ChangeObjectState(category, EntityState.Added);
                }
                else
                {
                    var entry = context.ObjectStateManager.GetObjectStateEntry(category);
                    entry.SetModifiedProperty("categoryText");
                }
                context.SaveChanges();
                return category;
            }
        }

        public void ResolveOptimisticScenario(ObjectContext ctx)
        {
            try
            {
                ctx.SaveChanges();
            }
            catch (OptimisticConcurrencyException oce)
            {
                var errorEntry = oce.StateEntries.First();
                ctx.Refresh(RefreshMode.ClientWins, errorEntry.Entity);
            }
        }

        void ctx_SavingChanges(object sender, EventArgs e)
        {
            var ctx = (sender as Entities.NorthwindEntities);
            var osm = ctx.ObjectStateManager;
            var entities = osm.GetObjectStateEntries(System.Data.EntityState.Added | System.Data.EntityState.Deleted);

            var objEntry = ctx.Categories.First();
            var entityToManipulate = osm.GetObjectStateEntry(objEntry);
            entityToManipulate.Delete();
            entityToManipulate.SetModified();
            entityToManipulate.SetModifiedProperty("SomePropertyHere");
            entityToManipulate.AcceptChanges();
            entityToManipulate.ChangeState(EntityState.Deleted);

            foreach (var entry in entities)
            {
                Console.WriteLine("Entry {0} - {1}", entry.Entity.GetType().Name, entry.State);
                //.SetModifiedProperty
            }
        }

        private void UpdateObjectWithApplyCurrentValues(Entities.NorthwindEntities ctx, Category cat)
        {
            var category = ctx.Categories.OfType<Category>().First(c => c.CategoryID == cat.CategoryID);
            ctx.Categories.ApplyCurrentValues(category);
            var entry = ctx.ObjectStateManager.GetObjectStateEntry(category);
            var originalValues = entry.GetUpdatableOriginalValues();

            var modifiedProperties = entry.GetModifiedProperties();

            originalValues.SetValue(originalValues.GetOrdinal("Description"), cat.Description);
            ctx.SaveChanges();
        }

        private void UpdateValueOfflineSystemWCF(Category category)
        {
            using (var ctx = new Entities.NorthwindEntities())
            {
                ctx.Categories.Attach(category);
                var entry = ctx.ObjectStateManager.GetObjectStateEntry(category);
                entry.SetModifiedProperty("Description");
                entry.SetModifiedProperty("SomePropertyToSet");
                ctx.SaveChanges();
            }
        }

        /*
            The easiest way to make an entity that’s returned by a query serializable by WCF is to
            set the ContextOptions.ProxyCreationEnabled property of the context to false (by
            default, it’s true) before performing the query. This way, you obtain the plain entity
            instead of a proxied one, eliminating any serialization problem.
            Disabling proxy generation isn’t always possible because you may need lazy loading
            or change tracking. In such cases, you need to serialize a proxied instance.
            In section 16.1.3, you’ve learned that ProxyDataContractResolver is the key to
            proxy serialization. The contract resolution is a WCF feature that enables you to map
            the actual type (the proxy) to a WCF known type (the entity exposed) using the workflow
            illustrated.  ProxyDataContractResolver performs such mappings but it isn’t able to interfere
            with the WCF pipeline. That’s something you have to do manually. Fortunately, this process
            is pretty simple; you just have to create an attribute and decorate the service interface
            methods with it. You can see the code for the attribute in the following listing.
          
          		[OperationContract]
		        [ProxyResolverAttribute]
		        OrderIT.Model.STE.Order ReadOrderUsingSTE(int orderId);
         */
        public class ProxyResolverAttribute : Attribute, IOperationBehavior
        {
            public ProxyResolverAttribute()
            {
            }

            public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
            {
            }

            public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
            {
                SetResolver(description);
            }

            public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
            {
                SetResolver(description);
            }

            public void Validate(OperationDescription operationDescription)
            {
            }

            private void SetResolver(OperationDescription description)
            {
                var dataContractSerializerOperationBehavior = description.Behaviors.Find<DataContractSerializerOperationBehavior>();
                dataContractSerializerOperationBehavior.DataContractResolver = new ProxyDataContractResolver();
            }
        }

        static Func<NorthwindEntities, string, IQueryable<Customer>> compQuery = CompiledQuery.Compile<NorthwindEntities, string, IQueryable<Customer>>((ctx, name) =>
            ctx.Customers.OfType<Customer>().Where(c => c.CompanyName.StartsWith(name))
        );

        //static void Main(string[] args)
        //{
        //    var prg = new Program();

        //    var ctx = new NorthwindEntities();
        //    ctx.Customers.MergeOption = MergeOption.NoTracking;

        //    IQueryable<Customer> q = compQuery.Invoke(ctx, "C");

        //    prg.TestSaveChanges();

        //    Console.WriteLine("END");
        //    Console.Read();
        //}
    }

}
