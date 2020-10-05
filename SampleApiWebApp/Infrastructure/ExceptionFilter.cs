﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SampleApiWebApp.Infrastructure
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public virtual Task OnExceptionAsync(ExceptionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context?.Exception == null) return Task.CompletedTask;

            switch (context.Exception)
            {
                case ValidationException validationException:
                    context.Result = HandleValidationException(validationException);
                    break;

                case UnauthorizedAccessException _:
                    context.Result = new ForbidResult();
                    break;
            }

            return Task.CompletedTask;
        }

        private static IActionResult HandleValidationException(ValidationException exception)
        {
            var modelState = new ModelStateDictionary();

            exception.Errors
                .ToList()
                .ForEach(x => modelState.AddModelError(x.PropertyName, x.ErrorMessage));

            return new BadRequestObjectResult(modelState);
        }
    }
}
