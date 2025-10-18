namespace Lethal_Battle.codes
{
    internal class ManageChat
    {
        public static void SendChatMessage(string message)
        {
            HUDManager.Instance.AddChatMessage(message);
        }

        public static void ConfirmRestart()
        {
            Plugin.verifying = true;
            SendChatMessage("Are you sure? Type CONFIRM or DENY.");
        }

        public static void AcceptRestart(StartOfRound manager)
        {
            SendChatMessage("Battle confirmed.");
            Plugin.verifying = false;
        }

        public static void DeclineRestart()
        {
            SendChatMessage("Battle aborted.");
            Plugin.verifying = false;
        }
    }
}
