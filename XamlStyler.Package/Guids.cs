// Guids.cs
// MUST match guids.h
using System;

namespace XamlStyler.Plugin
{
    public static class GuidList
    {
        public const string GuidXamlStylerPackagePkgString = "a224be3c-88d1-4a57-9804-181dbef68021";
        public const string GuidXamlStylerPackageCmdSetString = "83fc41d5-eacb-4fa8-aaa3-9a9bdd5f6407";
        public static readonly Guid GuidXamlStylerPackageCmdSet = new Guid(GuidXamlStylerPackageCmdSetString);
    };
}