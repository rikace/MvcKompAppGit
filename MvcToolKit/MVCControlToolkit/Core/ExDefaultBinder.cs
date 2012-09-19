/* ****************************************************************************
 *
 * Copyright (c) Francesco Abbruzzese. All rights reserved.
 * francesco@dotnet-programming.com
 * http://www.dotnet-programming.com/
 * 
 * This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
 * and included in the license.txt file of this distribution.
 * 
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MVCControlsToolkit.DataAnnotations;

namespace MVCControlsToolkit.Core
{
    public class ExDefaultBinder: DefaultModelBinder
    {
        public static string TypeRegisterPrefix = "__all_page_types_register__";
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            string displayPrefix = bindingContext.ModelName;
            if (displayPrefix == "model" || (bindingContext.ModelMetadata.PropertyName == null && !displayPrefix.Contains('.') && !displayPrefix.Contains('[') && !displayPrefix.Contains('$'))) displayPrefix = "display";
            ValueProviderResult attemptedTransformer = bindingContext.ValueProvider.GetValue(displayPrefix + ".$$");
            if (displayPrefix == "display" && attemptedTransformer == null) attemptedTransformer = bindingContext.ValueProvider.GetValue("$$");
            if (attemptedTransformer != null && attemptedTransformer.AttemptedValue != null)
            {
                Type displayModelType = BasicHtmlHelper.DecodeDisplayInfo(attemptedTransformer.AttemptedValue, bindingContext.ValueProvider);
               
                if (displayModelType != null && displayModelType.IsClass)
                {
                    object[] displayModelContext = null;

                    if (displayModelType.GetInterface("IDisplayModel") != null)
                    {
                        displayModelContext = new object[] { bindingContext.ModelName, controllerContext.HttpContext.Items };
                    }
                    else if (displayModelType.GetInterface("IDisplayModelBuilder") != null)
                    {
                        object displayModelBuilder = displayModelType.GetConstructor(new Type[0]).Invoke(new object[0]);
                        IDisplayModelBuilder displayModelFarmInterface = displayModelBuilder as IDisplayModelBuilder;
                        if (displayModelFarmInterface != null)
                        {
                            displayModelContext = displayModelFarmInterface.DisplayModelContext;
                            displayModelType = displayModelFarmInterface.DisplayModelType;
                            if (displayModelType.GetInterface("IDisplayModel") == null) displayModelType = null;
                        }
                    }
                    else displayModelType = null;
                    if (displayModelType != null)
                    {
                        
                        IDisplayModel displayModel = BindDisplayModel(controllerContext, bindingContext, displayModelType) as IDisplayModel;
                        if(displayModel !=null)
                        {
                            try
                            {
                                IUpdateModelState msUpdater = displayModel as IUpdateModelState;
                                if (msUpdater != null)
                                {
                                    msUpdater.GetCurrState(displayPrefix, -1, bindingContext.ModelState);
                                }
                                return  UpdateModel(controllerContext, bindingContext,
                                    displayModel.ExportToModel(bindingContext.ModelType, displayModelContext));
                            }
                            catch (Exception ex)
                            {
                                bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format(ex.Message, bindingContext.ModelMetadata.GetDisplayName()));
                                return null;
                            }
                        }
                    }

                    
                    
                }
            }
            object res = null;
            try
            {
                res = base.BindModel(controllerContext, bindingContext);
            }
            catch { }
            return
                 UpdateModel(controllerContext, bindingContext,
                   res);

        }

        protected string LowerModelName(string modelName, string postfix)
        {
            string res = postfix;
            if (!string.IsNullOrWhiteSpace(modelName) && modelName != "display" && modelName != "updatemodel")
            {
                res=modelName+postfix;
            }
            if (res[0] == '.') res = res.Substring(1);
            return res;
        }

        protected object BindDisplayModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type displayModelType, string postfix=".$")
        {

            ModelBindingContext newBindingContext = new ModelBindingContext();
            newBindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType((() => null), displayModelType);
            newBindingContext.ModelMetadata.IsRequired = bindingContext.ModelMetadata.IsRequired;
            newBindingContext.ModelMetadata.AdditionalValues.Add("OriginalModel", bindingContext.ModelType);
            if (string.IsNullOrWhiteSpace(bindingContext.ModelName) || bindingContext.ModelName == "model" || (bindingContext.ModelMetadata.PropertyName == null && !bindingContext.ModelName.Contains('.') && !bindingContext.ModelName.Contains('[') && !bindingContext.ModelName.Contains('$')))
            {
                newBindingContext.ModelName = postfix;
            }
            else
                newBindingContext.ModelName = bindingContext.ModelName+postfix;
            if (newBindingContext.ModelName[0] == '.') newBindingContext.ModelName = newBindingContext.ModelName.Substring(1);
            newBindingContext.ModelState = bindingContext.ModelState;
            newBindingContext.PropertyFilter = bindingContext.PropertyFilter;
            newBindingContext.ValueProvider = bindingContext.ValueProvider;
            object model = BindModel(controllerContext, newBindingContext);
            if (model != null) return model;
            model = CreateModel(controllerContext, newBindingContext, displayModelType);
            newBindingContext.ModelMetadata.Model = model;
            if (OnModelUpdating(controllerContext, newBindingContext))
            {
                BindProperties(controllerContext, newBindingContext);
                OnModelUpdated(controllerContext, newBindingContext);
            }
            return model;
            //return BindModel(controllerContext, newBindingContext);

            
        }
        protected object BindUpdateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type displayModelType, string postfix = ".$")
        {

            ModelBindingContext newBindingContext = new ModelBindingContext();
            newBindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType((() => null), displayModelType);
            newBindingContext.ModelMetadata.IsRequired = bindingContext.ModelMetadata.IsRequired;
            newBindingContext.ModelMetadata.AdditionalValues.Add("OriginalModel", bindingContext.ModelType);
            if (string.IsNullOrWhiteSpace(bindingContext.ModelName) || bindingContext.ModelName == "model" || (bindingContext.ModelMetadata.PropertyName == null && !bindingContext.ModelName.Contains('.') && !bindingContext.ModelName.Contains('[') && !bindingContext.ModelName.Contains('$')))
            {
                newBindingContext.ModelName = postfix;
            }
            else
                newBindingContext.ModelName = bindingContext.ModelName + postfix;
            if (newBindingContext.ModelName[0] == '.') newBindingContext.ModelName = newBindingContext.ModelName.Substring(1);
            newBindingContext.ModelState = bindingContext.ModelState;
            newBindingContext.PropertyFilter = bindingContext.PropertyFilter;
            newBindingContext.ValueProvider = bindingContext.ValueProvider;
            
            return BindModel(controllerContext, newBindingContext);


        }
        private void BindProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            IEnumerable<PropertyDescriptor> properties = GetFilteredModelProperties(controllerContext, bindingContext);
            foreach (PropertyDescriptor property in properties)
            {
                BindProperty(controllerContext, bindingContext, property);
            }
        }
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            object res = base.CreateModel(controllerContext, bindingContext, modelType);
            if (res is IUpdateModelType) 
                (res as IUpdateModelType).SendFatherType(bindingContext.ModelMetadata.AdditionalValues["OriginalModel"] as Type);
            return res;
        }
        protected void AlignModelState(ModelStateDictionary modelState, List<int> trueIndexes, string prefix)
        {
            if (trueIndexes == null || trueIndexes.Count == 0) return;
            int[] indexTranslation = trueIndexes.ToArray();

            List<KeyValuePair<string, ModelState>> toChange = new List<KeyValuePair<string, ModelState>>();
            List<string> toRemove = new List<string>();
            foreach (string key in modelState.Keys)
            {

                if (key.StartsWith(prefix))
                {
                    string tail = key.Substring(prefix.Length);
                    int endIndex = tail.IndexOf('.');
                    int index = 0;
                    int.TryParse(tail.Substring(0, endIndex), out index);
                    if (index == indexTranslation[index]) continue;
                    toRemove.Add(key); 
                    if (indexTranslation[index] >= 0)
                    {
                        string changedLowerModelName =
                            prefix + indexTranslation[index].ToString() + tail.Substring(endIndex);
                        toChange.Add(new KeyValuePair<string, ModelState>(changedLowerModelName, modelState[key]));
                    }
                }
            }
            foreach (string key in toRemove) modelState.Remove(key);
            foreach (KeyValuePair<string, ModelState> pair in toChange)
            {
                modelState.Add(pair.Key, pair.Value);
                
            }
 
        }

        protected object UpdateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
        {
           
            bool cont = true;
            int index=0;
            int notNullCount = 0;
            int prevErrors;
            List<int> trueIndexes = null;
            Type previousModelType = null;
            while(cont)
            {
                string displayPrefix = bindingContext.ModelName;
                if (displayPrefix == "model" || (bindingContext.ModelMetadata.PropertyName == null && !displayPrefix.Contains('.') && !displayPrefix.Contains('[') && !displayPrefix.Contains('$'))) displayPrefix = "updatemodel";
                ValueProviderResult attemptedTransformer = bindingContext.ValueProvider.GetValue(displayPrefix + string.Format(".$${0}", index));
                if (attemptedTransformer != null && attemptedTransformer.AttemptedValue != null)
                {
                    Type displayModelType = BasicHtmlHelper.DecodeUpdateInfo(attemptedTransformer.AttemptedValue, previousModelType, bindingContext.ValueProvider);
                    
                    if (displayModelType != null && displayModelType.IsClass && displayModelType.GetInterface("IUpdateModel") != null)
                    {
                        prevErrors = bindingContext.ModelState.Keys.Count;
                        previousModelType = displayModelType;
                        IUpdateModel displayModel = BindDisplayModel(controllerContext, bindingContext, displayModelType, string.Format(".$${0}.$", index)) as IUpdateModel;
                        if (trueIndexes == null) trueIndexes = new List<int>();
                        if (displayModel == null)
                        {
                            trueIndexes.Add(-1);
                        }
                        else
                        {
                            
                            string[] fields=null;
                            ValueProviderResult fieldsAttemptedTransformer = bindingContext.ValueProvider.GetValue(displayPrefix + string.Format(".$${0}.f$ields", index));
                            if (fieldsAttemptedTransformer != null && !string.IsNullOrWhiteSpace(fieldsAttemptedTransformer.AttemptedValue))
                            {
                                fields = BasicHtmlHelper.DecodeFieldsInfo(fieldsAttemptedTransformer.AttemptedValue).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            }
                            else
                                fields = new string[0];
                            int beforeElements = -1;
                            ICollection beforeCollection = model as ICollection;
                            
                            if (beforeCollection != null)
                            {
                                beforeElements = beforeCollection.Count;
                                
                            }
                            IUpdateModelState updateModelState = displayModel as IUpdateModelState;
                            bool finished = false;
                            if (updateModelState != null)
                            {

                                if (updateModelState.MoveState && index != notNullCount)
                                {
                                    trueIndexes.Add(notNullCount);
                                    AlignModelState(
                                        bindingContext.ModelState,
                                        trueIndexes,
                                        LowerModelName(displayPrefix, ".$$"));
                                    finished = true;
                                }
                                
                                updateModelState.GetCurrState(displayPrefix, notNullCount, bindingContext.ModelState);
                            }
                            IVisualState visualStateStorage = displayModel as IVisualState;
                            if (visualStateStorage != null)
                            {
                                visualStateStorage.ModelName = bindingContext.ModelName;
                                visualStateStorage.Store = controllerContext.HttpContext.Items;
                            }
                            IHandleUpdateIndex handleUpdateIndex = displayModel as IHandleUpdateIndex;
                            if (handleUpdateIndex != null)
                            {
                                handleUpdateIndex.Index = index;
                                handleUpdateIndex.ModelState = bindingContext.ModelState;
                            }
                            try
                            {
                                model=displayModel.UpdateModel(model, fields);
                            }
                            catch (Exception ex)
                            {
                                bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format(ex.Message, bindingContext.ModelMetadata.GetDisplayName()));
                                    
                            }
                            if (finished) break;
                            int afterElements = -1;
                            ICollection afterCollection = model as ICollection;
                            if (afterCollection != null) afterElements = afterCollection.Count;
                            bool noCollections = afterElements == -1 && beforeElements == -1;

                            if (model != null && (noCollections || beforeElements != afterElements))
                            {
                                trueIndexes.Add(notNullCount);

                                notNullCount++;
                            }
                            else
                            {
                                trueIndexes.Add(-1);

                            }
                            
                        }
                    }
                }
                else
                {
                    if (index != notNullCount)
                        AlignModelState(
                            bindingContext.ModelState,
                            trueIndexes,
                            LowerModelName(displayPrefix, ".$$"));
                    cont = false;
                    break;
                }
                index++;
            }
            return model;
        }
        protected override ICustomTypeDescriptor GetTypeDescriptor(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return new AssociatedMetadataTypeTypeDescriptionProviderExt(bindingContext.ModelType).GetTypeDescriptor(bindingContext.ModelType);
            
        }
    }
}
