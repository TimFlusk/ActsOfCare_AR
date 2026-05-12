

namespace TrackerPro.Unity
{
  public static class CalculatorGraphConfigExtension
  {
    public static string AddPacketPresenceCalculator(this CalculatorGraphConfig config, string outputStreamName)
    {
      var presenceStreamName = Tool.GetUnusedStreamName(config, $"{outputStreamName}_presence");
      var packetPresenceCalculatorNode = new CalculatorGraphConfig.Types.Node()
      {
        Calculator = "PacketPresenceCalculator"
      };
      packetPresenceCalculatorNode.InputStream.Add($"PACKET:{outputStreamName}");
      packetPresenceCalculatorNode.OutputStream.Add($"PRESENCE:{presenceStreamName}");

      config.Node.Add(packetPresenceCalculatorNode);
      return presenceStreamName;
    }
  }
}
