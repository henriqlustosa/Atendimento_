namespace Hspm.CadEncaminhamento.Domain
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T instance);
    }
}
