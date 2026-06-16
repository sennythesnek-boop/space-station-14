using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Roles;

/// <summary>
/// Sent server -> client to replicate the admin-editable runtime job requirement override, so the client
/// lobby evaluates job availability with the same requirements the server enforces.
/// The payload is the override serialized as JSON (empty string = no override / disabled).
/// </summary>
public sealed class MsgRoleRequirementOverride : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public string Data = string.Empty;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        Data = buffer.ReadString();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Data);
    }
}
