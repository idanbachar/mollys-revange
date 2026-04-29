using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection
{
    [Serializable]
    public class PlayerData
    {
        private int health;
        private float xPos;
        private float yPos;
        private float rotation;
        private float xSpeed;
        private float ySpeed;
        private string ip;
        private bool canShoot;
        private float xDirection;
        private float yDirection;

        public PlayerData(int health, float xPos, float yPos, float rotation, float xSpeed, float ySpeed, string ip, bool canShoot, float xDirection, float yDirection) {

            this.health = health;
            this.xPos = xPos;
            this.yPos = yPos;
            this.rotation = rotation;
            this.xSpeed = xSpeed;
            this.ySpeed = ySpeed;
            this.ip = ip;
            this.canShoot = canShoot;
            this.xDirection = xDirection;
            this.yDirection = yDirection;
        }

        public int GetHealth() {
            return health;
        }

        public void SetHealth(int newHealth) {
            this.health = newHealth;
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

        public float GetXSpeed() {
            return xSpeed;
        }

        public void SetXSpeed(float newXSpeed) {
            xSpeed = newXSpeed;
        }

        public float GetYSpeed() {
            return ySpeed;
        }

        public void SetYSpeed(float newYSpeed) {
            ySpeed = newYSpeed;
        }

        public float GetRotation() {
            return rotation;
        }

        public void SetRotation(float newRotation) {
            rotation = newRotation;
        }

        public string GetIp() {
            return ip;
        }

        public bool GetCanShoot() {
            return canShoot;
        }

        public void SetCanShoot(bool value) {
            canShoot = value;
        }

        public float GetYDirection() {
            return yDirection;
        }

        public void SetYDirection(float newYDirection) {
            yDirection = newYDirection;
        }

        public float GetXDirection() {
            return xDirection;
        }

        public void SetXDirection(float newXDirection) {
            xDirection = newXDirection;
        }

        public override string ToString() {
            return string.Format("health = {0}, position = {1}, {2}, speed={3},{4}, rotation={5}, canShoot={6}", GetHealth(), GetXPos(), GetYPos(), GetXSpeed(), GetYSpeed(), GetRotation(), GetCanShoot());
        }
    }
}
