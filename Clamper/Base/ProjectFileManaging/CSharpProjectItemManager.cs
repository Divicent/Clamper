﻿#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Clamper.Base.Configuration.Abstract;
using Clamper.Base.Exceptions;
using Clamper.Base.ProcessOutput.Abstract;
using Clamper.Tools;

#endregion

namespace Clamper.Base.ProjectFileManaging
{
    /// <summary>
    /// Manages items of an existing project file
    /// </summary>
    internal static class CSharpProjectItemManager
    {
        private const string ClamperCoreVersion = "1.0.2";


        /// <summary>
        ///  Process the project file using given items
        /// </summary>
        /// <param name="projectFilePath">Path to the project file</param>
        /// <param name="output"></param>
        /// <param name="configuration"></param>
        /// <param name="files"></param>
        public static void Process(string projectFilePath, IProcessOutput output, IConfiguration configuration,
            IEnumerable<string> files)
        {
            try
            {
                if (!File.Exists(projectFilePath))
                {
                    output.WriteInformation("Project file does not exist. creating project file");
                    File.WriteAllText(projectFilePath, $@"<?xml version=""1.0"" encoding=""utf-16""?>
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Clamper"" Version=""${ClamperCoreVersion}"" />
  </ItemGroup>
</Project>");
                    return;
                }

                var document = XmlHelper.GetDocument(projectFilePath);

                XmlNode projectTag = null;
                try
                {
                    projectTag = document.GetElementsByTagName("Project")[0];
                }
                catch (Exception)
                {
                    /* Ignored */
                }

                if (projectTag == null)
                {
                    output.WriteWarning("Project file seems to be invalid, could not find project tag");
                    return;
                }

                var isStandard = false;

                var sdkAttribute = projectTag.Attributes["Sdk"];
                if (sdkAttribute != null && sdkAttribute.Value == "Microsoft.NET.Sdk")
                    isStandard = true;

                if (isStandard)
                {
                    var packages = document.GetElementsByTagName("PackageReference");
                    var requiredPackages = new List<(string name, string version)>
                    {
                        (name: "Clamper", version: ClamperCoreVersion)
                    };

                    var existingPackages = new List<(XmlNode node, string requiredVersion)>();
                    var packagesToAdd = new List<XmlNode>();

                    foreach (var requiredPackage in requiredPackages)
                    {
                        XmlNode existing = null;
                        foreach (XmlNode package in packages)
                        {
                            if (package.Attributes["Include"].Value == requiredPackage.name)
                            {
                                existing = package;
                            }
                        }

                        if (existing == null)
                        {
                            var node = document.CreateElement("PackageReference", null);

                            var includeAttribute = XmlHelper.CreateAttribute(document, "Include", requiredPackage.name);
                            var versionAttribuete = XmlHelper.CreateAttribute(document, "Version", requiredPackage.version);

                            node.Attributes.Append(includeAttribute);
                            node.Attributes.Append(versionAttribuete);
                            packagesToAdd.Add(node);
                        }
                        else
                        {
                            existingPackages.Add((existing, requiredPackage.version));
                        }
                    }

                    var nodesToRemove = new HashSet<XmlNode>();

                    if (existingPackages.Count > 1)
                    {
                        var nodes = new HashSet<(XmlNode node, string requiredVersion)>();
                        for (var i = 0; i < existingPackages.Count; i++)
                        {
                            var current = existingPackages[i];
                            var removed = false;
                            for (var j = i + 1; j < existingPackages.Count; j++)
                            {
                                var next = existingPackages[j];
                                if (current.node.Attributes["Include"].Value != next.node.Attributes["Include"].Value)
                                    continue;
                                nodesToRemove.Add(next.node);
                                removed = true;
                            }

                            if (!removed)
                                nodes.Add(current);
                        }

                        existingPackages = nodes.ToList();
                    }


                    foreach (var nodeToRemove in nodesToRemove)
                    {
                        var parent = nodeToRemove.ParentNode;
                        parent.RemoveChild(nodeToRemove);
                        if (parent.ChildNodes.Count < 1) parent.ParentNode.RemoveChild(parent);
                    }

                    var parentNode = existingPackages.Any()
                        ? existingPackages.First().node.ParentNode
                        : document.CreateElement("ItemGroup", null);

                    foreach (var expackage in existingPackages)
                        expackage.node.Attributes["Version"].Value = expackage.requiredVersion;

                    foreach (var node in packagesToAdd)
                        parentNode.AppendChild(node);

                    if (!existingPackages.Any())
                        document.DocumentElement.AppendChild(parentNode);
                }
                else
                {
                    var folder = new FileInfo(projectFilePath).Directory;
                    var packageFile = Path.Combine(folder.FullName, "packages.config");
                    if (File.Exists(packageFile))
                    {
                        try
                        {
                            var packageDoc = new XmlDocument();
                            packageDoc.LoadXml(File.ReadAllText(packageFile));
                            var changed = false;

                            var packagesNodes = packageDoc.GetElementsByTagName("packages");
                            if (packagesNodes.Count > 0)
                            {
                                var packagesNode = packagesNodes[0];
                                var found = false;
                                foreach (XmlNode packagesNodeChildNode in packagesNode.ChildNodes)
                                {
                                    var id = packagesNodeChildNode.Attributes["id"]?.Value;
                                    if (id != "Clamper")
                                        continue;
                                    found = true;
                                    var version = packagesNode.Attributes["version"];
                                    if (version == null || version.Value != ClamperCoreVersion)
                                    {
                                        if (version == null)
                                        {
                                            version = XmlHelper.CreateAttribute(packageDoc, "version", ClamperCoreVersion);
                                            packagesNodeChildNode.Attributes.Append(version);
                                        }
                                        version.Value = ClamperCoreVersion;
                                        changed = true;
                                    }
                                    break;
                                }

                                if (!found)
                                {
                                    var newNode = packageDoc.CreateNode(XmlNodeType.Element, "package", null);


                                    var id = XmlHelper.CreateAttribute(packageDoc, "id", "Clamper");
                                    var version = XmlHelper.CreateAttribute(packageDoc, "version", ClamperCoreVersion);
                                    newNode.Attributes.Append(id);
                                    newNode.Attributes.Append(version);
                                    packagesNode.AppendChild(newNode);
                                    changed = true;
                                }
                            }

                            if (changed)
                            {
                                XmlHelper.Save(packageDoc, packageFile);
                            }
                            output.WriteWarning($"Please run 'Install-Package Clamper -Version {ClamperCoreVersion}' on the target library if you have not installed the package.");
                        }
                        catch (Exception)
                        {
                            /*Ignore*/
                        }
                    }
                    var compiles = document.GetElementsByTagName("Compile");
                    var torRemove = (from XmlNode compile in compiles
                                     where
                                         compile?.Attributes != null
                                     let include = compile.Attributes["Include"]?.Value
                                     where !string.IsNullOrWhiteSpace(include) &&
                                           (include.StartsWith("Dapper") || include.StartsWith("Infrastructure"))
                                     select compile).ToList();

                    foreach (var tr in torRemove)
                    {
                        var parent = tr.ParentNode;
                        parent.RemoveChild(tr);
                        if (parent.ChildNodes.Count < 1) parent.ParentNode.RemoveChild(parent);
                    }

                    var root = document.CreateElement("ItemGroup", null);
                    foreach (var file in files)
                    {
                        var node = document.CreateElement("Compile", null);
                        var include = XmlHelper.CreateAttribute(document, "Include", file.Replace("/", "\\"));
                        node.Attributes.Append(include);
                        root.AppendChild(node);
                    }

                    document.DocumentElement.AppendChild(root);
                }

                XmlHelper.Save(document, projectFilePath);
            }
            catch (Exception e)
            {
                throw new ClamperException("Unable to process file.", e);
            }
        }
    }
}