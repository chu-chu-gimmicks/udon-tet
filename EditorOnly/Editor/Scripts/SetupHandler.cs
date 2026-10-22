using ChuChuGimmicks.UDONTET.Runtime;
using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    public static class SetupHandler
    {
        public static void CreateMains(List<GameObject> mains, int count, GameObject prefab, Transform parent, UdonSharpBehaviour udonChips, float udonChipsRate)
        {
            mains.RemoveAll(x => x == null);

            if (count > mains.Count)
            {
                for (int i = mains.Count; i < count; i++)
                {
                    GameObject main = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                    main.name = i == 0 ? $"UDON TET" : $"UDON TET_{i}";
                    main.transform.localPosition += new Vector3(i * 0.704f, 1.2f, 0.0f);
                    mains.Add(main);
                }
            }
            else if (count < mains.Count)
            {
                for (int i = mains.Count - 1; i >= count; i--)
                {
                    GameObject main = mains[i];
                    mains.RemoveAt(i);
                    Object.DestroyImmediate(main);
                }
            }

            mains.RemoveAll(main => main == null);

            foreach (GameObject main in mains)
            {
                SetUpMain(main, udonChips, udonChipsRate);
            }
        }


        private static void SetUpMain(GameObject mainObj, UdonSharpBehaviour udonChips, float udonChipsRate)
        {
            if (!mainObj.TryGetComponent(out Main main)) { return; }

            SerializedObject so = new SerializedObject(main);
            SerializedProperty udonChipsProp = so.FindProperty(Main.FieldNames.UdonChips);
            SerializedProperty udonChipsRateProp = so.FindProperty (Main.FieldNames.UdonChipsRate);

            so.Update();

            udonChipsProp.objectReferenceValue = udonChips;
            udonChipsRateProp.floatValue = udonChipsRate;

            so.ApplyModifiedProperties();
        }


        public static void CreateStatsBoards(List<GameObject> statsBoards, int count, GameObject prefab, Transform parent)
        {
            statsBoards.RemoveAll(x => x == null);

            if (count > statsBoards.Count)
            {
                for (int i = statsBoards.Count; i < count; i++)
                {
                    GameObject statsBoard = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                    statsBoard.name = i == 0 ? $"Stats Board" : $"Stats Board_{i}";
                    statsBoard.transform.localPosition += new Vector3(-(i +1) * 0.704f, 1.2f, 0.0f);
                    statsBoards.Add(statsBoard);
                }
            }
            else if (count < statsBoards.Count)
            {
                for (int i = statsBoards.Count - 1; i >= count; i--)
                {
                    GameObject statsBoard = statsBoards[i];
                    statsBoards.RemoveAt(i);
                    Object.DestroyImmediate(statsBoard);
                }
            }

            statsBoards.RemoveAll(main => main == null);
        }


        public static GameObject CreateUserDataAccessor(GameObject userDataAccessor, bool usePersistence, GameObject prefab, Transform parent)
        {
            if (userDataAccessor == null)
            {
                if (usePersistence)
                {
                    userDataAccessor = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                    userDataAccessor.name = "User Data Accessor";
                    userDataAccessor.transform.SetAsFirstSibling();
                }
            }
            else
            {
                if (!usePersistence)
                {
                    Object.DestroyImmediate(userDataAccessor);
                    userDataAccessor = null;
                }
            }

            return userDataAccessor;
        }


        public static void RegisterToManager(GameObject managerObj, List<GameObject> mainObjs, List<GameObject> statsBoardObjs, GameObject userDataAccessorObj)
        {
            if (managerObj == null) { return; }
            if (!managerObj.TryGetComponent(out Manager manager)) { return; }

            SerializedObject so = new SerializedObject(manager);
            SerializedProperty mainsProp = so.FindProperty(Manager.FieldNames.Mains);
            SerializedProperty statsBoardsProp = so.FindProperty(Manager.FieldNames.StatsBoards);
            SerializedProperty userDataAccessorProp = so.FindProperty(Manager.FieldNames.UserDataAccessor);

            so.Update();

            if (mainObjs != null)
            {
                mainsProp.ClearArray();
                mainsProp.arraySize = mainObjs.Count;
                for (int i = 0; i < mainObjs.Count; i++)
                {
                    SerializedProperty element = mainsProp.GetArrayElementAtIndex(i);
                    if (mainObjs[i].TryGetComponent(out Main main))
                    {
                        element.objectReferenceValue = main;
                    }
                    else
                    {
                        element.objectReferenceValue = null;
                    }
                }
            }
            else
            {
                mainsProp.objectReferenceValue = null;
            }

            if (statsBoardObjs != null)
            {
                statsBoardsProp.ClearArray();
                statsBoardsProp.arraySize = statsBoardObjs.Count;
                for (int i = 0; i < statsBoardObjs.Count; i++)
                {
                    SerializedProperty element = statsBoardsProp.GetArrayElementAtIndex(i);
                    if (statsBoardObjs[i].TryGetComponent(out StatsBoard statsBoard))
                    {
                        element.objectReferenceValue = statsBoard;
                    }
                    else
                    {
                        element.objectReferenceValue = null;
                    }
                }
            }
            else
            {
                statsBoardsProp.objectReferenceValue = null;
            }

            if (userDataAccessorObj != null)
            {
                if (userDataAccessorObj.TryGetComponent(out UserDataAccessor userDataAccessor))
                {
                    userDataAccessorProp.objectReferenceValue = userDataAccessor;
                }
                else
                {
                    userDataAccessorProp.objectReferenceValue = null;
                }
            }
            else
            {
                userDataAccessorProp.objectReferenceValue = null;
            }

            so.ApplyModifiedProperties();
        }
    }
}
