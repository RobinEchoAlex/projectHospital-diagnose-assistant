using System;
using System.Reflection;
using HarmonyLib;

// Note: .NET framework version needs to be set to 3.5
[assembly: AssemblyTitle("diagnose-assistant")] // ENTER MOD TITLE
[assembly: AssemblyFileVersion("1.0.0.0")] // MOD VERSION
[assembly: AssemblyVersion("1.2.22045")] // GAME VERSION
 
// Parent HospitalMod class is inherited also from Mono Behavior, so this gets automatically added as a component
// on its own game object in Unity as soon as the main menu loads
public class ModEntrance : HospitalMod 
{
    void Start()
    {
        Assembly assembly = GetType().Assembly;
        string modName = assembly.GetName().Name;
        string dir = System.IO.Path.GetDirectoryName(assembly.Location);
 
        UnityEngine.Debug.LogWarning("Mod Init: "  + modName + " " + dir);
 
        try
        {
            string idForHarmony = GetType().FullName + "(" + modName + ")";
            UnityEngine.Debug.LogWarning(" Booting up Harmony from " + idForHarmony);
            
            Harmony harmony = new Harmony(idForHarmony);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Harmony patch fail. Error " + ex);
        }
    }
 
    void Update()
    {
 
    }
}