using System;
using System.Collections.Generic;
using System.Net;
using FluentValidation.Extensions;
using FluentValidation.Results;

namespace SampleApiWebApp.Infrastructure
{
    public class OperationResult
    {
        public bool IsSuccess { get; protected set; }

        public HttpStatusCode Status { get; protected set; }

        public IDictionary<string, IEnumerable<string>> Errors { get; protected set; }

        public static OperationResult Success()
        {
            return new OperationResult()
            {
                IsSuccess = true,
                Status = HttpStatusCode.OK
            };
        }

        public static OperationResult<T> Success<T>(T data)
        {
            return new OperationResult<T>(data)
            {
                IsSuccess = true,
                Status = HttpStatusCode.OK
            };
        }

        public static OperationResult Fail(IDictionary<string, IEnumerable<string>> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            return new OperationResult()
            {
                IsSuccess = false,
                Status = HttpStatusCode.BadRequest,
                Errors = errors
            };
        }

        public static OperationResult<T> Fail<T>(IDictionary<string, IEnumerable<string>> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            return new OperationResult<T>(default(T))
            {
                IsSuccess = false,
                Status = HttpStatusCode.BadRequest,
                Errors = errors
            };
        }

        public static OperationResult Fail(IEnumerable<ValidationFailure> validationErrors)
        {
            if (validationErrors == null) throw new ArgumentNullException(nameof(validationErrors));

            return Fail(validationErrors.GetErrors());
        }

        public static OperationResult<T> Fail<T>(IEnumerable<ValidationFailure> validationErrors)
        {
            if (validationErrors == null) throw new ArgumentNullException(nameof(validationErrors));

            return Fail<T>(validationErrors.GetErrors());
        }

        public static OperationResult Fail(string errorMessage)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            var errors = new Dictionary<string, IEnumerable<string>>()
            {
                { string.Empty, new string[] { errorMessage } }
            };

            return Fail(errors);
        }

        public static OperationResult<T> Fail<T>(string errorMessage)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            var errors = new Dictionary<string, IEnumerable<string>>()
            {
                { string.Empty, new string[] { errorMessage } }
            };

            return Fail<T>(errors);
        }

        public static OperationResult NotFound()
        {
            return new OperationResult
            {
                IsSuccess = false,
                Status = HttpStatusCode.NotFound
            };
        }

        public static OperationResult<T> NotFound<T>()
        {
            return new OperationResult<T>(default(T))
            {
                IsSuccess = false,
                Status = HttpStatusCode.NotFound
            };
        }
    }

#pragma warning disable SA1402 // File may only contain a single class
    public class OperationResult<T> : OperationResult
#pragma warning restore SA1402 // File may only contain a single class
    {
        internal OperationResult(T data)
        {
            this.Data = data;
        }

        internal OperationResult(IDictionary<string, IEnumerable<string>> errors)
        {
            this.Errors = errors;
            this.IsSuccess = false;
            this.Status = HttpStatusCode.BadRequest;
        }

        public T Data { get; protected set; }
    }
}
