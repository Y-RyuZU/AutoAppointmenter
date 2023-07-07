using System;
using System.IO;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Quartz;

namespace AutoAppointmenter.Model;

public static class AppointmenterScheduler {
    public static ChromeDriverService Service = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(CustomSeleniumManager.DriverPath("chromedriver")));
    public static ChromeDriver driver = new(Service);
    private static IScheduler? scheduler = null;
    public static async Task Add() {
        if(scheduler == null) {
            scheduler = await SchedulerBuilder.Create().BuildScheduler();
            scheduler.Start();
        }
        
        var aerobicsJob = JobBuilder.Create<AerobicsJob>().UsingJobData("UserName", "").Build();
        var zumbaJob = JobBuilder.Create<ZumbaJob>().UsingJobData("UserName", "").Build();
        var aerobicsTrigger = TriggerBuilder.Create().WithCronSchedule("1 25 7 ? * WED *").Build();
        var zumbaTrigger = TriggerBuilder.Create().WithCronSchedule("1 25 7 ? * SAT *").Build();
        var trigger = TriggerBuilder.Create().StartNow().Build();
        
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        driver.Navigate().GoToUrl("https://axtos.e-atoms.jp/AXTOSWebUser/Account/LogIn");
        driver.FindElement(By.Id("UserName")).SendKeys(Settings.Data.Username);
        driver.FindElement(By.Id("Password")).SendKeys(Settings.Data.Password);
        driver.FindElement(By.CssSelector("#main > ul > li:nth-child(1) > input")).Click();
        
        await scheduler.ScheduleJob(zumbaJob, zumbaTrigger);
        await scheduler.ScheduleJob(aerobicsJob, aerobicsTrigger);
    }
}