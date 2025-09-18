using System.Collections.Generic;

namespace Hspm.CadEncaminhamento.Domain
{
    public sealed class ValidationResult
    {
        private readonly IList<string> _errors = new List<string>();

        public bool IsValid
        {
            get { return _errors.Count == 0; }
        }

        public IList<string> Errors
        {
            get { return _errors; }
        }

        public void Add(string message)
        {
            if (!string.IsNullOrEmpty(message)) _errors.Add(message);
        }
    }
}
