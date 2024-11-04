namespace JwtLibrary
{
    public interface IJwtSubjectLookup<T>
    {
        Task<T> LookupAsync(string subject);
    }
}
