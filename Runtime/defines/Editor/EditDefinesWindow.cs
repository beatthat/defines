using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BeatThat.Pools;
using BeatThat.TypeUtil;
using UnityEditor;
using UnityEngine;

namespace BeatThat.Defines
{
    public class EditDefinesWindow : EditorWindow
    {
        private DefineEdits m_defineEdits = new DefineEdits();
        private BuildTargetGroup m_buildTargetGroup;
        private string m_addSymbol;
        private Dictionary<string, TypeAndAttribute[]> m_editDefinesBySymbol = new Dictionary<string, TypeAndAttribute[]>();

        [MenuItem("Window/BeatThat/Edit Defines")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(EditDefinesWindow));
        }

        private string definesCurrent 
        {
            get {
                return PlayerSettings.GetScriptingDefineSymbolsForGroup(m_buildTargetGroup) ?? "";
            }
        }

        private string definesEditted
        {
            get
            {
                return m_defineEdits.ToSymbolString();
            }
        }

        void OnEnable()
        {
            m_buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            m_editDefinesBySymbol.Clear();

            FindEditDefineAttributes(m_editDefinesBySymbol);

            ResetDefines();
        }

        private void ResetDefines()
        {
            m_defineEdits.Clear();

            m_defineEdits.AddSymbols(this.definesCurrent);

            foreach (var alist in m_editDefinesBySymbol.Values)
            {
                var attr = alist[0].attr as EditDefineAttribute;
                m_defineEdits.AddOption(attr.symbol, attr.desc);
            }
        }


        private void FindEditDefineAttributes(Dictionary<string, TypeAndAttribute[]> editDefinesBySymbol)
        {
            var types = TypeUtils.FindTypesWithAttribute<EditDefineAttribute>();
            foreach(var tAndA in types) {
                var attr = tAndA.attr as EditDefineAttribute;

                TypeAndAttribute[] listForSymbol;
                if(!editDefinesBySymbol.TryGetValue(attr.symbol, out listForSymbol)) {
                    editDefinesBySymbol[attr.symbol] = new TypeAndAttribute[] { tAndA };
                    continue;
                }

                var newListForSymbol = new TypeAndAttribute[listForSymbol.Length + 1];
                Array.Copy(listForSymbol, newListForSymbol, listForSymbol.Length);
                newListForSymbol[listForSymbol.Length] = tAndA;
                editDefinesBySymbol[attr.symbol] = new TypeAndAttribute[] { tAndA };
            }
        }

        void OnGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Edit Defines", EditorStyles.boldLabel);
            GUILayout.Label("Discover and set/unset define symbols that alter the behavior of your app and enviroment.",
                            EditorStyles.wordWrappedLabel);


            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Defines Editted", EditorStyles.miniBoldLabel, GUILayout.Width(100f));
            GUILayout.Label(this.definesEditted, EditorStyles.wordWrappedLabel);
            GUI.enabled = m_defineEdits.AnyEdits();
            if (GUILayout.Button("SAVE", EditorStyles.toolbarButton, GUILayout.Width(40f)))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(m_buildTargetGroup, this.definesEditted);
                ResetDefines();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Defines Current", EditorStyles.miniBoldLabel, GUILayout.Width(100f));
            GUILayout.Label(this.definesCurrent, EditorStyles.wordWrappedLabel);
            GUI.enabled = m_defineEdits.AnyEdits();
            if (GUILayout.Button("RESET", EditorStyles.miniButtonRight, GUILayout.Width(40f)))
            {
                ResetDefines();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Add Symbol", EditorStyles.miniBoldLabel, GUILayout.Width(100f));
            m_addSymbol = DefineEdits.PolishSymbol(
                GUILayout.TextField(m_addSymbol, EditorStyles.textField));
            
            GUI.enabled = !string.IsNullOrEmpty(m_addSymbol);
            if(GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(40f))
               || Event.current.keyCode == KeyCode.Return) {
                m_defineEdits.Set(m_addSymbol);
                m_addSymbol = "";
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            OnGui_ShowDefines(m_defineEdits);
            GUILayout.EndVertical();
        }

        private Vector2 scrollPos { get; set; }
        private void OnGui_ShowDefines(DefineEdits defineEdits)
        {
            using(var deflist = ListPool<DefineEditData>.Get()) {
                defineEdits.Get(deflist);

                this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, EditorStyles.helpBox);
                for (var i = 0; i < deflist.Count; i++)
                {
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    var willEnable = GUILayout.Toggle(deflist[i].willEnable, new GUIContent(deflist[i].name, deflist[i].desc));
                    defineEdits.Set(deflist[i].name, willEnable);

                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }

        }

    }
}

