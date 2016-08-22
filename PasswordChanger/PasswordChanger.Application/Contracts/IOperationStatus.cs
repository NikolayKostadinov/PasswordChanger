namespace PasswordChanger.Application.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Status of current operation
    /// </summary>
    public interface IOperationStatus
    {
        IReadOnlyList<ValidationResult> OperationErrors { get; }
        IReadOnlyList<string> SuccessMessages { get; }
        bool IsValid { get; }
        int ResultRecordsCount { get; set; }
        IOperationStatus SetErrors(IEnumerable<ValidationResult> errors);
        IOperationStatus SetSuccessMessages(IEnumerable<string> successMessages);
    }
}