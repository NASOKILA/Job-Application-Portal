namespace JobApplicationPortal.Backend.Responses
{
    public class JobApplicationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; }
    }

    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }

}
