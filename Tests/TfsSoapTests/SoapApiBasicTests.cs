﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace TfsSoapTests
{
    [TestClass]
    public class SoapApiBasicTests
    {
        [TestMethod]
        public void StartAndShutdown()
        {
            using (var executor = new DevOpsSoapInterface.SoapExecutor())
            {
                Assert.AreEqual("running", executor.Status);

                string response = executor.Ping();
                Assert.AreEqual("Pong!", response);

                executor.Shutdown();
                Thread.Sleep(500);

                Assert.AreEqual("stopped", executor.Status);
            }
        }
    }
}