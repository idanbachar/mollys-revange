using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Entity
    {
        private int fresh;
        private string name;
        private float xPos;
        private float yPos;

        public Entity(float xPos, float yPos, int fresh, string name){

            this.fresh = fresh;
            this.name = name;
            this.xPos = xPos;
            this.yPos = yPos;
        }

        public Entity(float xPos, float yPos, string name) {

            this.fresh = 0;
            this.name = name;
            this.xPos = xPos;
            this.yPos = yPos;
        }

        public int GetFresh() {
            return fresh;
        }

        public void SetFresh(int newFresh) {
            fresh = newFresh;
        }

        public float GetXPos() {
            return xPos;
        }

        public void SetXPos(int newXPos) {
            xPos = newXPos;
        }

        public float GetYPos() {
            return yPos;
        }

        public void SetYPos(int newYPos) {
            yPos = newYPos;
        }

        public string GetName() {
            return name;
        }

        public void SetName(string newName) {
            name = newName;
        }
    }
}
