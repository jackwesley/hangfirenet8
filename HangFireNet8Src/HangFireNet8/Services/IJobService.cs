namespace HangFireNet8.Services
{
    public interface IJobService
    {
        int SumNumber(int numA, int numB);
        public void SendMessage(string message);
        void JobException(string message);
    }
}
