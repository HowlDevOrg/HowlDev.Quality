using BenchmarkDotNet.Reports;

namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// Set expectations of the boundaries of the benchmark.
/// </summary>
public class BenchmarkExpectations {
    /// <summary>
    /// Number of nanoseconds expected.
    /// </summary>
    public double? Nanoseconds {
        get; private set {
            if (value < 0) throw new InvalidDataException("Time value can't be negative.");
            field = value;
        }
    }
    /// <summary>
    /// Number of microseconds expected.
    /// </summary>
    public double? Microseconds {
        get => (Nanoseconds is null) ? null : Nanoseconds / 1_000;
        private set => Nanoseconds = (value is null) ? null : value * 1_000;
    }
    /// <summary>
    /// Number of milliseconds expected.
    /// </summary>
    public double? Milliseconds {
        get => (Nanoseconds is null) ? null : Nanoseconds / 1_000_000;
        private set => Nanoseconds = (value is null) ? null : value * 1_000_000;
    }
    /// <summary>
    /// Set the margin of error (which includes upper and lower bounds) 
    /// for the ____Second amount. <br/> This defaults to 1.1.
    /// </summary>
    public double MarginOfError { get; private set; } = 1.1;
    /// <summary>
    /// Number of bytes used from the GC. 
    /// </summary>
    public int? Bytes {
        get; private set {
            if (value < 0) throw new InvalidDataException("Byte value can't be negative.");
            field = value;
        }
    }
    /// <summary>
    /// Size of the compiled code in bytes.
    /// </summary>
    public int? CodeSize {
        get; private set {
            if (value < 0) throw new InvalidDataException("Code Size value can't be negative.");
            field = value;
        }
    }

    private TimeCalculation Variation { get; set; } = TimeCalculation.Range;

#pragma warning disable CS1591
    #region Static functions
    public static BenchmarkExpectations ExpectedNanoseconds(double value) {
        return new BenchmarkExpectations() {
            Nanoseconds = value
        };
    }
    public static BenchmarkExpectations ExpectedMicroseconds(double value) {
        return new BenchmarkExpectations() {
            Microseconds = value
        };
    }
    public static BenchmarkExpectations ExpectedMilliseconds(double value) {
        return new BenchmarkExpectations() {
            Milliseconds = value
        };
    }
    public static BenchmarkExpectations ExpectedNanosecondsLessThan(double value) {
        return new BenchmarkExpectations() {
            Nanoseconds = value,
            Variation = TimeCalculation.LT
        };
    }
    public static BenchmarkExpectations ExpectedMicrosecondsLessThan(double value) {
        return new BenchmarkExpectations() {
            Microseconds = value,
            Variation = TimeCalculation.LT
        };
    }
    public static BenchmarkExpectations ExpectedMillisecondsLessThan(double value) {
        return new BenchmarkExpectations() {
            Milliseconds = value,
            Variation = TimeCalculation.LT
        };
    }
    public static BenchmarkExpectations ExpectedBytes(int value) {
        return new BenchmarkExpectations() {
            Bytes = value
        };
    }
    public static BenchmarkExpectations ExpectedCodeSize(int value) {
        return new BenchmarkExpectations() {
            CodeSize = value
        };
    }
    #endregion
    #region Object methods
    public BenchmarkExpectations WithMarginOfError(double value) {
        MarginOfError = value;
        return this;
    }
    public BenchmarkExpectations WithBytes(int value) {
        Bytes = value;
        return this;
    }
    public BenchmarkExpectations WithNanoseconds(double value) {
        Nanoseconds = value;
        return this;
    }
    public BenchmarkExpectations WithMicroseconds(double value) {
        Microseconds = value;
        return this;
    }
    public BenchmarkExpectations WithMilliseconds(double value) {
        Milliseconds = value;
        return this;
    }
    public BenchmarkExpectations WithNanosecondsLessThan(double value) {
        Nanoseconds = value;
        Variation = TimeCalculation.LT;
        return this;
    }
    public BenchmarkExpectations WithMicrosecondsLessThan(double value) {
        Microseconds = value;
        Variation = TimeCalculation.LT;
        return this;
    }
    public BenchmarkExpectations WithMillisecondsLessThan(double value) {
        Milliseconds = value;
        Variation = TimeCalculation.LT;
        return this;
    }
    public BenchmarkExpectations WithCodeSize(int value) {
        CodeSize = value;
        return this;
    }
#pragma warning restore CS1591

    internal void IsValidTime(string method, double result) {
        if (Nanoseconds is not null) {
            if (Variation == TimeCalculation.Range) {
                double bound1 = (double)Nanoseconds * MarginOfError;
                double inverse = 1 / MarginOfError;
                double bound2 = (double)Nanoseconds * inverse;
                // In case someone puts a value less than 1: 
                if (bound2 > bound1) {
                    (bound2, bound1) = (bound1, bound2);
                }

                if (result > bound1 || result < bound2) {
                    throw new BenchmarkException(method, result, bound1, bound2);
                }
            } else {
                if (result >= Nanoseconds) {
                    throw new BenchmarkException(method, result, (double)Nanoseconds, 0);
                }
            }
        }
    }

    internal void IsValidBytes(string method, double result) {
        if (Bytes is null) throw new InvalidDataException("Bytes needs to be checked before getting here.");
        if (result != Bytes) {
            throw new BenchmarkException(method, (int)Bytes, result, true);
        }
    }

    internal void IsValidCodeSize(string method, double result) {
        if (CodeSize is null) throw new InvalidDataException("Code Size needs to be checked before getting here.");
        if (result != CodeSize) {
            throw new BenchmarkException(method, (int)CodeSize, result, false);
        }
    }

    /// <summary>
    /// Takes in a Report and checks it against the inner values. Throws one 
    /// <c>BenchmarkException</c> if either the tests fail. 
    /// </summary>
    /// <exception cref="BenchmarkException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    internal void Report(BenchmarkReport report, string className) {
        BenchmarkException? e1 = null;
        BenchmarkException? e2 = null;
        BenchmarkException? e3 = null;
        string method = className + "." + report.BenchmarkCase.Descriptor.WorkloadMethod.Name;

        try {
            IsValidTime(method, report.ResultStatistics!.Mean);
        } catch (BenchmarkException ex) {
            e1 = ex;
        }

        try {
            if (Bytes is not null) {
                double actBytes;
                try {
                    actBytes = report.Metrics["Allocated Memory"].Value;
                } catch (KeyNotFoundException) {
                    throw new BenchmarkException($"{report.BenchmarkCase.ToString().Split(':')[0]}: Did not find a memory diagnoser.");
                }

                IsValidBytes(method, actBytes);
            }
        } catch (BenchmarkException ex) {
            e2 = ex;
        }

        try {
            if (CodeSize is not null) {
                double actCodeSize;
                try {
                    actCodeSize = report.Metrics["Native Code Size"].Value;
                } catch (KeyNotFoundException) {
                    throw new BenchmarkException($"{report.BenchmarkCase.ToString().Split(':')[0]}: Did not find a disassembly diagnoser.");
                }

                IsValidCodeSize(method, actCodeSize);
            }
        } catch (BenchmarkException ex) {
            e3 = ex;
        }

        if (e1 is not null || e2 is not null || e3 is not null) throw BenchmarkException.Combine(e1, e2, e3);
    }
    #endregion

    private enum TimeCalculation {
        Range,
        LT
    }
}
