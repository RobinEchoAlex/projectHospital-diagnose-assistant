using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Lopital;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace DiagnoseAssistant.Patches
{
    [HarmonyPriority(Priority.Normal)]
    [HarmonyPatch(typeof(DiagnosesTable), "FillData")]
    public static class DiagnosesTable_FillData
    {
        public static void Postfix(DiagnosesTable __instance)
        {
            UnityEngine.Debug.LogError("postfixrun");

            Dictionary<GameDBSymptom, int> symptomTimes = new Dictionary<GameDBSymptom, int>();
            // TableController component = __instance.GetComponent<TableController>();
            foreach (PossibleDiagnosis possibleDiagnosis in __instance.m_diagnosesToSort)
            {
                GameDBMedicalCondition entry = possibleDiagnosis.m_diagnosis.Entry;
                UnityEngine.Debug.Log(StringTable.GetInstance().GetLocalizedText((DatabaseEntry) entry));
                foreach (var symptom in entry.Symptoms)
                {
                    var symptomRef = symptom.GameDBSymptomRef.Entry;
                    if (!symptomTimes.ContainsKey(symptomRef))
                    {
                        symptomTimes.Add(symptomRef, 1);
                    }
                    else
                    {
                        symptomTimes [symptomRef] = symptomTimes[symptomRef] + 1;
                    }
                }
            }

            var distinctSymptomDict = symptomTimes.Where(s => s.Value == 1)
                .ToDictionary(s => s.Key, s => s.Value);

            foreach (var distinctSymptom in distinctSymptomDict)
            {
                UnityEngine.Debug.Log("distinct Symptom:" +
                                      StringTable.GetInstance().GetLocalizedText(distinctSymptom.Key));
            }

            // ((Graphic) component.GetItem(1, 1).m_gameObject.GetComponentInChildren<Image>())
            //     .color = UISettings.Instance.SYMPTOM_COLOR_HAZARD_HIGH;
        }
    }
}