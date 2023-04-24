using SensapexCs;

class Program
{
    static void Main(string[] args)
    {
        UmSdkInfo InfoObj = UmSdk.LibUmInfo();

        Console.WriteLine();
        Console.WriteLine($"SensapexCs SDK -info");
        Console.WriteLine($"---------------------------------------");
        Console.WriteLine($"SensapexCS SDK version:\t\t{InfoObj.SensapexCsVersion}");
        Console.WriteLine($"Using 'libum' version:\t\t{InfoObj.UmsdkVersion}");
        Console.WriteLine();
    }
}
