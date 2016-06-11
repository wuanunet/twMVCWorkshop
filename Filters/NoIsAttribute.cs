using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace twMVCWorkshop.Filters
{
    public sealed class NoIsAttribute : ValidationAttribute, IClientValidatable
    {
        //參考 http://demo.tc/Post/688

        public string Input { get; set; }

        public NoIsAttribute(string input)
        {
            this.Input = input;
        }

        public override bool IsValid(object value)
        {
            //權責分清楚，沒有輸入不算錯
            if (value == null)
            {
                return true;
            }

            if (value is string)
            {
                //輸入值是字串才判斷
                if (this.Input.Contains(value.ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "nois",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };

            //此參數一定要是小寫！
            rule.ValidationParameters["input"] = Input;

            throw new NotImplementedException();
        }
    }
}