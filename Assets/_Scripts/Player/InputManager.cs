using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;


    public PlayerInputs Input { get; private set; }
    public InputMapEnum ActiveMap { get => activeMap; private set => activeMap = value; }
    public static InputManager Instance { get => instance;}

    private InputMapEnum activeMap = InputMapEnum.PLAYER_INPUT;

    private void Awake()
    {
        //Enforce singleton pattern
        if (Instance != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        instance = this;
        Input = new PlayerInputs();

    }



    public void SwitchToPlayerInput()
    {
        SetActiveMap(Input.GameInputs);
        SetCursorMode(CursorLockMode.Confined);
        Cursor.visible = true;
        activeMap = InputMapEnum.PLAYER_INPUT;
    }

    private void SetCursorMode(CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;
    }


    private void SetActiveMap(InputActionMap activeMap)
    {

        Debug.Log("Setting input map to " + activeMap);
        foreach (InputActionMap map in Input.asset.actionMaps)
        {
            if (activeMap == map)
            {
                map.Enable();
            }
            else
            {
                map.Disable();
            }
        }
    }


 

    //public void SwitchToMenuInput()
    //{
    //    GameManager.Instance.Player.DisablePlayerInput();

    //    Input.Menu.Enable();
    //    Input.PlayerMain.Disable();

    //    SetCursorMode(CursorLockMode.Confined);
    //    Cursor.visible = true;
    //    activeMap = InputMapEnum.MENU;
    //}


    public enum InputMapEnum
    {
        NONE = 0,
        PLAYER_INPUT = 1,
        MENU =2

    }
}
