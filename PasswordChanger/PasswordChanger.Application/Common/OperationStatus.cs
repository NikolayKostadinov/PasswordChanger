/// <summary>
/// Summary description for EfStatus
/// </summary>
namespace PasswordChanger.Application.Common
{
    using Application.Contracts;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OperationStatus : IOperationStatus
    {
        private List<ValidationResult> errors;
        private List<string> successMessages;

        private int resultRecordsCount;

        public int ResultRecordsCount
        {
            get { return this.resultRecordsCount; }
            set { this.resultRecordsCount = value; }
        }

        /// <summary>
        /// If there are no errors then it is valid
        /// </summary>
        public bool IsValid { get { return this.errors == null; } }

        public IReadOnlyList<ValidationResult> OperationErrors
        {
            get { return this.errors ?? new List<ValidationResult>(); }
        }

        public IReadOnlyList<string> SuccessMessages
        {
                get { return this.successMessages ?? new List<string>();}
        }

        /// <summary>
        /// This converts the Entity framework errors into Validation Errors
        /// </summary>

        public IOperationStatus SetErrors(IEnumerable<ValidationResult> errors)
        {
            this.errors = errors.ToList();
            return this;
        }

        public IOperationStatus SetSuccessMessages(IEnumerable<string> successMessages)
        {
            this.successMessages = successMessages.ToList();
            return this;
        }
    }
}
