using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MVCControlsToolkit.Core;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using MVCControlsToolkit.DataAnnotations;

namespace MVCNestedModels.Models
{
    public class PersonalInfos
    {
        [DisplayName("Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DisplayName("Surname")]
        [DataType(DataType.Text)]
        public string SurName { get; set; }

        [DisplayName("Female")]
        public bool Female { get; set; }

       

        [DisplayName("Amount Invested In Forex")]
        public float ForexInvestment { get; set; }
        [DisplayName("Amount Invested In Futures")]
        public float FuturesInvestment { get; set; }
        [DisplayName("Amount Invested In Shares")]
        public float SharesInvestment { get; set; }

        [DynamicRange(typeof(float), DynamicMinimum = "MinAge", DynamicMinimumDelay = "MinAgeDelay", DynamicMaximum = "MaxAge")]
        [Display(ResourceType = typeof(Resource1), Prompt = "UserAgePrompt")]
        [Format(typeof(Resource1), "UserAgePrefix", null, null, ClientFormat = "n")]
        public float? Age { get; set; }

        public float? MinAgeDelay { get; set; }

        public float? MinAge { get; set; }

        public float? MaxAge { get; set; }

        [Format(Prefix="Date of birth is: ", ClientFormat = "d", NullDisplayText="No date of birth available")]
        [Display(Prompt="Insert date of birth")]
        [DateRange(SMinimum = "Today", SMaximum = "2040-01-01")]
        public DateTime? BirthDate { get; set; }

        [Format(Prefix = "Date of birth is: ", ClientFormat = "d", NullDisplayText = "No date of birth available")]
        [Display(Prompt = "Insert date of birth")]
        [DateRange(SMinimum = "1960-10-10", SMaximum = "2050-01-01")]
        public DateTime? BirthDate1 { get; set; }
        
        
        
    }
    public class PersonalInfosExt:PersonalInfos
    {
        [DisplayName("Home Adress")]
        public string Adress { get; set; }
        
    }

   
}