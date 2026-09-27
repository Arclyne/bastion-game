using Xunit;

// Switching the language changes process-wide culture settings, so client tests must not run in parallel.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
