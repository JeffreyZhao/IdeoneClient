namespace IdeoneClient
{
    public enum SubmissionResult
    {
        NotRunning = 0,
        CompilationError = 11,
        RuntimeError = 12,
        TimeLimitExceeded = 13,
        Success = 15,
        MemoryLimitExceeded = 17,
        IllegalSystemCall = 19,
        InternalError = 20
    }
}
