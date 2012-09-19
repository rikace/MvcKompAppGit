using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCControlsToolkit.Controls;
using MVCControlsToolkit.Core;
using System.ComponentModel.DataAnnotations;
namespace MVCNestedModels.Controls
{
    public class PercentageUpdater:IUpdateModel
    {
      
        public float[] Percentages { get; set; }
        public string[] Labels { get; set; }
        [Required(ErrorMessage="errrrrror")]
        public float Total { get; set; }
        public object UpdateModel(object model, string[] fields)
        {
            float partialTotal = 0f;
            foreach (float percentage in Percentages) partialTotal += percentage;
            if (partialTotal != 0f)
            {
                for (int index = 0; index < fields.Length; index++)
                {
                    new PropertyAccessor(model, fields[index]).Value = (Percentages[index]/partialTotal) * Total;
                }
            }
            return model;
        }

        public void ImportFromModel(object model, object[] fields, string[] fieldNames, object[] args = null)
        {
            Percentages=new float[fields.Length];
            Labels=new string[fieldNames.Length];
            Total = 0;
            for(int index = 0; index < fields.Length; index++ )
            {
                Percentages[index] = (float)fields[index];
                Labels[index]=new PropertyAccessor(model, fieldNames[index]).DisplayName;
                Total += Percentages[index];
            }
            if (Total != 0f)
            {
                for (int index = 0; index < fields.Length; index++)
                {
                    Percentages[index] = Percentages[index]*100 / Total;
                }
            }
        }
    }
}