using Stubbery;

namespace Payment.Bank.Stub
{
    public class BankApiStub
    {
        public static ApiStub StartStub()
        {
            var stub = new ApiStub();

            stub.Post(
                "/Payment",
                (req, args) => "Test Response");

            stub.Start();

            return stub;
        }
    }
}
