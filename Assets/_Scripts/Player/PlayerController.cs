using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private InputManager inputs;

    private void Start()
    {
        inputs = InputManager.Instance;
        inputs.SwitchToPlayerInput();
    }

}
