public record DeltaResults<T>(
    IEnumerable<T> ChangedValues, 
    string? NextDeltaToken,
    string? NextDeltaLink
);