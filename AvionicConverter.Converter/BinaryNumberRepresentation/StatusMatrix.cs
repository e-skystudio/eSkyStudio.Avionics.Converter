using AvionicConverter.Converter.Helpers;

namespace AvionicConverter.Converter.BinaryNumberRepresentation;

public enum BnrStatusMatrix
{
    FailureWarning   = 0b00, // 0
    NoComputedData   = 0b01, // 1
    FunctionalTest   = 0b10, // 2
    NormalOps        = 0b11  // 3
}