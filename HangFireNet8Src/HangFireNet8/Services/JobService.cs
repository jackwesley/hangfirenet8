
namespace HangFireNet8.Services
{
    public class JobService : IJobService
    {

        public int SumNumber(int numA, int numB)
        {
            int result = numA + numB;
            Thread.Sleep(2000);
            return result;
        }

        public void SendMessage(string message)
        {
            Thread.Sleep(2000);
            Console.WriteLine(message);
        }

        public void JobException(string message)
        {
            throw new Exception("Error trying to process");
        }
    }
}
