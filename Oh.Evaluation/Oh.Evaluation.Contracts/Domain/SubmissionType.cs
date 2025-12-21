using System.Runtime.Serialization;

namespace Contracts.Domain;

public enum SubmissionType
{
    [EnumMember(Value = "SINGLE_CHOICE")]
    SingleChoice,

    [EnumMember(Value = "MULTIPLE_CHOICE")]
    MultipleChoice,

    [EnumMember(Value = "TEXT_INPUT")]
    TextInput,

    [EnumMember(Value = "CODE")]
    Code
}