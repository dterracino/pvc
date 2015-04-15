﻿using Common.Logging;
using Newtonsoft.Json.Linq;
using NuGet;
using PvcCore;
using ScriptCs.Hosting.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pvc.CLI.Commands
{
    public class InstallCommand : CommandBase
    {
        internal override bool IsTopLevel
        {
            get { return true; }
        }

        internal override string[] Names
        {
            get { return new[] { "install" }; }
        }

        internal override string Description
        {
            get { return "Install a plugin or NuGet package"; }
        }

        internal override void Execute(string[] args, Dictionary<string, string> flags)
        {
            var console = new ScriptCs.Hosting.ScriptConsole();
            var logger = new PvcConsoleLogger();
            var services = new PvcScriptServicesBuilder(console, logger);
            
            var initServices = new ScriptCs.Hosting.InitializationServices(services.InitializationServices.Logger);

            var installationProvider = services.InitializationServices.GetInstallationProvider();
            installationProvider.Initialize();

            var packageInstaller = services.InitializationServices.GetPackageInstaller();
            var packageAssemblyResolver = services.InitializationServices.GetPackageAssemblyResolver();

            if (args.Length > 1)
            {
                var packageName = args[1];
                var packageVersion = "";
                if (flags.ContainsKey("version"))
                    packageVersion = flags["version"];

                var packageRef = new ScriptCs.PackageReference(packageName, VersionUtility.ParseFrameworkName("net45"), packageVersion);
                packageInstaller.InstallPackages(new[] { packageRef }, true);
            }
            else
            {
                var packages = packageAssemblyResolver.GetPackages(Directory.GetCurrentDirectory());
                packageInstaller.InstallPackages(packages);
            }

            packageAssemblyResolver.SavePackages();
        }
    }
}
