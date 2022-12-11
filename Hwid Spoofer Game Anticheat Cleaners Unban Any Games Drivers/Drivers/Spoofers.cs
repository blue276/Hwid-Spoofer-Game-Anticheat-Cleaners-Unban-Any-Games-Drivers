using Microsoft.Win32;

namespace LunaR_Spoofer
{
    internal class Spoofers
    {
        public static void Init()
        {
            Console.Title = "LunaR Spoofer by https://github.com/Umbra999";
            Console.WriteLine("RUN THE PROGRAM AS ADMIN TO MAKE IT WORK");
            Console.WriteLine("!!! WARNING THE SPOOF IS PERMANENT ONLY USE IF YOU KNOW WHAT YOU ARE DOING !!!");
            Console.WriteLine("");
            Console.WriteLine("Press Enter to spoof your HWID");
            Console.ReadLine();
            CleanTraces();
            SpoofProductID();
            SpoofProfileGUID();
            SpoofMachineID();
            SpoofMachineGUID();
            //SpoofInstallTime();
            HideSMBios();
            FlushDNS();

            Console.WriteLine("HWID Spoofed, Restart your PC to finish");
            Console.WriteLine("https://github.com/Umbra999");
            Console.ReadLine();
        }

        private static void CleanTraces()
        {
            string LocalLowFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow");
            string RoamingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (Directory.Exists(RoamingFolder + @"/Unity"))
            {
                DirectoryInfo Unity = new(RoamingFolder + @"/Unity");
                Extensions.DeleteDirectory(Unity);
            }

            if (Directory.Exists(LocalLowFolder + @"/Unity"))
            {
                DirectoryInfo Unity = new(LocalLowFolder + @"/Unity");
                Extensions.DeleteDirectory(Unity); ;
            }

            if (Directory.Exists(LocalLowFolder + @"/VRChat"))
            {
                DirectoryInfo VRChat = new(LocalLowFolder + @"/VRChat");
                Extensions.DeleteDirectory(VRChat);
            }

            RegistryKey CurrentUserReg = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            CurrentUserReg.OpenSubKey("Software", true).DeleteSubKeyTree("VRChat", false);
            CurrentUserReg.OpenSubKey("Software", true).DeleteSubKeyTree("Unity", false);
            CurrentUserReg.OpenSubKey("Software", true).DeleteSubKeyTree("Unity Technologies", false);
            CurrentUserReg.Close();
        }

        private static void SpoofProductID()
        {
            RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", true);
            registryKey.SetValue("ProductID", $"{Extensions.RandomNumberString(5)}-{Extensions.RandomNumberString(5)}-{Extensions.RandomNumberString(5)}-{Extensions.RandomString(5)}");
            registryKey.Close();
        }

        private static void HideSMBios()
        {
            Extensions.RunAsProcess("reg add HKLM\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Restrictions /F");
            Extensions.RunAsProcess("reg add HKLM\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Restrictions /v HideMachine /t REG_DWORD /d 1 /F");
            Extensions.RunAsProcess("taskkill /F /IM WmiPrvSE.exe");
        }

        private static void FlushDNS()
        {
            Extensions.RunAsProcess("ipconfig /release");
            Extensions.RunAsProcess("ipconfig /flushdns");
            Extensions.RunAsProcess("ipconfig /renew");
            Extensions.RunAsProcess("ipconfig /flushdns");
            Extensions.RunAsProcess("ping localhost -n 3 >nul");
        }

        private static void SpoofMachineID()
        {;
            RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Microsoft\\SQMClient", true);
            registryKey.SetValue("MachineId", "{" + Guid.NewGuid().ToString().ToUpper() + "}");
        }

        private static void SpoofMachineGUID()
        {
            string value = Guid.NewGuid().ToString();
            RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Microsoft\\Cryptography", true);
            registryKey.SetValue("MachineGuid", value);
        }

        private static void SpoofProfileGUID()
        {
            RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\IDConfigDB\\Hardware Profiles\\0001", true);
            registryKey.SetValue("HwProfileGUID", "{" + Guid.NewGuid().ToString() + "}");
        }

        private static void SpoofInstallTime()
        {
            RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", true);
            long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            registryKey.SetValue("InstallTime", unixTime);
            registryKey.SetValue("InstallDate", (int)unixTime);
            registryKey.Close();
        }
    }
}
