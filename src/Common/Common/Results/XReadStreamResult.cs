namespace Common.Results;

public record XReadStreamResult(string StreamKey, List<StreamDataResult>? Data);
