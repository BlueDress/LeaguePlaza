namespace LeaguePlaza.Common.Constants
{
    public static class ErrorConstants
    {
        public const string FailedAt = "Failed at {method name}";
        public const string ErrorMessage = "Error message: {message}";

        public const string DecimalModelBinderErrorMessage = "The provided value {value} is not in correct decimal format";

        public const string ValidateImageFileSignatureErrorMessage = "File type is not allowed";

        public const string MaxFileSizeErrorMessage = "Trying to upload file with size {file length}";

        public const string GenericErrorMessage = "Something went wrong";
    }
}
