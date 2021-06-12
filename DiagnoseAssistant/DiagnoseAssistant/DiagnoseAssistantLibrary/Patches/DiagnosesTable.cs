using HarmonyLib;
using Lopital;

using UnityEngine.UI;

namespace DiagnoseAssistant.Patches
{
    [HarmonyPriority(Priority.Normal)]
    [HarmonyPatch(typeof(DiagnosesTable), "FillData")]
    public static class DiagnosesTable_FillData
    {
        public static void Postfix(DiagnosesTable __instance)
        { 
            UnityEngine.Debug.LogError("postfixrun");
            
            // TableController component = __instance.GetComponent<TableController>();
            foreach (PossibleDiagnosis possibleDiagnosis in __instance.m_diagnosesToSort)
            {
                UnityEngine.Debug.LogError(StringTable.GetInstance().GetLocalizedText((DatabaseEntry) possibleDiagnosis.m_diagnosis.Entry));
            }

            // ((Graphic) component.GetItem(1, 1).m_gameObject.GetComponentInChildren<Image>())
            //     .color = UISettings.Instance.SYMPTOM_COLOR_HAZARD_HIGH;
            
        }
    }
}