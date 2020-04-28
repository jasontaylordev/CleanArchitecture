using System;
using System.Linq;
using CleanArchitecture.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http.Extensions;
using System.Web;
using System.Collections.Generic;

namespace CleanArchitecture.WebUI.Attributes
{
    public class PagedResponseAttribute : Attribute, IActionFilter
    {
        private PaginationQuery _queryParameters;
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _queryParameters = (PaginationQuery) context.ActionArguments.Values.FirstOrDefault(x => x.GetType() == typeof(PaginationQuery));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result = ((ObjectResult)context.Result).Value as IEnumerable<object>;

            var uriBuilder = new UriBuilder(context.HttpContext.Request.GetDisplayUrl()); 
            var queryStrings = HttpUtility.ParseQueryString(uriBuilder.Query);

            string nextPage = null, previousPage = null;

            if (_queryParameters.PageNumber >= 1 && result.Any())
            {
                queryStrings.Set("PageNumber", (_queryParameters.PageNumber + 1).ToString());
                uriBuilder.Query = queryStrings.ToString();
                nextPage = uriBuilder.Uri.ToString();
            }
            
            if(_queryParameters.PageNumber - 1 >= 1)
            {
                queryStrings.Set("PageNumber", (_queryParameters.PageNumber - 1).ToString());
                uriBuilder.Query = queryStrings.ToString();
                previousPage  = uriBuilder.Uri.ToString();
            }

            var finalResponse = new PaginationResponse
            {
                Data = result,
                PageNumber = _queryParameters.PageNumber >= 1 ? _queryParameters.PageNumber : (int?)null,
                PageSize = _queryParameters.PageSize >= 1 ? _queryParameters.PageSize : null,
                PreviousPage = previousPage,
                NextPage =   nextPage
            };

            context.Result = new OkObjectResult(finalResponse);
        }
    }
}
