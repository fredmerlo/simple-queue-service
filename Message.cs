namespace Simple.Queue
{
    public delegate void MessageProcessorDelegate(Account account);

    public enum RunState { Started = 1, Stoped = 2, Paused = 3 };

	public class Message
	{
		private RunState state = 0;
		private Account payload = null;

	    public RunState State
	    {
	        get { return state; }
	        set { state = value; }
	    }

	    public Account Payload
	    {
	        get { return payload; }
	        set { payload = value; }
	    }
	}
}
