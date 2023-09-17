﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Microsoft.VisualBasic.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Support.UI;
using Quartz;
using Path = System.IO.Path;

namespace AutoAppointmenter.Model; 

public class AerobicsJob : IJob {
    
    public Task Execute(IJobExecutionContext context) {
        return Task.Run(() => {
            AppointmenterScheduler.driver.FindElement(By.CssSelector("body > div.ui-page.ui-body-bbb.ui-page-active > div.ca.ui-content.ui-body-bbb > div:nth-child(2) > p:nth-child(5) > a > span")).Click();
            AppointmenterScheduler.driver.FindElement(By.CssSelector("body > div.ui-page.ui-body-bbb.ui-page-active > div.ca.ui-content.ui-body-bbb > p:nth-child(1) > a > span")).Click();
            AppointmenterScheduler.driver.FindElement(By.CssSelector("#datepicker > div > div > a.ui-datepicker-next.ui-corner-all")).Click();
            AppointmenterScheduler.driver.FindElements(By.CssSelector("#datepicker > div > table > tbody a"))[^1].Click();
            var elements = AppointmenterScheduler.driver.FindElements(By.CssSelector("#listcontainer li"));
            var index = elements.IndexOf(elements.First(e => e.FindElement(By.CssSelector("div > h3")).Text == "ダンス初級")) + 1;
            AppointmenterScheduler.driver.ExecuteScript("document.querySelector(\"#listcontainer li:nth-child(" + index + ") > p > span > button\").click();");
            
            var tds = AppointmenterScheduler.driver
                .FindElement(By.CssSelector("#main > dl > dd > div:nth-child(12) > fieldset > fieldset > div > table > tbody > tr:nth-child(3)"))
                .FindElements(By.CssSelector("td"));
            
            // var index2 = tds.IndexOf(tds.Where(e => tds.IndexOf(e) % 2 == 0).First(e => {
            //     int number = IntegerType.FromString(e.FindElement(By.CssSelector("div > label > span > span")).Text);
            //     return 11 <= number && number <= 13;
            // })) + 1;
            
            Random rand = new Random();
            int? index2 = null;

            while (index2 == null)
            {
                int targetNumber = rand.Next(11, 14);

                var matchedElement = tds
                    .Where(e => tds.IndexOf(e) % 2 == 0)
                    .FirstOrDefault(e => int.Parse(e.FindElement(By.CssSelector("div > label > span > span")).Text) == targetNumber);

                if (matchedElement != null)
                {
                    index2 = tds.IndexOf(matchedElement) + 1;
                }
            }
            
            AppointmenterScheduler.driver.ExecuteScript("document.querySelector(\"#main > dl > dd > div:nth-child(12) > fieldset > fieldset > div > table > tbody > tr:nth-child(3) > td:nth-child(" + index2 + ") > div > label\").click();");
            
            AppointmenterScheduler.driver.FindElement(By.Id("confirmsubmit")).Click();
        });
    }
}