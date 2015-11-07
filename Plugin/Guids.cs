// Guids.cs
// MUST match guids.h
using System;

namespace XamlMagic.Plugin
{
    public static class GuidList
    {
        public const string GuidXamlMagicPackagePkgString = "a224be3c-88d1-4a57-9804-181dbef68021";
        public const string GuidXamlMagicPackageCmdSetString = "83fc41d5-eacb-4fa8-aaa3-9a9bdd5f6407";
        public static readonly Guid GuidXamlMagicPackageCmdSet = new Guid(GuidXamlMagicPackageCmdSetString);
    };
}