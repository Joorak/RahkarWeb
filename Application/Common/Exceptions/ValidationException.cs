﻿namespace Application.Exceptions; 
public class ValidationException : Exception {
    public ValidationException()
    : base("One or more validation failures have occurred")
    {
        this.Errors = new Dictionary<string, string?[]>();
    }

   
    //public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    //    : this()
    //{
    //    this.Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage).ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    //}

    public IDictionary<string, string?[]> Errors { get; }
} 
