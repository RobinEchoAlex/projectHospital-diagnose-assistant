using System;
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
                        symptomTimes[symptomRef] = symptomTimes[symptomRef] + 1;
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

            TableController component = null;
            try
            {
                component = __instance.GetComponent<TableController>();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("component cannot be fetched"+e);
            }

            MedicalCondition medicalCondition =
                __instance.m_patient.GetComponent<BehaviorPatient>().m_state.m_medicalCondition;
            int rowIndex = 0;
            foreach (PossibleDiagnosis possibleDiagnosis in __instance.m_diagnosesToSort)
            {
                GameDBMedicalCondition diagnoseEntry = possibleDiagnosis.m_diagnosis.Entry;
                TableItemIconList tableItemIconList = (TableItemIconList) component.GetItem(rowIndex, 5);
                UnityEngine.Debug.Log(tableItemIconList.m_icons.Length);
                rowIndex++;

                int visibleCount = 0;
                for (int index = 0; index < diagnoseEntry.Symptoms.Length; ++index)
                {
                    if (medicalCondition.HasVisibleSymptom(diagnoseEntry.Symptoms[index].GameDBSymptomRef.Entry))
                    {
                        visibleCount++;
                        UnityEngine.Debug.Log("Sym"+ StringTable.GetInstance()
                            .GetLocalizedText((DatabaseEntry) diagnoseEntry.Symptoms[index]
                                .GameDBSymptomRef.Entry)+"is visible");
                    }
                    UnityEngine.Debug.Log("Visible symptom"+visibleCount);
                }

                for (int index = 0; index < diagnoseEntry.Symptoms.Length; ++index)
                {
                    if (medicalCondition.HasVisibleSymptom(diagnoseEntry.Symptoms[index].GameDBSymptomRef.Entry))
                    {
                        visibleCount--;
                    }
                    else
                    {
                        int displayPos = visibleCount + index;
                        if (distinctSymptomDict.ContainsKey(diagnoseEntry.Symptoms[index].GameDBSymptomRef.Entry))
                        {
                            tableItemIconList.m_icons[displayPos].GetComponentInChildren<Image>().color = Color.blue;
                            tableItemIconList.m_icons[displayPos].GetComponent<IconController>().Update();
                            UnityEngine.Debug.Log("Set distinct Symptom at pos" + displayPos +
                                                  StringTable.GetInstance()
                                                      .GetLocalizedText((DatabaseEntry) diagnoseEntry.Symptoms[index]
                                                          .GameDBSymptomRef.Entry) +
                                                  " to blue");
                        }
                        if (diagnoseEntry.Symptoms[index].GameDBSymptomRef.Entry.Hazard == SymptomHazard.High)
                        {
                            tableItemIconList.m_icons[displayPos].GetComponentInChildren<Image>().color = Color.red;
                            tableItemIconList.m_icons[displayPos].GetComponent<IconController>().Update();
                            UnityEngine.Debug.Log("Set symptom  at pos" + displayPos +
                                                  StringTable.GetInstance()
                                                      .GetLocalizedText((DatabaseEntry) diagnoseEntry.Symptoms[index]
                                                          .GameDBSymptomRef.Entry) +
                                                  " to red");
                        }
                    }
                }
            }

            // ((Graphic) component.GetItem(1, 1).m_gameObject.GetComponentInChildren<Image>())
            //     .color = UISettings.Instance.SYMPTOM_COLOR_HAZARD_HIGH;
        }
    }
}