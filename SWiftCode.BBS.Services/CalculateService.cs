using SwiftCode.BBS.IServices;
using SwiftCode.BBS.Repostories;

namespace SwiftCode.BBS.Services
{
    public class CalculateService : ICalculateService
    {
        CalculateRepostory CalculateRepostory = new CalculateRepostory();
        public int Sum(int i, int j)
        {
            return CalculateRepostory.Sum(i, j);
        }
    }
}