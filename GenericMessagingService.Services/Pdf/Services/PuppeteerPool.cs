﻿using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Pdf.Services
{
    public interface IPuppeteerPool
    {
        Task<PuppeteerPool.PooledBrowser> GetBrowser();
    }

    public class PuppeteerPool : IPuppeteerPool
    {
        private List<PooledBrowser> availableBrowsers = new List<PooledBrowser>();
        private List<PooledBrowser> takenBrowsers = new List<PooledBrowser>();
        private int currentBrowserCount => availableBrowsers.Count + takenBrowsers.Count;

        public async Task<PooledBrowser> GetBrowser()
        {
            if (availableBrowsers.Any())
            {
                var browser = availableBrowsers.First();
                availableBrowsers.Remove(browser);
                takenBrowsers.Add(browser);
                return browser;
            }
            var newBrowser = await StartBrowserAsync();
            takenBrowsers.Add(newBrowser);
            return newBrowser;
        }

        private void ReleaseBrowser(PooledBrowser browser)
        {
            takenBrowsers.Remove(browser);
            availableBrowsers.Add(browser);
        }

        private async Task<PooledBrowser> StartBrowserAsync()
        {
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            return new PooledBrowser(this, browser);
        }

        public class PooledBrowser : IDisposable
        {
            private readonly PuppeteerPool pool;
            private readonly IBrowser browser;

            public PooledBrowser(PuppeteerPool pool, IBrowser browser)
            {
                this.pool = pool;
                this.browser = browser;
            }

            public IBrowser Browser => browser;

            public void Dispose() 
            {
                pool.ReleaseBrowser(this);
            }
        }
    }
}
