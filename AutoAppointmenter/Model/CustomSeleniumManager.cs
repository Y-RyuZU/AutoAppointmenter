using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using OpenQA.Selenium;

namespace AutoAppointmenter.Model;

public class CustomSeleniumManager {
    private static string binary;

    private static readonly List<string> KnownDrivers = new List<string>() {
        "geckodriver",
        "chromedriver",
        "msedgedriver",
        "IEDriverServer"
    };

    /// <summary>
    /// Determines the location of the correct driver.
    /// </summary>
    /// <param name="driverName">Which driver the service needs.</param>
    /// <returns>
    /// The location of the driver.
    /// </returns>
    public static string DriverPath(string driverName) {
        driverName = driverName.Replace(".exe", "");
        if (!KnownDrivers.Contains(driverName)) {
            throw new WebDriverException("Unable to locate driver with name: " + driverName);
        }

        var binaryFile = Binary;
        if (binaryFile == null) return null;

        var arguments = "--driver " + driverName;
        return RunCommand(binaryFile, arguments);
    }

    /// <summary>
    /// Gets the location of the correct Selenium Manager binary.
    /// </summary>
    private static string? Binary {
        get {
            if (string.IsNullOrEmpty(binary)) {
                string binarySuffix;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    binarySuffix = "selenium-manager/windows/selenium-manager.exe";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    binarySuffix = "selenium-manager/linux/selenium-manager";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    binarySuffix = "selenium-manager/macos/selenium-manager";
                }
                else {
                    throw new WebDriverException("Selenium Manager did not find supported operating system");
                }

                binary = binarySuffix;

                if (!File.Exists(binarySuffix) && AppContext.GetData("NATIVE_DLL_SEARCH_DIRECTORIES") is string dirs) {
                    foreach (var dir in dirs.Split(';')) {
                        var combined = Path.Combine(dir, binarySuffix);
                        if (File.Exists(combined)) {
                            binary = combined;
                            break;
                        }
                    }
                }
            }

            return binary;
        }
    }

    /// <summary>
    /// Executes a process with the given arguments.
    /// </summary>
    /// <param name="fileName">The path to the Selenium Manager.</param>
    /// <param name="arguments">The switches to be used by Selenium Manager.</param>
    /// <returns>
    /// the standard output of the execution.
    /// </returns>
    private static string RunCommand(string fileName, string arguments) {
        Process process = new Process();
        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        StringBuilder outputBuilder = new StringBuilder();
        int processExitCode;

        DataReceivedEventHandler outputHandler = (sender, e) => outputBuilder.AppendLine(e.Data);

        try {
            process.OutputDataReceived += outputHandler;
            process.ErrorDataReceived += outputHandler;

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
        }
        catch (Exception ex) {
            throw new WebDriverException($"Error starting process: {fileName} {arguments}", ex);
        }
        finally {
            processExitCode = process.ExitCode;
            process.OutputDataReceived -= outputHandler;
            process.ErrorDataReceived -= outputHandler;
        }

        string output = outputBuilder.ToString().Trim();

        if (processExitCode != 0) {
            throw new WebDriverException(
                $"Invalid response from process (code {processExitCode}): {fileName} {arguments}\n{output}");
        }

        return output.Replace("INFO\t", "");
    }
}