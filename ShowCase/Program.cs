using SensapexCs;

class Program
{
    static void Main(string[] args)
    {
        UmSdkInfo InfoObj = UmSdk.LibUmInfo();
        Console.WriteLine($"SensapexCS SDK version: {InfoObj.Version}");
    }
}
