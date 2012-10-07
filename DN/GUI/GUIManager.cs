using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DN.GUI
{
    public sealed class GUIManager
    {
        private readonly List<GUIObject> _guiObjects;

        public GUIManager()
        {
            _guiObjects = new List<GUIObject>();
        }

        public void Add(GUIObject guiObject)
        {
            _guiObjects.Add(guiObject);
        }

        public void Update(float dt)
        {
            foreach (var guiObject in _guiObjects)
            {
                guiObject.Update(dt);
            }
        }

        public void Draw(float dt)
        {
            foreach (var guiObject in _guiObjects)
            {
                guiObject.Draw(dt);
            }
        }

    }
}
