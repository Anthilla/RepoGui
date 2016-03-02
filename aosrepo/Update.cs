using System;
using System.Collections.Generic;
using System.Linq;

namespace aosrepo {
    public class UpdateManagement {
        public static IEnumerable<KeyValuePair<string, string>> GetUpdateInfo(string context, string currentVersion) {
            if (context == "kernel") {
                var kernelRepo = Repository.GetByName(context);
                var firmwareFile = kernelRepo.Files.FirstOrDefault(_ => _.FileName.Contains("DIR_lib64_firmware"));
                var modulesFile = kernelRepo.Files.FirstOrDefault(_ => _.FileName.Contains("DIR_lib64_modules"));
                var sysmapFile = kernelRepo.Files.FirstOrDefault(_ => _.FileName.Contains("System.map-genkernel"));
                var initramfsFile = kernelRepo.Files.FirstOrDefault(_ => _.FileName.Contains("initramfs-genkernel"));
                var kernelFile = kernelRepo.Files.FirstOrDefault(_ => _.FileName.Contains("kernel-genkernel"));
                var xenFile = kernelRepo.Files.FirstOrDefault(_ => _.FileName.Contains("xen-"));
                var modnewestDate = Convert.ToInt32(modulesFile.Order);
                var modcurrentDate = Convert.ToInt32(currentVersion);
                string modupdate;
                string modupdateMessage;
                if (modcurrentDate == modnewestDate) {
                    modupdate = "false";
                    modupdateMessage = "your version is up to date";
                }
                else if (modcurrentDate < modnewestDate) {
                    modupdate = "true";
                    modupdateMessage = "there's a newest version available";
                }
                else if (modcurrentDate > modnewestDate) {
                    modupdate = "false";
                    modupdateMessage = "your version is newer than the last version on this repository";
                }
                else {
                    modupdate = "false";
                    modupdateMessage = "";
                }
                return new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("request-context", context),
                    new KeyValuePair<string, string>("request-version", currentVersion),
                    new KeyValuePair<string, string>("version", modulesFile.Order),
                    new KeyValuePair<string, string>("update", modupdate),
                    new KeyValuePair<string, string>("update-message", modupdateMessage),
                    new KeyValuePair<string, string>("channel", modulesFile.Type),
                    new KeyValuePair<string, string>("firmware-guid", firmwareFile.Guid),
                    new KeyValuePair<string, string>("firmware-hash", firmwareFile.ShaSum),
                    new KeyValuePair<string, string>("firmware-url", $"/download/{firmwareFile.Guid}/{firmwareFile.FileName}"),
                    new KeyValuePair<string, string>("modules-guid", modulesFile.Guid),
                    new KeyValuePair<string, string>("modules-hash", modulesFile.ShaSum),
                    new KeyValuePair<string, string>("modules-url", $"/download/{modulesFile.Guid}/{modulesFile.FileName}"),
                    new KeyValuePair<string, string>("sysmapFile-guid", sysmapFile.Guid),
                    new KeyValuePair<string, string>("sysmapFile-hash", sysmapFile.ShaSum),
                    new KeyValuePair<string, string>("sysmapFile-url", $"/download/{sysmapFile.Guid}/{sysmapFile.FileName}"),
                    new KeyValuePair<string, string>("initramfs-guid", initramfsFile.Guid),
                    new KeyValuePair<string, string>("initramfs-hash", initramfsFile.ShaSum),
                    new KeyValuePair<string, string>("initramfs-url", $"/download/{initramfsFile.Guid}/{initramfsFile.FileName}"),
                    new KeyValuePair<string, string>("kernel-guid", kernelFile.Guid),
                    new KeyValuePair<string, string>("kernel-hash", kernelFile.ShaSum),
                    new KeyValuePair<string, string>("kernel-url", $"/download/{kernelFile.Guid}/{kernelFile.FileName}"),
                    new KeyValuePair<string, string>("xen-guid", xenFile.Guid),
                    new KeyValuePair<string, string>("xen-hash", xenFile.ShaSum),
                    new KeyValuePair<string, string>("xen-url", $"/download/{xenFile.Guid}/{xenFile.FileName}"),
                };
            }
            var repo = Repository.GetByName(context);
            var newestFile = repo.Files.FirstOrDefault();
            var newestDate = Convert.ToInt32(newestFile.Order);
            var currentDate = Convert.ToInt32(currentVersion);
            string update;
            string updateMessage;
            if (newestDate == currentDate) {
                update = "false";
                updateMessage = "your version is up to date";
            }
            else if (currentDate < newestDate) {
                update = "true";
                updateMessage = $"there's a newest version available: {newestFile.Order}";
            }
            else if (currentDate > newestDate) {
                update = "false";
                updateMessage = "your version is newer than the last version on this repository";
            }
            else {
                update = "false";
                updateMessage = "";
            }
            return new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("request-context", context),
                new KeyValuePair<string, string>("request-version", currentVersion),
                new KeyValuePair<string, string>("version", newestFile.Order),
                new KeyValuePair<string, string>("update", update),
                new KeyValuePair<string, string>("update-message", updateMessage),
                new KeyValuePair<string, string>("guid", newestFile.Guid),
                new KeyValuePair<string, string>("hash", newestFile.ShaSum),
                new KeyValuePair<string, string>("channel", newestFile.Type),
                new KeyValuePair<string, string>("url", $"/download/{newestFile.Guid}/{newestFile.FileName}"),
            };
        }
    }
}
