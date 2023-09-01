using DSharpPlus.Entities;

namespace Badger.Classes
{
    public class TimerMessage
    {
        public Timer Timer { get; set; }
        public DiscordMessage Message { get; set; }
        public TimerMessage(DiscordMessage message)
        {
            Message = message;
            Timer = new Timer(TimerMessageCallback, this, 600000, 0);
        }
        private void TimerMessageCallback(object? obj)
        {
            TimerMessage timerMessage = (TimerMessage)obj;
            timerMessage.Message.DeleteAsync().ConfigureAwait(false);
            timerMessage.Timer.Dispose();
        }
    }
}
