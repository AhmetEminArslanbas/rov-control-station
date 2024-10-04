using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class GamepadManager : MonoBehaviour
{
    public static GamepadManager gamepadManager { get; private set; }

    Gamepad_input gamepad_input;

    [Range(-1f, 1f)]
    public float throttle;
    [Range(-1f, 1f)]
    public float yaw;
    [Range(-1f, 1f)]
    public float rol;
    [Range(-1f, 1f)]
    public float pitch;

    public float sd;
    public float sc;
    public float sb;
    public float sa;
    public float sg;
    public float se;
    public float sh;
    public float si;
    public float s2;
    public float rs;
    public float ls;

    private void Awake()
    {
        gamepad_input = new Gamepad_input();

        gamepad_input.gamepad.throttle.performed += context => { throttle = context.ReadValue<float>(); };
        gamepad_input.gamepad.yaw.performed += context => { yaw = context.ReadValue<float>(); };
        gamepad_input.gamepad.rol.performed += context => { rol = context.ReadValue<float>(); };
        gamepad_input.gamepad.pitch.performed += context => { pitch = context.ReadValue<float>(); };

        gamepad_input.gamepad.sd.performed += context => { sd = context.ReadValue<float>(); };
        gamepad_input.gamepad.sc.performed += context => { sc = context.ReadValue<float>(); };
        gamepad_input.gamepad.sb.performed += context => { sb = context.ReadValue<float>(); };
        gamepad_input.gamepad.sa.performed += context => { sa = context.ReadValue<float>(); };
        gamepad_input.gamepad.sg.performed += context => { sg = context.ReadValue<float>(); };
        gamepad_input.gamepad.se.performed += context => { se = context.ReadValue<float>(); };
        gamepad_input.gamepad.sh.performed += context => { sh = context.ReadValue<float>(); };
        gamepad_input.gamepad.si.performed += context => { si = context.ReadValue<float>(); };
        gamepad_input.gamepad.s2.performed += context => { s2 = context.ReadValue<float>(); };
        gamepad_input.gamepad.rs.performed += context => { rs = context.ReadValue<float>(); };
        gamepad_input.gamepad.ls.performed += context => { ls = context.ReadValue<float>(); };
    }

    private void OnEnable()
    {
        gamepad_input.gamepad.Enable();
    }
    private void OnDisable()
    {
        gamepad_input.gamepad.Enable();
    }
}