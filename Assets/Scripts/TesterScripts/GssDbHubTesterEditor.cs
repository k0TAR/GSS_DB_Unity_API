using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GssDbManageWrapper
{
    [CustomEditor(typeof(GssDbHubTester))]
    public class GssDbHubTesterEditor : Editor
    {
        // https://qiita.com/Gok/items/4ca51e2d3927ddfe69a5
        public override VisualElement CreateInspectorGUI()
        {
            GssDbHubTester gssDbHubTester = target as GssDbHubTester;

            var visualElement = new VisualElement();
            visualElement.Add(new IMGUIContainer(() => DrawDefaultInspector()));

            var button = new Button(() => gssDbHubTester.SendRequest())
            {
                text = "Send Request"
            };
            //button.clicked += () => { materialSettings.SendRequest(); };
            visualElement.Add(button);
            return visualElement;
        }
    }
}