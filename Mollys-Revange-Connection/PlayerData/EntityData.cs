using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    [Serializable]
    public class EntityData
    {
        private int fresh;
        private float xPos;
        private float yPos;
        private float rotation;
        private string name;

        public EntityData(int fresh, float xPos, float yPos, float rotation, string name) {
            
            this.xPos = xPos;
            this.yPos = yPos;
            this.rotation = rotation;
            this.name = name;
            this.fresh = fresh;
        }

        public int GetFresh() {
            return fresh;
        }

        public void SetFresh(int newFresh) {
            this.fresh = newFresh;
        }

        public float GetXPos() {
            return xPos;
        }

        public void SetXPos(float newXPos) {
            xPos = newXPos;
        }

        public float GetYPos() {
            return yPos;
        }

        public void SetYPos(float newYPos) {
            yPos = newYPos;
        }
 
        public float GetRotation() {
            return rotation;
        }

        public void SetRotation(float newRotation) {
            rotation = newRotation;
        }

        public string GetName() {
            return name;
        }

        public void SetName(string newName) {
            name = newName;
        }

 
        public override string ToString() {
            return string.Format("fresh = {0}, position = {1}, {2}, rotation={3}, name={4}", GetFresh(), GetXPos(), GetYPos(), GetRotation(), GetName());
        }
    }
}
