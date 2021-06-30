using UnityEngine;

namespace MalnourishedMania
{
    public struct PlayerInput
    {
        public bool IsController()
        {
            if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].ToCharArray().Length > 0)
            {
                return true;
            }

            return false;
        }

        public bool GetJumpInput()
        {
            if(Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].ToCharArray().Length > 0)
            {
                return Input.GetButtonDown("JumpController");
            }

            return Input.GetKeyDown(KeyCode.Space);
        }

        public bool IsJumpInputKeyUp()
        {
            if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].ToCharArray().Length > 0)
            {
                return Input.GetButtonUp("JumpController");
            }

            return Input.GetKeyUp(KeyCode.Space);
        }

        public int GetHorizontalInput()
        {
            if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].ToCharArray().Length > 0)
            {
                return (int)Input.GetAxisRaw("HorizontalController");
            }

            return (int)Input.GetAxisRaw("HorizontalKBM");
        }

        public int GetVerticalInput()
        {
            if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].ToCharArray().Length > 0)
            {
                return (int)Input.GetAxisRaw("Vertical");
            }

            return (int)Input.GetAxisRaw("VerticalKBM");
        }

        public bool PauseInputIsPressed()
        {
            if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0].ToCharArray().Length > 0)
            {
                Debug.Log("ADD CONTROLLER START BUTTON");
            }

            return Input.GetKeyDown(KeyCode.Escape);
        }
    }
}
