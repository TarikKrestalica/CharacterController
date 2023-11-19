using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    // Setting up the controller : https://youtu.be/p-3S73MaDP8?si=vQZRlzvkSdZx6AMT
    protected PlayerControls controls;

    [SerializeField] private float speedDelay;
    private float curSpeedDelay = 0f;

    protected void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Jump.performed += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            if (!GameManager.playerController.CanJump())
                return;

            GameManager.playerController.jumpKeyPressed = true;
            GameManager.playerController.Jump();
        };

        controls.Gameplay.Move.performed += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            GameManager.playerController.horizontalInput = ctx.ReadValue<float>();
            if (!GameManager.playerController.IsGrounded())
            {
                GameManager.playerController.horizontalInput /= 2f;
            }
        };

        controls.Gameplay.Move.canceled += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            GameManager.playerController.horizontalInput = ctx.ReadValue<float>();
        };

        controls.Gameplay.Dash.canceled += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            GameManager.playerController.SlowDown();
        };

        controls.Gameplay.Crouch.performed += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            if (GameManager.playerController.GetCrouchedState())
                return;

            if (GameManager.playerController.transform.localScale.y - (GameManager.playerController.GetCurrentSize().y - GameManager.playerController.GetCrouchedSize().y) <= 0)
                return;

            GameManager.playerController.Crouch();
        };

        controls.Gameplay.Crouch.canceled += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            if (!GameManager.playerController.GetCrouchedState())
                return;

            GameManager.playerController.RevertCrouch();
        };

        // TODO: Organize this functionality
        controls.Gameplay.DropItem.performed += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            if (GameManager.playerController.playerInventory.items.Count <= 0)
            {
                return;
            }

            GameManager.playerController.playerInventory.DropItem();
            
            // Update the Inventory UI to accomodate that the item has been removed!
            if (GameManager.playerController.playerInventory.items.Count <= 0)
            {
                GameObject.FindGameObjectWithTag("CurItemDisplay").GetComponent<CurItemSlotScript>().RemoveCurItemSlot();
            }
            else
            {
                ItemDefinition previousItem = GameManager.playerController.playerInventory.items[GameManager.playerController.playerInventory.items.Count - 1];
                GameObject.FindGameObjectWithTag("CurItemDisplay").GetComponent<CurItemSlotScript>().UpdateCurItemSlot(previousItem);
            }
        };

        controls.Gameplay.Pause.performed += ctx =>
        {
            GameManager.PausePerformed();
        };

        controls.Gameplay.OnNextItemToLeft.performed += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            ItemDefinition leftItem = GameManager.playerController.playerInventory.FindNextItemToTheLeft();
            if(leftItem.name != "Heart")  // Hearts are collectibles, do not requip unless dropped!
            { 
                GameManager.playerController.playerInventory.ReEquipItem(leftItem);
            }
            
            if(leftItem.tag == "Item")
            {
                GameManager.playerController.itemHolder.sprite = leftItem.sprite;
            }
            GameObject.FindGameObjectWithTag("CurItemDisplay").GetComponent<CurItemSlotScript>().UpdateCurItemSlot(leftItem);
        };

        controls.Gameplay.OnNextItemToRight.performed += ctx =>
        {
            if (GameManager.IsPaused())
                return;

            ItemDefinition rightItem = GameManager.playerController.playerInventory.FindNextItemToTheRight();
            if (rightItem.name != "Heart")  // Hearts are collectibles, do not requip unless dropped!
            {
                GameManager.playerController.playerInventory.ReEquipItem(rightItem);
            }
            if (rightItem.tag == "Item")
            {
                GameManager.playerController.itemHolder.sprite = rightItem.sprite;
            }
            GameObject.FindGameObjectWithTag("CurItemDisplay").GetComponent<CurItemSlotScript>().UpdateCurItemSlot(rightItem);
        };

        controls.Gameplay.TryToGoToNextLevel.performed += ctx => GameManager.gameManager.TryToGoToNextScene();

    }

    // When I perform actions using the right keys, I want to access them(opposite is true as well)
    protected void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    protected void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void Update()
    {
        float dashValue = controls.Gameplay.Dash.ReadValue<float>();
        float dashFactor = 0;
        if (dashValue > 0.1f)  // Resolving the dash mechanic: Speed up overtime https://youtu.be/0z-6lxL_sS4?si=2aYM6uEfE26993Ds
        {
            if (curSpeedDelay > 0f)
            {
                curSpeedDelay -= Time.deltaTime;
                return;
            }

            if(GameManager.playerController.horizontalInput == 0)
            {
                GameManager.playerController.SlowDown();
                return;
            }

            // Find dash factors to apply speed boost
            if (GameManager.playerController.GetCrouchedState())
            {
                dashFactor = GameManager.playerController.playerStats.FindMaxValue(StatType.DashSpeed) / 2;
            }
            else
            {
                dashFactor = GameManager.playerController.playerStats.FindMaxValue(StatType.DashSpeed);
            }
            GameManager.playerController.Dash(dashFactor);
            curSpeedDelay = speedDelay;
        }
    }

}
