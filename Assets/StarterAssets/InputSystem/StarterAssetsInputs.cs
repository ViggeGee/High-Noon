using Unity.Netcode;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;

		//ButtonSmash
		public bool buttonSmash1;
		public bool buttonSmash2;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

        public void OnAim(InputValue value)
        {
            AimInput(value.isPressed);
        }
        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }
        public void OnButtonSmash1(InputValue value)
        {
            ButtonSmashInput1(value.isPressed);
        }
        public void OnButtonSmash2(InputValue value)
        {
            ButtonSmashInput2(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
            //Debug.Log($"[StarterAssetsInputs] Look Input Updated: {look} - Player: {GetComponent<NetworkBehaviour>()?.OwnerClientId}");
        }


        public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }
        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
            //if(GameManager.Instance.readyToShoot)
            //{
            //             shoot = newShootState;
            //         }   
        }
        public void ButtonSmashInput1(bool newButtonSmash1State)
        {
            buttonSmash1 = newButtonSmash1State;
        }

        public void ButtonSmashInput2(bool newButtonSmash2State)
        {
            buttonSmash2 = newButtonSmash2State;
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}


		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}