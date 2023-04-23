using SensapexCs;

class Program
{
    static void Main(string[] args)
    {
        UmSdkInfo InfoObj = UmSdk.LibUmInfo();

        Console.WriteLine($"libum SDK version:\t\t{InfoObj.UmsdkVersion}");
        Console.WriteLine($"SensapexCS SDK version:\t\t{InfoObj.SensapexCsVersion}");
        Console.WriteLine($"SensapexCS SDK location:\t{InfoObj.SdkLocation}");
    }
}
