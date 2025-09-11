using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvionicConverter.STAPServer.Messages;

public class GpsDataMessage(GpsData data) : ValueChangedMessage<GpsData>(data);
